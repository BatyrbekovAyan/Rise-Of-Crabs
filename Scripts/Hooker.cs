using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hooker : MonoBehaviour
{
    [SerializeField] private GameObject TopWall;
    [SerializeField] private GameObject LeftWall;
    [SerializeField] private GameObject RightWall;
    [SerializeField] private GameObject BottomWall;
    [SerializeField] private GameObject GameOverTrigger;
    [SerializeField] private GameObject HookedCreaturesParent;
    [SerializeField] private GameObject ShooterBotsZone;
    [SerializeField] private GameObject HooksPanel;
    [SerializeField] private GameObject BackgroundTimer;
    [SerializeField] private Transform RightClawMid;
    [SerializeField] private Transform CrabMouth;
    [SerializeField] private TextMeshProUGUI HooksNumberText;
    [SerializeField] private TextMeshProUGUI HooksTimerText;
    [SerializeField] private Slider Slider;
    [SerializeField] private Slider HooksSlider;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask LayerIgnoreRaycast;

    private float maxDistance;
    private float hookSpeed ;
    private float hookShootSpeed;
    private float clawLength;
    private float angle;
    private int linesIndex;
    private int LayerIgnoreRaycastIndex;
    private int wallLayerIndex;
    private bool pulling;
    private bool creatureTouched;
    private bool clawBounced;
    private bool creatureEaten;
    private bool hookerRotationUp;
    private bool sliderIsPressed;
    private Vector2 target;
    private Vector2 direction;
    private Vector2 newDirection;
    private Vector2 clawSegmentLength;
    private Vector3 clawLastPosition;
    private Quaternion clawLastRotation;
    private GameObject lastHitWall;
    private Transform hookedCreature;
    private LineRenderer line;
    private RaycastHit2D hit;

    private bool isRestoringHooks;
    private int maxHooksNumber;
    private int restoreDuration;
    private DateTime nextHookTime;
    private DateTime lastHookTime;

    public static bool isHooking;
    public static bool switchedToShooter;
    public static int hooksNumber;

    public static Hooker Instance;


    private void Start()
    {
        maxDistance = 50f;
        hookSpeed = 10;
        hookShootSpeed = 15f;
        clawLength = 10;
        linesIndex = 1;
        maxHooksNumber = 100;
        restoreDuration = 2;
        isRestoringHooks = false;
        isHooking = false;
        pulling = false;
        creatureTouched = false;
        clawBounced = false;
        switchedToShooter = false;
        creatureEaten = false;
        hookerRotationUp = false;
        sliderIsPressed = false;
        clawLastPosition = transform.parent.position;
        LayerIgnoreRaycastIndex = LayerMask.NameToLayer("Ignore Raycast");
        wallLayerIndex = LayerMask.NameToLayer("Wall");
        lastHitWall = TopWall;
        line = GetComponent<LineRenderer>();

        hooksNumber = PlayerPrefs.GetInt("Hooks", 100);
        nextHookTime = StringToDate(PlayerPrefs.GetString("NextHookTime"));
        lastHookTime = StringToDate(PlayerPrefs.GetString("LastHookTime"));

        Instance = this;
        StartCoroutine(RestoreHooks());
        CheckFreeHooks();
    }

    private void FixedUpdate()
    {
        if (!hookerRotationUp && !sliderIsPressed)
        {
            RightClawMid.rotation = Quaternion.RotateTowards(RightClawMid.rotation, Quaternion.AngleAxis(90, Vector3.forward), 100 * Time.deltaTime);
            Slider.value = 180 - RightClawMid.rotation.eulerAngles.z;

            if (Quaternion.Angle(RightClawMid.rotation, Quaternion.AngleAxis(90, Vector3.forward)) < 0.01f)
            {
                hookerRotationUp = true;
            }
        }

        if ((Input.GetMouseButton(0) || sliderIsPressed) && (!isHooking || creatureEaten && !switchedToShooter))
        {
            if (!GameManager.hookEnabled || !LevelStart.touched || HooksPanel.activeSelf || ShooterBotsZone.activeSelf || Input.GetMouseButton(0) && !sliderIsPressed && (!isHooking || creatureEaten) && (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > TopWall.transform.position.y || Camera.main.ScreenToWorldPoint(Input.mousePosition).y < GameOverTrigger.transform.position.y))
            {
                return;
            }

            if (hooksNumber < 1)
            {
                GameManager.Instance.OpenHooksPanel();
                return;
            }

            hooksNumber--;
            UpdateTimerText();

            if (!isRestoringHooks)
            {
                if (hooksNumber == maxHooksNumber - 1)
                {
                    nextHookTime = DateTime.Now.AddMinutes(restoreDuration);
                }

                StartCoroutine(RestoreHooks());
            }

            isHooking = true;
            hookerRotationUp = true;
            creatureEaten = false;
            pulling = false;

            if (sliderIsPressed)
            {
                angle = 180 - Slider.value;

                direction = new Vector2(1, Mathf.Tan(angle * Mathf.Deg2Rad));

                if (angle > 90)
                {
                    direction = -direction;
                }
            }
            else
            {
                direction = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                Slider.value = 180 - angle;
            }

            RightClawMid.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            hit = Physics2D.Raycast(transform.position, direction, maxDistance, wallLayer);
            target = hit.point;
            lastHitWall = hit.collider.gameObject;

            line.enabled = true;
            line.SetPosition(0, transform.parent.position);
            line.SetPosition(1, transform.parent.position);

            clawLastPosition = transform.parent.position;
        }

        else if (isHooking && !pulling)
        {
            clawSegmentLength = transform.position - clawLastPosition;

            if (Mathf.Sqrt(clawSegmentLength.x * clawSegmentLength.x + clawSegmentLength.y * clawSegmentLength.y) < clawLength)
            {
                transform.position = Vector2.MoveTowards(transform.position, target, hookShootSpeed * Time.deltaTime);

                line.SetPosition(linesIndex, transform.position);
            }
            else
            {
                pulling = true;
            }


            if (Vector2.Distance(target, transform.position) < 0.01f)
            {
                if (clawBounced)
                {
                    pulling = true;

                    return;
                }

                clawLastRotation = transform.localRotation;
                clawBounced = true;

                if (lastHitWall.name.Equals("TopWall"))
                {
                    newDirection = Vector2.Reflect(direction, Vector2.up);

                    angle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;
                    transform.localRotation = Quaternion.AngleAxis(2 * angle, Vector3.forward);
                }
                else if (lastHitWall.name.Equals("LeftWall"))
                {
                    newDirection = Vector2.Reflect(direction, Vector2.left);

                    angle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;
                    transform.localRotation = Quaternion.AngleAxis(2 * (angle + 90), Vector3.forward);
                }
                else if (lastHitWall.name.Equals("RightWall"))
                {
                    newDirection = Vector2.Reflect(direction, Vector2.left);

                    angle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;
                    transform.localRotation = Quaternion.AngleAxis(2 * (angle - 90), Vector3.forward);
                }

                line.SetPosition(linesIndex, transform.position);
                linesIndex++;
                line.positionCount = linesIndex + 1;
                line.SetPosition(linesIndex, transform.position);

                clawSegmentLength = transform.position - transform.parent.position;
                clawLength -= Mathf.Sqrt(clawSegmentLength.x * clawSegmentLength.x + clawSegmentLength.y * clawSegmentLength.y);
                clawLastPosition = transform.position;


                lastHitWall.layer = LayerIgnoreRaycastIndex;

                hit = Physics2D.Raycast(transform.position, newDirection, maxDistance, wallLayer);

                if (hit.collider != null)
                {
                    target = hit.point;
                }
                else
                {
                    target = line.GetPosition(0);
                }

                lastHitWall.layer = wallLayerIndex;
            }
        }

        else if (pulling)
        {
            transform.position = Vector2.MoveTowards(transform.position, clawLastPosition, hookSpeed * Time.deltaTime);

            line.SetPosition(linesIndex, transform.position);

            if (hookedCreature != null)
            {
                hookedCreature.position = transform.GetChild(0).position;

                if (clawLastPosition == transform.parent.position && !creatureEaten)
                {
                    clawLastPosition = CrabMouth.position;
                }
            }

            if (Vector2.Distance(clawLastPosition, transform.position) < 0.01f)
            {
                if (clawLastPosition == transform.parent.position)
                {
                    pulling = false;
                    isHooking = false;
                    line.enabled = false;
                    clawBounced = false;
                    hookerRotationUp = false;
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                    clawLength = 10;
                    line.SetPosition(1, transform.parent.position);

                    if (switchedToShooter)
                    {
                        switchedToShooter = false;
                        GameManager.hookEnabled = false;
                    }
                }

                else if (clawLastPosition == CrabMouth.position && !creatureEaten)
                {
                    creatureTouched = false;
                    clawBounced = false;
                    gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                    clawLength = 10;
                    creatureEaten = true;

                    clawLastPosition = transform.parent.position;

                    if (hookedCreature != null)
                    {
                        StartCoroutine(hookedCreature.GetComponent<ICreatureBody>().Hooked());
                    }
                }

                else
                {
                    if (hookedCreature != null)
                    {
                        clawLastPosition = CrabMouth.position;
                    }
                    else
                    {
                        clawLastPosition = transform.parent.position;
                    }

                    transform.localRotation = clawLastRotation;

                    linesIndex = 1;
                    line.positionCount = 2;
                    line.SetPosition(1, transform.position);
                }
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Target"))
        {
            if (!creatureTouched)
            {
                creatureTouched = true;
                PullCreature(collision);
            }
        }
    }

    private void PullCreature(Collider2D collision)
    {
        hookedCreature = collision.transform;
        hookedCreature.tag = "HookedCreature";

        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        hookedCreature.GetComponent<Collider2D>().enabled = false;

        hookedCreature.GetComponent<Canvas>().overrideSorting = true;
        hookedCreature.GetComponent<Canvas>().sortingOrder = 7;

        hookedCreature.GetComponent<ICreatureBody>().DisableScript();

        hookedCreature.parent.SetParent(HookedCreaturesParent.transform);

        pulling = true;
    }

    public void SetSliderIsPressed(bool isPressed)
    {
        if (GameManager.hookEnabled)
        {
            sliderIsPressed = isPressed;
        }
    }

    private IEnumerator RestoreHooks()
    {
        HooksNumberText.text = hooksNumber.ToString();
        HooksSlider.value = hooksNumber;

        isRestoringHooks = true;

        while (hooksNumber < maxHooksNumber)
        {
            DateTime newNextHookTime = nextHookTime;
            bool isHookAdding = false;

            while (newNextHookTime < DateTime.Now)
            {
                if (hooksNumber < maxHooksNumber)
                {
                    isHookAdding = true;
                    hooksNumber++;
                    UpdateTimerText();
                    DateTime timeToAdd = lastHookTime > newNextHookTime ? lastHookTime : newNextHookTime;
                    newNextHookTime = timeToAdd.AddMinutes(restoreDuration);
                }

                else
                {
                    break;
                }
            }

            if (isHookAdding)
            {
                lastHookTime = DateTime.Now;
                nextHookTime = newNextHookTime;
            }

            HooksNumberText.text = hooksNumber.ToString();
            HooksSlider.value = hooksNumber;

            UpdateTimerText();

            PlayerPrefs.SetInt("Hooks", hooksNumber);
            PlayerPrefs.SetString("NextHookTime", nextHookTime.ToString());
            PlayerPrefs.SetString("LastHookTime", lastHookTime.ToString());

            yield return null;
        }

        isRestoringHooks = false;
    }

    public void UpdateTimerText()
    {
        if (hooksNumber >= maxHooksNumber)
        {
            HooksTimerText.text = "";
            BackgroundTimer.SetActive(false);
            return;
        }

        TimeSpan time = nextHookTime - DateTime.Now;

        string timeValue ;

        if (PlayerPrefs.GetInt("Locale", 0) == 0)
        {
            if (time.Minutes > 0)
            {
                timeValue = string.Format("{0:D1}", time.Minutes) + "m " + string.Format("{0:D1}", time.Seconds) + "s";
            }
            else
            {
                timeValue = string.Format("{0:D1}", time.Seconds) + "s";
            }
        }
        else
        {
            if (time.Minutes > 0)
            {
                timeValue = string.Format("{0:D1}", time.Minutes) + ":" + string.Format("{0:D1}", time.Seconds);
            }
            else
            {
                timeValue = string.Format("{0:D1}", time.Seconds);
            }
        }


        HooksTimerText.text = timeValue;

        if (!BackgroundTimer.activeSelf)
        {
            BackgroundTimer.SetActive(true);
        }
    }

    private DateTime StringToDate(string dateTime)
    {
        if (string.IsNullOrEmpty(dateTime))
        {
            return DateTime.Now;
        }
        else
        {
            return DateTime.Parse(dateTime);
        }
    }

    private void CheckFreeHooks()
    {
        if (DateTime.Compare(StringToDate(PlayerPrefs.GetString("LastFreeHooksDate", "08/10/2024 00:00:00")).Date, DateTime.Now.Date) < 0 && DateTime.Compare(DateTime.Today.AddHours(14), DateTime.Now) < 0)
        {
            PlayerPrefs.SetString("LastFreeHooksDate", DateTime.Now.Date.ToString());
            PlayerPrefs.SetInt("AdsForHookShown", 0);
        }
    }
}