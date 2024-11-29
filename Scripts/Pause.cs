using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField] private Button PauseContinueButton;
    [SerializeField] private Button PauseMenuButton;
    [SerializeField] private Button PauseRetryButton;


    void Start()
    {
        if (PauseContinueButton != null)
        {
            PauseContinueButton.onClick.AddListener(PauseContinue);
        }

        if (PauseMenuButton != null)
        {
            PauseMenuButton.onClick.AddListener(PauseQuit);
        }

        if (PauseRetryButton != null)
        {
            PauseRetryButton.onClick.AddListener(PauseRetry);
        }
    }

    private void OnEnable()
    {
        if (StartUI.adsOn && Creatures.levelRow > 60)
        {
            AdsManager.Instance.bannerAds.HideBannerAd();
        }

        foreach (GameObject creature in Creatures.creaturesBodiesList)
        {
            if (creature != null)
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
        }

        Time.timeScale = 0;
    }

    private void PauseContinue()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        if (StartUI.adsOn && Creatures.levelRow > 60)
        {
            AdsManager.Instance.bannerAds.ShowBannerAd();
        }

        foreach (GameObject creature in Creatures.creaturesBodiesList)
        {
            if (creature != null)
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

        Time.timeScale = 1;

        gameObject.SetActive(false);
    }

    private void PauseQuit()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        Time.timeScale = 1;

        StartUI.ShowStartUI();

        SceneManager.LoadScene("Start");
    }

    private void PauseRetry()
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        Time.timeScale = 1;

        SceneManager.LoadScene("Main");
    }
}
