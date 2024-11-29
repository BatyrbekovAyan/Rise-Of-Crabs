using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class Minus2 : MonoBehaviour, ICreatureBody
{
    [SerializeField] private GameObject Animation;
    [SerializeField] private GameObject Coin;

    private string sign = "-";
    private int number;

    private bool moveEnabled = false;
    private int totalLife;
    private int currentLife;
    private float speed;
    private float endPointPresition;
    private float horizontalDistanceMin;
    private float horizontalDistanceMax;
    private float verticalDistanceMin;
    private float verticalDistanceMax;
    private float leftLimit;
    private float rightLimit;
    private Vector2 newPosition;
    private Transform SignAndNumberText;
    private Slider Slider;


    public void Start()
    {
        number = (int)Mathf.Pow(2f, Random.Range(0, 1 + (Levels.level - 1) % Creatures.sameLevelsNumber + Mathf.FloorToInt(Mathf.Sqrt(Creatures.levelRow - 1)) - Mathf.FloorToInt((Levels.level - 1) / 48)));

        if (GameManager.doubleCounting)
        {
            number *= 2;
        }

        totalLife = 12 + Creatures.levelRow / 2;
        currentLife = totalLife;
        speed = 0.1f * Mathf.Sqrt(30 + 0.1f * Levels.level);
        endPointPresition = Random.Range(0.001f, 0.1f);
        horizontalDistanceMin = 1f;
        horizontalDistanceMax = 2f;
        verticalDistanceMin = 0.2f;
        verticalDistanceMax = 0.6f;
        leftLimit = Creatures.leftWallInnerEdge;
        rightLimit = Creatures.righWallInnerEdge;
        newPosition = transform.position;
        SignAndNumberText = transform.GetChild(0);
        SignAndNumberText.GetComponent<TextMeshProUGUI>().text = sign + number + "";
        Slider = transform.GetChild(1).GetComponent<Slider>();
        Slider.maxValue = totalLife;
        Slider.value = totalLife;

        transform.parent.GetComponent<Creature>().totalLife = 3 + Creatures.levelRow / 16;

        if (RandomBool())
        {
            Turn();
        }

        StartCoroutine(DelayMove());
    }

    public void FixedUpdate()
    {
        if (moveEnabled)
        {
            if (!GameManager.freezing)
            {
                transform.position = Vector2.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);

                if (Mathf.Abs(transform.position.x - newPosition.x) < endPointPresition)
                {
                    if (RandomBool())
                    {
                        Turn();
                    }

                    newPosition = new Vector2(transform.position.x + Random.Range(horizontalDistanceMin, horizontalDistanceMax), transform.position.y - Random.Range(verticalDistanceMin, verticalDistanceMax));
                }

                if (transform.position.x < leftLimit || transform.position.x > rightLimit)
                {
                    if (transform.position.x < leftLimit)
                    {
                        newPosition = new Vector2(2 * leftLimit - newPosition.x, newPosition.y);
                    }
                    else if (transform.position.x > rightLimit)
                    {
                        newPosition = new Vector2(2 * rightLimit - newPosition.x, newPosition.y);
                    }

                    Turn();
                }
            }


            Slider.value = Mathf.MoveTowards(Slider.value, currentLife, 100 * Time.deltaTime);

            if (Slider.value == 0)
            {
                moveEnabled = false;

                StartCoroutine(Shot());
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ball") || collision.transform.CompareTag("BallBot"))
        {
            if (currentLife > 0)
            {
                if (collision.transform.CompareTag("Ball"))
                {
                    currentLife -= collision.gameObject.GetComponent<Ball>().ballPower;
                }
                else
                {
                    currentLife -= collision.gameObject.GetComponent<BallBot>().ballPower;
                }

                Animation.transform.SetPositionAndRotation(transform.position, transform.rotation);
                Animation.SetActive(true);
                Animation.GetComponent<Animation>().Play();

                if (currentLife <= 0)
                {
                    gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
                }
            }
        }

        else if (collision.transform.CompareTag("Wall"))
        {
            if (collision.transform.name == "RightWall")
            {
                newPosition = new Vector2(2 * collision.transform.position.x - newPosition.x - transform.GetComponent<Image>().sprite.bounds.size.x / 10 - 2 * collision.collider.bounds.size.x, newPosition.y);

                Turn();
            }
            else if (collision.transform.name == "LeftWall")
            {
                newPosition = new Vector2(2 * collision.transform.position.x - newPosition.x + transform.GetComponent<Image>().sprite.bounds.size.x / 10 + 2 * collision.collider.bounds.size.x, newPosition.y);

                Turn();
            }
        }
    }

    public IEnumerator Hooked()
    {
        if (Sound.SoundEnabled)
        {
            Sound.BlueBotCrash.Play();
        }

        transform.GetComponent<Image>().enabled = false;
        Slider.gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);

        Creatures.blueCreaturesToCreate.Add(transform.parent.gameObject);

        GameManager.Instance.UpdateCrabsNumber(number, sign);

        SignAndNumberText.SetParent(transform.parent);

        SignAndNumberText.DOMove(GameManager.OperationMadeTextStatic.transform.position, 1f).OnComplete(() =>
        {
            GameManager.OperationMadeTextStatic.text = sign + number + "";
            GameManager.OperationMadeTextStatic.gameObject.GetComponent<Animation>().Play("EnlargeText");
        });

        yield return new WaitForSeconds(1f);

        SignAndNumberText.gameObject.SetActive(false);

        Destroy(gameObject);
    }

    public IEnumerator Shot()
    {
        transform.GetComponent<Animation>().Play("CreatureDisappear");

        if (Sound.SoundEnabled)
        {
            Sound.CreatureDied.Play();
        }

        yield return new WaitForSeconds(0.5f);

        Coin.transform.position = gameObject.transform.position;
        Coin.SetActive(true);
        Coin.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);

        transform.parent.GetComponent<Creature>().GivePoints(5);

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        Creatures.creaturesBodiesList.Remove(gameObject);
    }

    private bool RandomBool()
    {
        return (Random.value > 0.5f);
    }

    public IEnumerator DelayMove()
    {
        yield return new WaitForSeconds(1);

        if (!GameManager.hookEnabled || Hooker.switchedToShooter)
        {
            Slider.gameObject.SetActive(true);
        }
        else
        {
            SignAndNumberText.gameObject.SetActive(true);
        }

        moveEnabled = true;
    }

    private void Turn()
    {
        horizontalDistanceMin = -horizontalDistanceMin;
        horizontalDistanceMax = -horizontalDistanceMax;

        if (transform.GetComponent<RectTransform>().rotation.y == 0)
        {
            transform.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        }

        SignAndNumberText.transform.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        Slider.transform.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
    }

    public void DisableScript()
    {
        enabled = false;
    }

    public int GetNumber()
    {
        return number;
    }

    public void SetNumber(float multiplier)
    {
        number = (int)(number * multiplier);
        SignAndNumberText.GetComponent<TextMeshProUGUI>().text = sign + number + "";
    }
}
