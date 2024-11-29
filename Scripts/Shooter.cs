using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject Ball;
    [SerializeField] private GameObject Balls;
    [SerializeField] private GameObject FirstBallSprite;
    [SerializeField] private GameObject TopWall;
    [SerializeField] private GameObject GameOverTrigger;
    [SerializeField] private GameObject ShooterBotsZone;
    [SerializeField] private TextMeshProUGUI CurrentBallCountText;
    [SerializeField] private Slider Slider;
    [SerializeField] private Slider BallsSlider;
    [SerializeField] private Transform LeftClawMidTransform;
    [SerializeField] private Button ResetBallsButton;

    private static bool stopShooting;
    private static List<GameObject> ballInstancesList = new();

    public static bool shooterRotationUp;
    public static bool shooting;
    public static int totalBallCount;
    public static TextMeshProUGUI CurrentBallCountTextStatic;
    public static GameObject FirstBallSpriteStatic;
    public static Slider BallsSliderStatic;

    private bool sliderIsPressed;
    private float force = 600;
    private float angle;
    private float angleMin = 10;
    private float angleMax = 170;
    private int currentBallCount;

    public static Shooter Instance;


    private void Start()
    {
        shooting = false;
        sliderIsPressed = false;
        shooterRotationUp = false;

        FirstBallSpriteStatic = FirstBallSprite;
        CurrentBallCountTextStatic = CurrentBallCountText;
        BallsSliderStatic = BallsSlider;

        totalBallCount = PlayerPrefs.GetInt("Balls", 10);
        CurrentBallCountTextStatic.text = totalBallCount.ToString();

        BallsSliderStatic.maxValue = totalBallCount;
        BallsSliderStatic.value = totalBallCount;

        TopWall.GetComponent<BoxCollider2D>().size = TopWall.GetComponent<RectTransform>().rect.size;
        TopWall.GetComponent<BoxCollider2D>().offset = new Vector2(0, -TopWall.GetComponent<RectTransform>().rect.size.y / 2);

        transform.parent.localScale = transform.parent.localScale / 2.16f * Screen.height / Screen.width;
        transform.parent.localPosition = new Vector2(0, 0.16f * StartUI.canvasHeight);

        Instance = this;


        if (ResetBallsButton != null)
        {
            ResetBallsButton.onClick.AddListener(ResetBalls);
        }
    }

    private void FixedUpdate()
    {
        if (!shooterRotationUp && Balls.transform.childCount == 0 && !sliderIsPressed)
        {
            LeftClawMidTransform.rotation = Quaternion.RotateTowards(LeftClawMidTransform.rotation, Quaternion.AngleAxis(90, Vector3.forward), 100 * Time.deltaTime);
            Slider.value = 180 - LeftClawMidTransform.rotation.eulerAngles.z;

            if (Quaternion.Angle(LeftClawMidTransform.rotation, Quaternion.AngleAxis(90, Vector3.forward)) < 0.01f)
            {
                shooterRotationUp = true;
            }
        }

        if (Input.GetMouseButton(0) || sliderIsPressed)
        {
            if (GameManager.hookEnabled || !LevelStart.touched || ShooterBotsZone.activeSelf || Input.GetMouseButton(0) && !sliderIsPressed && (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > TopWall.transform.position.y || Camera.main.ScreenToWorldPoint(Input.mousePosition).y < GameOverTrigger.transform.position.y))
            {
                return;
            }

            if (sliderIsPressed)
            {
                angle = 180 - Slider.value;
            }
            else
            {
                Vector3 direction = Input.mousePosition - Camera.main.WorldToScreenPoint(LeftClawMidTransform.position);
                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            }

            if (angle > angleMin && angle < angleMax)
            {
                LeftClawMidTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                if (!sliderIsPressed)
                {
                    Slider.value = 180 - angle;
                }
            }
            else
            {
                if (angle <= angleMin && angle >= -90 && !Input.GetMouseButtonDown(0))
                {
                    LeftClawMidTransform.rotation = Quaternion.AngleAxis(angleMin, Vector3.forward);

                    if (!sliderIsPressed)
                    {
                        Slider.value = 180 - angle;
                    }
                }
                else if ((angleMax <= angle || (angle <= -90 && angle >= -180)) && !Input.GetMouseButtonDown(0))
                {
                    LeftClawMidTransform.rotation = Quaternion.AngleAxis(angleMax, Vector3.forward);

                    if (!sliderIsPressed)
                    {
                        if (angleMax <= angle)
                        {
                            Slider.value = 180 - angle;
                        }
                        else if (angle <= -90 && angle >= -180)
                        {
                            Slider.value = 0;
                        }
                    }
                }
            }


            if (!shooting)
            {
                shooting = true;
                StartCoroutine(ShootBall());
            }
        }


        if (!Input.GetMouseButton(0))
        {
            stopShooting = true;
        }
    }


    private IEnumerator ShootBall()
    {
        currentBallCount = totalBallCount;

        if (currentBallCount == 0)
        {
            shooting = false;
            FirstBallSpriteStatic.SetActive(false);
        }

        else
        {
            stopShooting = false;

            for (int i = 0; i < currentBallCount; i++)
            {
                if (stopShooting)
                {
                    break;
                }

                yield return new WaitForFixedUpdate();

                GameObject ballInstance = Instantiate(Ball, FirstBallSpriteStatic.transform.position, Quaternion.identity, Balls.transform);
                ballInstancesList.Add(ballInstance);

                if (GameManager.powerBalls)
                {
                    ballInstance.GetComponent<Ball>().ballPower = 4;
                    ballInstance.GetComponent<Image>().color = Color.blue;
                }

                ballInstance.GetComponent<Rigidbody2D>().AddForce(LeftClawMidTransform.right * force);

                totalBallCount--;
                CurrentBallCountTextStatic.text = totalBallCount.ToString();
                BallsSliderStatic.value = totalBallCount;


                shooterRotationUp = false;

                if (totalBallCount <= 0)
                {
                    FirstBallSpriteStatic.SetActive(false);
                }
            }

            shooting = false;
        }
    }

    public void ResetBalls()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        stopShooting = true;
        shooting = false;

        foreach (GameObject ballInstance in ballInstancesList)
        {
            if (ballInstance != null)
            {
                ballInstance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                ballInstance.GetComponent<Ball>().returnBall = true;
            }
        }

        for (int i = 1; i < ShooterBotsZone.transform.parent.childCount; i++)
        {
            for (int j = 0; j < ShooterBotsZone.transform.parent.GetChild(i).GetChild(0).childCount; j++)
            {
                ShooterBotsZone.transform.parent.GetChild(i).GetChild(0).GetChild(j).GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                ShooterBotsZone.transform.parent.GetChild(i).GetChild(0).GetChild(j).GetComponent<BallBot>().returnBall = true;
            }
        }

        ballInstancesList.Clear();
    }

    public void SetSliderIsPressed(bool isPressed)
    {
        if (!GameManager.hookEnabled)
        {
            sliderIsPressed = isPressed;
        }
    }
}
