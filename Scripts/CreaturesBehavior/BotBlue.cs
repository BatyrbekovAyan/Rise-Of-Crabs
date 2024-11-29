using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class BotBlue : MonoBehaviour
{
    [SerializeField] public GameObject BotBlueImage;
    [SerializeField] private TextMeshProUGUI CurrentLifeText;
    [SerializeField] private GameObject Sword;
    [SerializeField] private GameObject Sword2;
    [SerializeField] private GameObject LightSaber;
    [SerializeField] private GameObject Shield;
    [SerializeField] private GameObject Shield2;
    [SerializeField] private GameObject Helmet;
    [SerializeField] private GameObject Helmet2;

    private bool moveEnabled = false;
    private float speed;
    public int currentLife;
    private int totalLife;

    private Color brown = new(0.4f, 0.2f, 0);
    private Color blue = new(0, 0, 0.4f);
    private Color violet = new(0.3f, 0, 0.4f);


    private void Start()
    {
        speed = 0.4f;

        CurrentLifeText.text = currentLife.ToString();

        StartCoroutine(DelayMove());
    }

    private void FixedUpdate()
    {
        if (moveEnabled && !GameManager.freezing)
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }

        if (Mathf.Abs(transform.position.x - GameManager.houseRedPosition.x) < 0.01)
        {
            transform.parent.parent.parent.parent.GetComponent<GameManager>().GameOver();
            GameManager.redHouseAttacked = true;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("BotRed"))
        {
            if (currentLife > collision.GetComponent<BotRed>().currentLife)
            {
                currentLife -= collision.GetComponent<BotRed>().currentLife;
                CurrentLifeText.text = currentLife.ToString();

                StartCoroutine(DestroyBotRed(collision.gameObject));
            }
            else if(currentLife < collision.GetComponent<BotRed>().currentLife)
            {
                collision.GetComponent<BotRed>().currentLife -= currentLife;
                collision.GetComponent<BotRed>().CurrentLifeText.text = collision.GetComponent<BotRed>().currentLife.ToString();

                StartCoroutine(DestroyBotBlue());
            }
            else
            {
                StartCoroutine(DestroyBotBlue());
                StartCoroutine(DestroyBotRed(collision.gameObject));
            }
        }
        else if (collision.transform.CompareTag("HouseRed"))
        {
            if (GameManager.redCreaturesLeft > 0)
            {
                if (Sound.SoundEnabled)
                {
                    Sound.BotHitBlueHouse.Play();
                }

                GameManager.redCreaturesLeft -= totalLife;

                if (GameManager.redCreaturesLeft < 0)
                {
                    GameManager.redCreaturesLeft = 0;
                }

                transform.parent.parent.parent.parent.GetComponent<GameManager>().UpdateCreaturesLeftText();

                collision.gameObject.GetComponent<Animation>().Play("HouseRed");
                StartCoroutine(DestroyBotBlue());
            }
        }
    }

    private IEnumerator DestroyBotBlue()
    {
        gameObject.GetComponent<ParticleSystem>().GetComponent<Renderer>().material.mainTexture = BotBlueImage.GetComponent<Image>().sprite.texture;
        gameObject.GetComponent<ParticleSystem>().Play();

        if (Sound.SoundEnabled)
        {
            Sound.BlueBotCrash.Play();
        }

        GameManager.psPlayingBotsList.Add(gameObject);

        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        moveEnabled = false;

        Destroy(BotBlueImage);
        Destroy(CurrentLifeText.gameObject);

        Creatures.botBlueList.Remove(gameObject);

        GameManager.UpdateSliderValue();

        yield return new WaitForSeconds(2);

        GameManager.psPlayingBotsList.Remove(gameObject);

        Destroy(gameObject);
    }

    private IEnumerator DestroyBotRed(GameObject botRed)
    {
        botRed.GetComponent<ParticleSystem>().GetComponent<Renderer>().material.mainTexture = botRed.GetComponent<BotRed>().BotRedImage.GetComponent<Image>().sprite.texture;
        botRed.GetComponent<ParticleSystem>().Play();

        if (Sound.SoundEnabled)
        {
            Sound.RedBotCrash.Play();
        }

        GameManager.psPlayingBotsList.Add(botRed);

        botRed.GetComponent<BoxCollider2D>().enabled = false;
        botRed.GetComponent<BotRed>().moveEnabled = false;

        Destroy(botRed.GetComponent<BotRed>().BotRedImage);
        Destroy(botRed.GetComponent<BotRed>().CurrentLifeText.gameObject);

        Creatures.botRedList.Remove(botRed);

        GameManager.UpdateSliderValue();

        yield return new WaitForSeconds(2);

        GameManager.psPlayingBotsList.Remove(botRed);

        Destroy(botRed);
    }

    private IEnumerator DelayMove()
    {
        yield return new WaitForSeconds(1);

        if (GameManager.freezing)
        {
            yield return new WaitForSeconds(GameManager.freezingTime);
        }

        transform.GetChild(0).GetComponent<Image>().enabled = false;
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

        CurrentLifeText.gameObject.SetActive(true);

        Creatures.botBlueList.Add(gameObject);

        transform.GetComponent<BoxCollider2D>().enabled = true;

        moveEnabled = true;
    }

    public IEnumerator SetBotBlueLevel()
    {
        yield return new WaitForSeconds(0.1f);

        totalLife = currentLife;

        if (currentLife > 7)
        {
            BotBlueImage.transform.GetChild(0).transform.localScale = 1.6f * BotBlueImage.transform.GetChild(0).transform.localScale;
            BotBlueImage.transform.GetChild(0).transform.localPosition = new Vector2(BotBlueImage.transform.GetChild(0).transform.localPosition.x, BotBlueImage.GetComponent<RectTransform>().rect.y);
            CurrentLifeText.GetComponent<RectTransform>().localPosition = new Vector2(CurrentLifeText.GetComponent<RectTransform>().localPosition.x, BotBlueImage.GetComponent<RectTransform>().rect.size.y / 1.12f + CurrentLifeText.GetComponent<RectTransform>().localPosition.y / 2 + 2 * Screen.height / Screen.width);

            BotBlueImage.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = violet;
            LightSaber.SetActive(true);
        }
        else if (currentLife > 6)
        {
            BotBlueImage.transform.GetChild(0).transform.localScale = 1.4f * BotBlueImage.transform.GetChild(0).transform.localScale;
            BotBlueImage.transform.GetChild(0).transform.localPosition = new Vector2(BotBlueImage.transform.GetChild(0).transform.localPosition.x, BotBlueImage.GetComponent<RectTransform>().rect.y);
            CurrentLifeText.GetComponent<RectTransform>().localPosition = new Vector2(CurrentLifeText.GetComponent<RectTransform>().localPosition.x, BotBlueImage.GetComponent<RectTransform>().rect.size.y / 1.4f + CurrentLifeText.GetComponent<RectTransform>().localPosition.y / 2 + 2 * Screen.height / Screen.width);

            BotBlueImage.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = blue;
            Helmet2.SetActive(true);
            Sword2.SetActive(true);
            Shield2.SetActive(true);
        }
        else if (currentLife > 5)
        {
            BotBlueImage.transform.GetChild(0).transform.localScale = 1.4f * BotBlueImage.transform.GetChild(0).transform.localScale;
            BotBlueImage.transform.GetChild(0).transform.localPosition = new Vector2(BotBlueImage.transform.GetChild(0).transform.localPosition.x, BotBlueImage.GetComponent<RectTransform>().rect.y);
            CurrentLifeText.GetComponent<RectTransform>().localPosition = new Vector2(CurrentLifeText.GetComponent<RectTransform>().localPosition.x, BotBlueImage.GetComponent<RectTransform>().rect.size.y / 1.4f + CurrentLifeText.GetComponent<RectTransform>().localPosition.y / 2 + 2 * Screen.height / Screen.width);

            BotBlueImage.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = blue;
            Sword2.SetActive(true);
            Shield2.SetActive(true);
        }
        else if (currentLife > 4)
        {
            BotBlueImage.transform.GetChild(0).transform.localScale = 1.4f * BotBlueImage.transform.GetChild(0).transform.localScale;
            BotBlueImage.transform.GetChild(0).transform.localPosition = new Vector2(BotBlueImage.transform.GetChild(0).transform.localPosition.x, BotBlueImage.GetComponent<RectTransform>().rect.y);
            CurrentLifeText.GetComponent<RectTransform>().localPosition = new Vector2(CurrentLifeText.GetComponent<RectTransform>().localPosition.x, BotBlueImage.GetComponent<RectTransform>().rect.size.y / 1.4f + CurrentLifeText.GetComponent<RectTransform>().localPosition.y / 2 + 2 * Screen.height / Screen.width);

            BotBlueImage.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = blue;
            Shield2.SetActive(true);
        }
        else if (currentLife > 3)
        {
            BotBlueImage.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = brown;
            Helmet.SetActive(true);
            Sword.SetActive(true);
            Shield.SetActive(true);
        }
        else if (currentLife > 2)
        {
            BotBlueImage.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = brown;
            Sword.SetActive(true);
            Shield.SetActive(true);
        }
        else if (currentLife > 1)
        {
            BotBlueImage.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = brown;
            Shield.SetActive(true);
        }


        if (!GameManager.HouseBlueLevelTextStatic.text.Equals(currentLife.ToString()))
        {
            GameManager.HouseBlueLevelTextStatic.text = currentLife.ToString();
            GameManager.HouseBlueLevelTextStatic.GetComponent<Animation>().Play();
        }

        CurrentLifeText.text = currentLife.ToString();
    }

}
