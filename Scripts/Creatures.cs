using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Creatures : MonoBehaviour
{
    [SerializeField] private GameObject BotBlue;
    [SerializeField] private GameObject BotRed;
    [SerializeField] private GameObject LeftWall;
    [SerializeField] private GameObject RightWall;
    [SerializeField] private GameObject PlaceholderBlue;
    [SerializeField] private GameObject PlaceholderRed;
    [SerializeField] private GameObject BotBlueParent;
    [SerializeField] private GameObject BotRedParent;
    [SerializeField] private GameObject Portals;
    [SerializeField] private GameObject ShooterBot;
    [SerializeField] private GameObject ShooterBots;
    [SerializeField] private TextMeshProUGUI blueCreaturesTotalText;
    [SerializeField] private List<GameObject> CreaturesList;
    [SerializeField] private List<GameObject> PortalsList;


    public static List<GameObject> botBlueList = new();
    public static List<GameObject> botRedList = new();
    public static List<GameObject> creaturesBodiesList = new();
    public static List<GameObject> blueCreaturesToCreate = new();

    public static float leftWallInnerEdge;
    public static float righWallInnerEdge;
    public static float sizeRatioRed;
    public static float sizeRatioBlue;
    public static int levelRow;
    public static int sameLevelsNumber;
    public static int blueCreaturesTotal;
    public static int botRedLevel;
    public static bool movePortals;
    public static bool portalsEnabled;
    public static TextMeshProUGUI blueCreaturesTotalTexStatic;
    private static List<GameObject> activePortals;

    private float screenWidth;
    private float portalsSpeed;
    private float blueCreaturesAppearInterval;
    private int powerOfTen;
    private int offset;
    private int shooterBotsNumber;
    private int subtructPower;
    private bool creaturesCreationEnabled;
    private bool redCreationEnabled;
    private bool blueCreationEnabled;
    private Vector3 shooterBotVerticalPos;


    private void Start()
    {
        creaturesBodiesList.Clear();
        blueCreaturesToCreate.Clear();
        botBlueList.Clear();
        botRedList.Clear();
        activePortals = new();
        blueCreaturesTotal = (int)Mathf.Floor(20 + 8 * Mathf.Sqrt(Levels.level));
        GameManager.redCreaturesLeft = 0;
        blueCreaturesTotalText.text = blueCreaturesTotal.ToString();
        blueCreaturesTotalTexStatic = blueCreaturesTotalText;
        sameLevelsNumber = 3;
        botRedLevel = 1;
        subtructPower = 0;
        powerOfTen = 0;
        sizeRatioRed = PlaceholderRed.GetComponent<RectTransform>().rect.size.y / BotRed.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
        sizeRatioBlue = PlaceholderBlue.GetComponent<RectTransform>().rect.size.y / BotBlue.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
        blueCreaturesAppearInterval = 0.8f + 4 / Mathf.Sqrt(Levels.level);
        creaturesCreationEnabled = true;
        redCreationEnabled = true;
        blueCreationEnabled = true;
        leftWallInnerEdge = -Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        righWallInnerEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        screenWidth = RightWall.transform.position.x - LeftWall.transform.position.x;

        CreatePortals();

        CreateShooterBots();

        transform.SetAsLastSibling();
    }

    private void FixedUpdate()
    {
        if (!LevelStart.touched)
        {
            return;
        }

        if (creaturesCreationEnabled && blueCreaturesTotal > 0 && !GameManager.freezing)
        {
            creaturesCreationEnabled = false;
            StartCoroutine(CreateCreature(GetRandomCreatureIndex()));
            EnablePortals();
        }

        if (redCreationEnabled && GameManager.redCreaturesLeft > 0 && !GameManager.freezing)
        {
            redCreationEnabled = false;
            StartCoroutine(CreateRed());
        }

        if (blueCreaturesToCreate.Count != 0 && blueCreationEnabled && blueCreaturesTotal > 0 && !GameManager.freezing)
        {
            blueCreationEnabled = false;
            StartCoroutine(CreateBLue());
        }

        if (movePortals && !GameManager.freezing)
        {
            Portals.transform.position = Vector2.MoveTowards(Portals.transform.position, Vector3.zero, portalsSpeed * Time.deltaTime);
        }

        if (botBlueList.Count <= 0 && botRedList.Count <= 0 && creaturesBodiesList.Count <= 0 && GameManager.redCreaturesLeft <= 0 && blueCreaturesTotal <= 0)
        {
            transform.parent.parent.GetComponent<GameManager>().GameOver();
        }
    }

    private IEnumerator CreateCreature(int creatureIndex)
    {
        yield return new WaitForSeconds(1f);

        if (GameManager.freezing)
        {
            yield return new WaitForSeconds(GameManager.freezingTime);
        }

        GameObject creature = Instantiate(CreaturesList[creatureIndex % CreaturesList.Count], activePortals[(creatureIndex - offset) % CreaturesList.Count].transform.position, CreaturesList[creatureIndex % CreaturesList.Count].transform.rotation, gameObject.transform);
        creaturesBodiesList.Add(creature.transform.GetChild(2).gameObject);

        creature.SetActive(true);

        creaturesCreationEnabled = true;
    }

    private IEnumerator CreateRed()
    {
        for (int i = 5; i > 1; i--)
        {
            if (GameManager.redCreaturesLeft >= (int)Mathf.Pow(10, i))
            {
                if (botRedLevel < i)
                {
                    botRedLevel = i;
                    break;
                }
            }
        }

        powerOfTen = botRedLevel - 1;
        yield return new WaitForSeconds(0.1f * powerOfTen);


        GameObject botRed = Instantiate(BotRed, PlaceholderRed.transform.position, BotRed.transform.rotation, BotRedParent.transform);

        botRed.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = PlaceholderRed.GetComponent<RectTransform>().rect.size;
        botRed.transform.GetChild(1).GetComponent<TextMeshProUGUI>().fontSize = 16 * Screen.height / Screen.width;
        botRed.transform.GetChild(1).GetComponent<RectTransform>().localPosition = new Vector2(0, PlaceholderRed.GetComponent<RectTransform>().rect.size.y / 2 + botRed.transform.GetChild(1).GetComponent<RectTransform>().localPosition.y / 2 + 2 * Screen.height / Screen.width);
        botRed.transform.GetComponent<BoxCollider2D>().size = new Vector2(botRed.transform.GetChild(0).GetComponent<RectTransform>().rect.size.y * botRed.transform.GetChild(0).GetComponent<Image>().sprite.rect.width / botRed.transform.GetChild(0).GetComponent<Image>().sprite.rect.height, botRed.transform.GetChild(0).GetComponent<RectTransform>().rect.size.y);

        StartCoroutine(botRed.GetComponent<BotRed>().SetBotRedLevel(botRedLevel));
        botRedLevel = 1;

        yield return new WaitForSeconds(1);

        if (GameManager.freezing)
        {
            yield return new WaitForSeconds(GameManager.freezingTime);
        }

        subtructPower = 0;
        for (int i = 5; i > 1; i--)
        {
            if (GameManager.redCreaturesLeft >= (int)Mathf.Pow(10, i))
            {
                if (subtructPower < i - 1)
                {
                    subtructPower = i - 1;
                    break;
                }
            }
        }

        GameManager.redCreaturesLeft -= (int)Mathf.Pow(10, subtructPower);
        if (GameManager.redCreaturesLeft < 0)
        {
            GameManager.redCreaturesLeft = 0;
        }
        transform.parent.parent.GetComponent<GameManager>().UpdateCreaturesLeftText();

        yield return new WaitForSeconds(0.1f * powerOfTen);

        if (GameManager.redCreaturesLeft < 10)
        {
            yield return new WaitForSeconds(1);
        }

        GameManager.UpdateSliderValue();

        redCreationEnabled = true;
    }

    private IEnumerator CreateBLue()
    {
        if (!PlaceholderBlue.GetComponent<Image>().enabled)
        {
            PlaceholderBlue.GetComponent<Image>().enabled = true;
        }

        GameObject botBlue = Instantiate(BotBlue, PlaceholderBlue.transform.position, BotBlue.transform.rotation, BotBlueParent.transform);

        botBlue.GetComponent<BotBlue>().currentLife = blueCreaturesToCreate[0].GetComponent<Creature>().totalLife;

        botBlue.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = PlaceholderBlue.GetComponent<RectTransform>().rect.size;
        botBlue.transform.GetChild(1).GetComponent<TextMeshProUGUI>().fontSize = 16 * Screen.height / Screen.width;
        botBlue.transform.GetChild(1).GetComponent<RectTransform>().localPosition = new Vector2(botBlue.transform.GetChild(1).GetComponent<RectTransform>().localPosition.x, PlaceholderBlue.GetComponent<RectTransform>().rect.size.y / 2 + botBlue.transform.GetChild(1).GetComponent<RectTransform>().localPosition.y / 2 + 2 * Screen.height / Screen.width);
        botBlue.transform.GetComponent<BoxCollider2D>().size = new Vector2(botBlue.transform.GetChild(0).GetComponent<RectTransform>().rect.size.y * botBlue.transform.GetChild(0).GetComponent<Image>().sprite.rect.width / botBlue.transform.GetChild(0).GetComponent<Image>().sprite.rect.height, botBlue.transform.GetChild(0).GetComponent<RectTransform>().rect.size.y);

        botBlue.transform.GetChild(0).GetChild(0).transform.localScale = botBlue.transform.GetChild(0).GetChild(0).transform.localScale * sizeRatioBlue;
        botBlue.transform.GetChild(0).position = new Vector2(botBlue.transform.GetChild(0).position.x, PlaceholderBlue.transform.position.y);
        botBlue.transform.GetChild(0).GetChild(0).transform.localPosition = new Vector2(botBlue.transform.GetChild(0).GetChild(0).transform.localPosition.x, botBlue.transform.GetChild(0).GetComponent<RectTransform>().rect.y);

        StartCoroutine(botBlue.GetComponent<BotBlue>().SetBotBlueLevel());

        if (blueCreaturesToCreate.Count > 1)
        {
            Destroy(blueCreaturesToCreate[0]);
        }


        yield return new WaitForSeconds(blueCreaturesAppearInterval);

        if (GameManager.freezing)
        {
            yield return new WaitForSeconds(GameManager.freezingTime);
        }

        blueCreaturesTotal--;
        if (blueCreaturesTotal < 0)
        {
            blueCreaturesTotal = 0;
        }
        blueCreaturesTotalText.text = blueCreaturesTotal.ToString();


        if (blueCreaturesTotal <= 0)
        {
            if (portalsEnabled)
            {
                StartCoroutine(DisablePortals());
            }
        }

        GameManager.UpdateSliderValue();

        blueCreationEnabled = true;
    }

    private void CreateShooterBots()
    {
        shooterBotVerticalPos = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)) - Camera.main.ScreenToWorldPoint(new Vector3(0, 40, 0));

        shooterBotsNumber = 4 - 2 * ((Levels.level - 1) % sameLevelsNumber);

        if (shooterBotsNumber > 0)
        {
            if (shooterBotsNumber % 2 == 1)
            {
                if (shooterBotsNumber / 2 % 2 == 0)
                {
                    for (int i = 0; i < shooterBotsNumber; i++)
                    {
                        GameObject shooterBot;

                        if (i % 2 == 0)
                        {
                            shooterBot = Instantiate(ShooterBot, new Vector3(screenWidth * (2 * i + 1) / (2 * shooterBotsNumber) - screenWidth / 2, ShooterBots.transform.position.y - shooterBotVerticalPos.y, ShooterBots.transform.position.z), ShooterBots.transform.rotation, ShooterBots.transform);
                        }
                        else
                        {
                            shooterBot = Instantiate(ShooterBot, new Vector3(screenWidth * (2 * i + 1) / (2 * shooterBotsNumber) - screenWidth / 2, ShooterBots.transform.position.y, ShooterBots.transform.position.z), ShooterBots.transform.rotation, ShooterBots.transform);
                        }

                        shooterBot.transform.localScale = shooterBot.transform.localScale / 2.16f * Screen.height / Screen.width;
                    }
                }

                else
                {
                    for (int i = 0; i < shooterBotsNumber; i++)
                    {
                        GameObject shooterBot;

                        if (i % 2 == 1)
                        {
                            shooterBot = Instantiate(ShooterBot, new Vector3(screenWidth * (2 * i + 1) / (2 * shooterBotsNumber) - screenWidth / 2, ShooterBots.transform.position.y - shooterBotVerticalPos.y, ShooterBots.transform.position.z), ShooterBots.transform.rotation, ShooterBots.transform);
                        }
                        else
                        {
                            shooterBot = Instantiate(ShooterBot, new Vector3(screenWidth * (2 * i + 1) / (2 * shooterBotsNumber) - screenWidth / 2, ShooterBots.transform.position.y, ShooterBots.transform.position.z), ShooterBots.transform.rotation, ShooterBots.transform);
                        }

                        shooterBot.transform.localScale = shooterBot.transform.localScale / 2.16f * Screen.height / Screen.width;
                    }
                }
            }
            else
            {
                for (int i = 0; i < shooterBotsNumber; i++)
                {
                    GameObject shooterBot;

                    if (i == 0 || (i + 1) % shooterBotsNumber == 0)
                    {
                        shooterBot = Instantiate(ShooterBot, new Vector3(screenWidth * (2 * i + 1) / (2 * shooterBotsNumber) - screenWidth / 2, ShooterBots.transform.position.y, ShooterBots.transform.position.z), ShooterBots.transform.rotation, ShooterBots.transform);
                    }
                    else
                    {
                        shooterBot = Instantiate(ShooterBot, new Vector3(screenWidth * (2 * i + 1) / (2 * shooterBotsNumber) - screenWidth / 2, ShooterBots.transform.position.y - shooterBotVerticalPos.y, ShooterBots.transform.position.z), ShooterBots.transform.rotation, ShooterBots.transform);
                    }

                    shooterBot.transform.localScale = shooterBot.transform.localScale / 2.16f * Screen.height / Screen.width;
                }
            }
        }
    }

    private void CreatePortals()
    {
        movePortals = false;
        portalsEnabled = false;

        levelRow = (int)Mathf.Floor((Levels.level - 1) / sameLevelsNumber + 1);
        portalsSpeed = 0.0004f * levelRow;

        if (levelRow <= 4)
        {
            offset = 0;
        }
        else
        {
            offset = levelRow - 4;
        }

        Portals.transform.localScale = Portals.transform.localScale * Screen.height / (2.16f * Screen.width);
        Portals.transform.localPosition = new Vector2(0, -65 * Screen.height / Screen.width);

        for (int i = 0; i < levelRow - offset; i++)
        {
            GameObject portal = Instantiate(PortalsList[(i + offset) % 16], new Vector3(screenWidth * (i + 1) / (levelRow + 1 - offset) - screenWidth / 2, Portals.transform.position.y, Portals.transform.position.z), Portals.transform.rotation, Portals.transform);
            activePortals.Add(portal);
        }
    }

    private static void EnablePortals()
    {
        if (!portalsEnabled)
        {
            portalsEnabled = true;

            foreach (GameObject portal in activePortals)
            {
                portal.transform.GetChild(2).gameObject.SetActive(false);
                portal.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    public static IEnumerator DisablePortals()
    {
        yield return new WaitForSeconds(2);

        if (portalsEnabled)
        {
            portalsEnabled = false;

            foreach (GameObject portal in activePortals)
            {
                portal.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    private int GetRandomCreatureIndex()
    {
        int creatureIndex = offset;

        float random = Random.value;

        if (levelRow > 3)
        {
            if (random >= 0.85f)
            {
                creatureIndex = offset + 3;
            }
            else if (random >= 0.65f)
            {
                creatureIndex = offset + 2;
            }
            else if (random >= 0.35f)
            {
                creatureIndex = offset + 1;
            }
        }

        else if (levelRow == 3)
        {
            if (random >= 0.75f)
            {
                creatureIndex = offset + 2;
            }
            else if (random >= 0.4f)
            {
                creatureIndex = offset + 1;
            }
        }

        else if (levelRow == 2)
        {
            if (random >= 0.7f)
            {
                creatureIndex = offset + 1;
            }
        }

        return creatureIndex;
    }
}
