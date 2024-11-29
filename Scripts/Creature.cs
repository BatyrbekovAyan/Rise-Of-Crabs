using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Creature : MonoBehaviour
{
    [SerializeField] private GameObject Coin;

    public bool tooClose = false;
    public int totalLife = 0;


    public void GivePoints(int points)
    {
        StartCoroutine(GivePointsCoroutine(points));
    }

    private IEnumerator GivePointsCoroutine(int points)
    {
        yield return new WaitForSeconds(1f);

        Coin.transform.DOMove(GameManager.CoinTopStatic.transform.position, 1f).OnComplete(() =>
        {
            Coin.GetComponent<Image>().DOFade(0, 0.2f);

            GameManager.levelEranedCoins += points;

            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + points);
            GameManager.CoinsTextStatic.text = PlayerPrefs.GetInt("Coins", 0).ToString();
        });
    }

    public void OnDestroy()
    {
        Creatures.blueCreaturesToCreate.Remove(gameObject);
    }
}