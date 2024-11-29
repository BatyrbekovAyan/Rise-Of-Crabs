using System.Collections;
using UnityEngine;
using TMPro;

public class ShooterBot : MonoBehaviour
{
    [SerializeField] private GameObject BallBot;
    [SerializeField] private GameObject Balls;
    [SerializeField] private GameObject FirstBallSprite;
    [SerializeField] private TextMeshProUGUI CurrentBallCountText;

    private GameObject chosenCreature;

    private bool shooting;
    private bool randomCreatureChosen;
    private bool shootingIntervalPast;
    private bool shooterEnabled;
    private bool enabledOnce;

    private float shootingIntervalTime;
    private float force = 300;
    private float ballsIntervalTime;
    private float angle;
    private int currentBallCount;
    private int totalBallCount;

    private void Start()
    {
        shooting = false;
        shootingIntervalPast = true;
        randomCreatureChosen = false;
        shooterEnabled = false;
        enabledOnce = false;

        shootingIntervalTime = 5;
        totalBallCount = 5;
        CurrentBallCountText.text = 0 + "x";
    }

    private void FixedUpdate()
    {
        if (!enabledOnce && LevelStart.touched)
        {
            enabledOnce = true;
            StartCoroutine(EnableShooter());
        }

        if (Creatures.creaturesBodiesList.Count > 0)
        {
            if (!randomCreatureChosen)
            {
                chosenCreature = Creatures.creaturesBodiesList[Random.Range(0, Creatures.creaturesBodiesList.Count)];
                randomCreatureChosen = true;

                if (shootingIntervalPast)
                {
                    shooting = false;
                }
            }
            else
            {
                if (chosenCreature != null)
                {
                    Vector3 pos = Camera.main.WorldToScreenPoint(FirstBallSprite.transform.position);
                    Vector3 dir = Camera.main.WorldToScreenPoint(chosenCreature.transform.position) - pos;
                    angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                }
                else
                {
                    randomCreatureChosen = false;
                }
            }
        }
        else
        {
            angle = 90;
            shooting = true;
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), 20 * Time.deltaTime);

        if (!shooting && shooterEnabled)
        {
            shooting = true;
            StartCoroutine(ShootBall());
        }
    }


    private IEnumerator ShootBall()
    {
        if (Sound.SoundEnabled)
        {
            Sound.BotShoot.Play();
        }

        shootingIntervalPast = false;

        currentBallCount = totalBallCount;

        for (int i = 0; i < currentBallCount; i++)
        {
            yield return new WaitForFixedUpdate();

            GameObject ballBotInstance = Instantiate(BallBot, FirstBallSprite.transform.position, Quaternion.identity, Balls.transform);
            ballBotInstance.GetComponent<BallBot>().FirstBallSprite = FirstBallSprite;
            ballBotInstance.GetComponent<Rigidbody2D>().AddForce(transform.right * force);

            totalBallCount--;
            CurrentBallCountText.text = totalBallCount + "x";

            if (totalBallCount <= 0)
            {
                FirstBallSprite.SetActive(false);
            }
        }

        yield return new WaitForSeconds(0.5f);

        randomCreatureChosen = false;

        for (; totalBallCount < currentBallCount; totalBallCount++)
        {
            ballsIntervalTime = shootingIntervalTime / currentBallCount;
            CurrentBallCountText.text = totalBallCount + 1 + "x";

            if (!FirstBallSprite.activeSelf)
            {
                FirstBallSprite.SetActive(true);
            }

            yield return new WaitForSeconds(ballsIntervalTime);
        }

        shooting = false;
        shootingIntervalPast = true;
    }

    private IEnumerator EnableShooter()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < totalBallCount; i++)
        {
            ballsIntervalTime = shootingIntervalTime / totalBallCount;
            CurrentBallCountText.text = i + 1 + "x";

            if (!FirstBallSprite.activeSelf)
            {
                FirstBallSprite.SetActive(true);
            }

            yield return new WaitForSeconds(ballsIntervalTime);
        }

        shooterEnabled = true;
    }
}
