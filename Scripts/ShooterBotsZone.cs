using UnityEngine;
using UnityEngine.UI;

public class ShooterBotsZone : MonoBehaviour
{
    [SerializeField] private GameObject ShooterBot;

    private bool shooterBotCreated;
    private float canvasCorrection;
    private float deltaDistanceX;
    private float deltaDistanceY;
    private Vector3 zonePosition;
    private GameObject selectedShooterBot;


    private void Start()
    { 
        shooterBotCreated = false;
        selectedShooterBot = null;
        canvasCorrection = 1200f / Screen.width;
        zonePosition = Camera.main.WorldToScreenPoint(GetComponent<RectTransform>().position);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.07f * StartUI.canvasHeight);
        GetComponent<BoxCollider2D>().size = GetComponent<RectTransform>().rect.size;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D rayToShooterBots = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

            if (rayToShooterBots.collider && rayToShooterBots.collider.CompareTag("ShooterBot"))
            {
                selectedShooterBot = rayToShooterBots.collider.gameObject;

                selectedShooterBot.transform.GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
                foreach (Transform bodyPart in selectedShooterBot.transform.GetChild(1).GetChild(0).GetChild(0))
                {
                    bodyPart.gameObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
                }

                deltaDistanceX = Input.mousePosition.x - Camera.main.WorldToScreenPoint(selectedShooterBot.transform.position).x;
                deltaDistanceY = Input.mousePosition.y - Camera.main.WorldToScreenPoint(selectedShooterBot.transform.position).y;
            }
            else
            {
                GetComponent<BoxCollider2D>().enabled = true;

                RaycastHit2D rayToZone = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

                if (rayToZone.collider && rayToZone.collider.name.Equals(name))
                {
                    if (PlayerPrefs.GetInt("Rubys", 0) >= 2)
                    {
                        if (Sound.SoundEnabled)
                        {
                            Sound.ShooterBotPlace.Play();
                        }

                        Vector3 botPlacePosition = new(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
                        GameObject shooterBot = Instantiate(ShooterBot, botPlacePosition, ShooterBot.transform.rotation, transform.parent);
                        shooterBot.transform.localScale = shooterBot.transform.localScale / 2.16f * Screen.height / Screen.width;

                        selectedShooterBot = shooterBot;

                        selectedShooterBot.transform.GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
                        foreach (Transform bodyPart in selectedShooterBot.transform.GetChild(1).GetChild(0).GetChild(0))
                        {
                            bodyPart.gameObject.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
                        }

                        deltaDistanceX = 0;
                        deltaDistanceY = 0;

                        PlayerPrefs.SetInt("Rubys", PlayerPrefs.GetInt("Rubys", 0) - 2);
                        GameManager.RubysTextStatic.text = PlayerPrefs.GetInt("Rubys", 0).ToString();
                    }
                    else
                    {
                        GameManager.Instance.OpenShop();
                    }
                }

                shooterBotCreated = true;
            }
        }

        else if (Input.GetMouseButtonUp(0))
        {
            GetComponent<BoxCollider2D>().enabled = false;

            if (selectedShooterBot != null)
            {
                selectedShooterBot.transform.GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                foreach (Transform bodyPart in selectedShooterBot.transform.GetChild(1).GetChild(0).GetChild(0))
                {
                    bodyPart.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                }

                selectedShooterBot = null;
            }


            if (shooterBotCreated)
            {
                shooterBotCreated = false;
                gameObject.SetActive(false);
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (selectedShooterBot != null)
            {
                if (Input.mousePosition.y - deltaDistanceY > zonePosition.y + 0.5f * GetComponent<RectTransform>().rect.height / canvasCorrection)
                {
                    if (Input.mousePosition.x - deltaDistanceX > Screen.width)
                    {
                        selectedShooterBot.transform.localPosition = new Vector3(canvasCorrection * Screen.width - 600, 0.5f * GetComponent<RectTransform>().rect.height);
                    }
                    else if (Input.mousePosition.x - deltaDistanceX < 0)
                    {
                        selectedShooterBot.transform.localPosition = new Vector3(-600, 0.5f * GetComponent<RectTransform>().rect.height);
                    }
                    else
                    {
                        selectedShooterBot.transform.localPosition = new Vector3(canvasCorrection * (Input.mousePosition.x - deltaDistanceX) - 600, 0.5f * GetComponent<RectTransform>().rect.height);
                    }
                }
                else if (Input.mousePosition.y - deltaDistanceY < zonePosition.y - 0.5f * GetComponent<RectTransform>().rect.height / canvasCorrection)
                {
                    if (Input.mousePosition.x - deltaDistanceX > Screen.width)
                    {
                        selectedShooterBot.transform.localPosition = new Vector3(canvasCorrection * Screen.width - 600, -0.5f * GetComponent<RectTransform>().rect.height);
                    }
                    else if (Input.mousePosition.x - deltaDistanceX < 0)
                    {
                        selectedShooterBot.transform.localPosition = new Vector3(-600, -0.5f * GetComponent<RectTransform>().rect.height);
                    }
                    else
                    {
                        selectedShooterBot.transform.localPosition = new Vector3(canvasCorrection * (Input.mousePosition.x - deltaDistanceX) - 600, -0.5f * GetComponent<RectTransform>().rect.height);
                    }
                }
                else if (Input.mousePosition.x - deltaDistanceX > Screen.width)
                {
                    selectedShooterBot.transform.localPosition = new Vector3(0.5f * GetComponent<RectTransform>().rect.width, canvasCorrection * (Input.mousePosition.y - deltaDistanceY - zonePosition.y));
                }
                else if (Input.mousePosition.x - deltaDistanceX < 0)
                {
                    selectedShooterBot.transform.localPosition = new Vector3(-0.5f * GetComponent<RectTransform>().rect.width, canvasCorrection * (Input.mousePosition.y - deltaDistanceY - zonePosition.y));
                }
                else
                {
                    selectedShooterBot.transform.localPosition = new Vector3(canvasCorrection * (Input.mousePosition.x - deltaDistanceX) - 600, canvasCorrection * (Input.mousePosition.y - deltaDistanceY - zonePosition.y));
                }
            }
        }
    }
}
