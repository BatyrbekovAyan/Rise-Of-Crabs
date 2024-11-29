using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private GameObject Clapperboard;
    [SerializeField] private GameObject NoInternet;
    [SerializeField] private Button GameOverContinueButton;
    [SerializeField] private Button GameOverRetryButton;

    public static GameOver Instatnce;


    void Start()
    {
        Instatnce = this;

        gameObject.GetComponent<BoxCollider2D>().size = gameObject.GetComponent<RectTransform>().rect.size;
        gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0, gameObject.GetComponent<RectTransform>().rect.size.y / 2);

        if (GameOverContinueButton != null)
        {
            GameOverContinueButton.onClick.AddListener(GameOverContinue);
        }

        if (GameOverRetryButton != null)
        {
            GameOverRetryButton.onClick.AddListener(GameOverRetry);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Target"))
        {
            ShowGameOverPanel();
        }
    }

    public void ShowGameOverPanel()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Lose.Play();
        }

        if (StartUI.adsOn && Creatures.levelRow > 60)
        {
            AdsManager.Instance.bannerAds.HideBannerAd();
        }

        foreach (GameObject creature in Creatures.creaturesBodiesList)
        {
            if (creature.transform.childCount > 2)
            {
                creature.transform.GetChild(2).GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);
            }
            else if (creature.transform.childCount > 1)
            {
                creature.transform.GetChild(1).GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);
            }
        }

        GameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }

    private void GameOverContinue()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        if (StartUI.internetConnected)
        {
            AdsManager.Instance.rewardedAds.ShowRewardedAd();
        }
        else
        {
            NoInternet.SetActive(true);

            DiactivateNoInternetPanel();
        }
    }

    public void Continue()
    {
        foreach (GameObject creature in Creatures.creaturesBodiesList)
        {
            if (creature != null)
            {
                if (creature.transform.parent.GetComponent<Creature>().tooClose && creature.transform.CompareTag("Target"))
                {
                    Destroy(creature);
                }
                else
                {
                    if (creature.transform.childCount > 2)
                    {
                        creature.transform.GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1);
                    }
                    else if (creature.transform.childCount > 1)
                    {
                        creature.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1);
                    }
                }
            }
        }

        if (GameManager.redHouseAttacked)
        {
            if (Creatures.botBlueList.Count > 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    Destroy(Creatures.botBlueList[i]);
                }

                Creatures.botBlueList.RemoveRange(0, 4);
            }
            else
            {
                for (int i = 0; i < Creatures.botBlueList.Count; i++)
                {
                    Destroy(Creatures.botBlueList[i]);
                }

                Creatures.botBlueList.RemoveRange(0, Creatures.botBlueList.Count);
            }

            GameManager.redHouseAttacked = false;
        }

        GameOverContinueButton.interactable = false;

        Shooter.Instance.ResetBalls();
        GameOverPanel.SetActive(false);

        if (StartUI.adsOn && Creatures.levelRow > 60)
        {
            AdsManager.Instance.bannerAds.ShowBannerAd();
        }

        Time.timeScale = 1;
    }

    public void DiisableContinue()
    {
        GameOverContinueButton.interactable = false;
    }

    private void GameOverRetry()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        Time.timeScale = 1;

        SceneManager.LoadScene("Main");
    }

    private async void DiactivateNoInternetPanel()
    {
        await Task.Delay(2000);

        NoInternet.SetActive(false);
    }
}
