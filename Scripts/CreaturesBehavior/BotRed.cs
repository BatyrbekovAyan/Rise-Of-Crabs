using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class BotRed : MonoBehaviour
{
    [SerializeField] public GameObject BotRedImage;
    [SerializeField] public TextMeshProUGUI CurrentLifeText;
    [SerializeField] private List<Sprite> CrabsList;

    public int activeCrabIndex;
    public bool moveEnabled = false;
    private float speed;
    public int currentLife;
    private int totalLife;


    private void Awake()
    {
        activeCrabIndex = 0;
        speed = 0.4f;
        currentLife = 20;
        totalLife = currentLife;
        CurrentLifeText.text = currentLife.ToString();

        StartCoroutine(DelayMove());
    }

    private void FixedUpdate()
    {
        if (moveEnabled && !GameManager.freezing)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }

        if (Mathf.Abs(transform.position.x - GameManager.houseBluePosition.x) < 0.01)
        {
            transform.parent.parent.parent.parent.GetComponent<GameManager>().LevelWin();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("HouseBlue"))
        {
            if (Creatures.blueCreaturesTotal > 0)
            {
                if (Sound.SoundEnabled)
                {
                    Sound.BotHitBlueHouse.Play();
                }

                Creatures.blueCreaturesTotal -= totalLife;

                if (Creatures.blueCreaturesTotal <= 0)
                {
                    Creatures.blueCreaturesTotal = 0;

                    if (Creatures.portalsEnabled)
                    {
                        StartCoroutine(Creatures.DisablePortals());
                    }
                }

                Creatures.blueCreaturesTotalTexStatic.text = Creatures.blueCreaturesTotal.ToString();

                collision.gameObject.GetComponent<Animation>().Play("HouseBlue");

                StartCoroutine(DestroyBotRed());
            }
        }
    }

    private IEnumerator DestroyBotRed()
    {
        gameObject.GetComponent<ParticleSystem>().GetComponent<Renderer>().material.mainTexture = BotRedImage.GetComponent<Image>().sprite.texture;
        gameObject.GetComponent<ParticleSystem>().Play();

        if (Sound.SoundEnabled)
        {
            Sound.RedBotCrash.Play();
        }

        GameManager.psPlayingBotsList.Add(gameObject);

        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        moveEnabled = false;

        Destroy(BotRedImage);
        Destroy(CurrentLifeText.gameObject);

        Creatures.botRedList.Remove(gameObject);

        GameManager.UpdateSliderValue();

        yield return new WaitForSeconds(2);

        GameManager.psPlayingBotsList.Remove(gameObject);

        Destroy(gameObject);
    }

    private IEnumerator DelayMove()
    {
        yield return new WaitForSeconds(1);

        if (GameManager.freezing)
        {
            yield return new WaitForSeconds(GameManager.freezingTime);
        }

        transform.GetChild(0).GetComponent<Image>().enabled = false;
        transform.GetChild(0).GetChild(activeCrabIndex).gameObject.SetActive(true);

        CurrentLifeText.gameObject.SetActive(true);

        Creatures.botRedList.Add(gameObject);

        transform.GetComponent<BoxCollider2D>().enabled = true;

        moveEnabled = true;
    }

    public IEnumerator SetBotRedLevel(int botRedLevel)
    {
        yield return new WaitForSeconds(0.1f);

        activeCrabIndex = botRedLevel - 1;


        if (activeCrabIndex > 0)
        {
            BotRedImage.GetComponent<Image>().sprite = CrabsList[activeCrabIndex - 1];
        }

        currentLife = 2 * botRedLevel + Creatures.levelRow / 16;
        totalLife = currentLife;
        BotRedImage.GetComponent<RectTransform>().sizeDelta = (1 + 0.2f * activeCrabIndex) * BotRedImage.GetComponent<RectTransform>().sizeDelta;
        BotRedImage.transform.GetChild(activeCrabIndex).transform.localScale = BotRedImage.transform.GetChild(activeCrabIndex).transform.localScale * Creatures.sizeRatioRed * (1 + 0.2f * activeCrabIndex);
        BotRedImage.transform.GetChild(activeCrabIndex).transform.localPosition = new Vector2(BotRedImage.transform.GetChild(activeCrabIndex).transform.localPosition.x, BotRedImage.GetComponent<RectTransform>().rect.y);
        CurrentLifeText.GetComponent<RectTransform>().localPosition = new Vector2(0, BotRedImage.GetComponent<RectTransform>().rect.size.y / 2 + CurrentLifeText.GetComponent<RectTransform>().localPosition.y / 2 + 2 * Screen.height / Screen.width);
        transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y / (1 + 0.1f * activeCrabIndex));
        transform.GetComponent<BoxCollider2D>().size = new Vector2(BotRedImage.GetComponent<RectTransform>().rect.size.y * BotRedImage.GetComponent<Image>().sprite.rect.width / BotRedImage.GetComponent<Image>().sprite.rect.height, BotRedImage.GetComponent<RectTransform>().rect.size.y);

        if (botRedLevel == 1)
        {
            BotRedImage.transform.GetChild(activeCrabIndex).transform.localScale *= 1.054f;
        }
        if (botRedLevel == 5)
        {
            BotRedImage.transform.GetChild(activeCrabIndex).transform.localScale *= 1.066f;
        }

        CurrentLifeText.text = currentLife.ToString();
    }
}