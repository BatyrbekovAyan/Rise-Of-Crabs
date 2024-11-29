using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Levels : MonoBehaviour
{
    public static int level = 1;

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;

    private int unlockedLevelRow;
    private Color yellow = new(1, 0.7f, 0);
    private Color green = new(0, 1, 0);


    void Start()
    {
        //PlayerPrefs.SetInt("Coins", 0);
        //PlayerPrefs.SetInt("Rubys", 105);
        //PlayerPrefs.SetInt("Hooks", 3);

        //PlayerPrefs.SetInt("UnlockedLevel", 240);
        //PlayerPrefs.Save();

        for (int i = 0; i < content.transform.childCount; i++)
        {
            content.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
        }

        ScrollToUnlockedLevel(content.transform.GetChild(unlockedLevelRow - 1).GetComponent<RectTransform>());
    }

    private void OnEnable()
    {
        unlockedLevelRow = (PlayerPrefs.GetInt("UnlockedLevel", 1) - 1) / 3 + 1;

        for (int i = 0; i < unlockedLevelRow; i++)
        {
            content.transform.GetChild(i).GetComponent<Button>().interactable = true;
            content.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
            content.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);

            if (i + 1 == unlockedLevelRow)
            {
                content.transform.GetChild(i).GetComponent<Image>().color = green;
                content.transform.GetChild(i).GetComponent<Animation>().Play();
            }
            else
            {
                content.transform.GetChild(i).GetComponent<Image>().color = yellow;
            }
        }

        ScrollToUnlockedLevel(content.transform.GetChild(unlockedLevelRow - 1).GetComponent<RectTransform>());
    }

    public void OpenLevel(int level)
    {
        if (Sound.SoundEnabled)
        {
            Sound.Tap.Play();
        }

        Levels.level = 3 * level - 2;
        LevelTranslation.level = Mathf.Floor((Levels.level - 1) / 3 + 1).ToString();
        StartUI.levelsPanelOpen = true;
        SceneManager.LoadScene("Main");
    }

    public void ScrollToUnlockedLevel(RectTransform targetLevel)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewPortLocalPosition = scrollRect.viewport.localPosition;
        Vector2 targetLocalPosition = targetLevel.localPosition;

        if (unlockedLevelRow > 10)
        {
            content.localPosition = new Vector2(-viewPortLocalPosition.x, targetLevel.rect.height / 2 + 512f - viewPortLocalPosition.y - targetLocalPosition.y - scrollRect.viewport.rect.height / 2);
        }

        else if (unlockedLevelRow > 5)
        {
            content.localPosition = new Vector2(-viewPortLocalPosition.x, targetLevel.rect.height / 2 + 272f - viewPortLocalPosition.y - targetLocalPosition.y - scrollRect.viewport.rect.height / 2);
        }

        else
        {
            content.localPosition = new Vector2(-viewPortLocalPosition.x, targetLevel.rect.height / 2 + 32f - viewPortLocalPosition.y - targetLocalPosition.y - scrollRect.viewport.rect.height / 2);
        }
    }
}
