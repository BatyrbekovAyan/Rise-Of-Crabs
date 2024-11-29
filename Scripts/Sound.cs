using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    private static bool created = false;

    public static bool MusicEnabled;
    public static bool SoundEnabled;

    private AudioSource[] soundsList;

    public static AudioSource Tap;
    public static AudioSource CreatureHit;
    public static AudioSource CreatureDied;
    public static AudioSource BlueBotCrash;
    public static AudioSource RedBotCrash;
    public static AudioSource BotHitBlueHouse;
    public static AudioSource BotHitRedHouse;
    public static AudioSource OpenShop;
    public static AudioSource BotShoot;
    public static AudioSource Win;
    public static AudioSource Lose;
    public static AudioSource CoinsDrop;
    public static AudioSource ShooterBotPlace;
    public static AudioSource Weee;
    public static AudioSource MainMusic;
    public static AudioSource BackgroundMusic;

    public static Sound Instance;

    public List<AudioClip> MusicClipsList = new();


    private void Start()
    {
        if (created)
        {
            Destroy(gameObject);
        }

        else
        {
            DontDestroyOnLoad(gameObject);
            created = true;
            Instance = this;

            if (PlayerPrefs.GetInt("Music", 1) == 1)
            {
                MusicEnabled = true;
            }
            else
            {
                MusicEnabled = false;
            }
            if (PlayerPrefs.GetInt("Sound", 1) == 1)
            {
                SoundEnabled = true;
            }
            else
            {
                SoundEnabled = false;
            }


            soundsList = transform.GetComponents<AudioSource>();

            Tap = soundsList[0];
            CreatureHit = soundsList[1];
            CreatureDied = soundsList[2];
            BlueBotCrash = soundsList[3];
            RedBotCrash = soundsList[4];
            BotHitBlueHouse = soundsList[5];
            BotHitRedHouse = soundsList[6];
            OpenShop = soundsList[7];
            BotShoot = soundsList[8];
            Win = soundsList[9];
            Lose = soundsList[10];
            CoinsDrop = soundsList[11];
            ShooterBotPlace = soundsList[12];
            Weee = soundsList[13];
            MainMusic = soundsList[14];
            BackgroundMusic = soundsList[15];


            if (MusicEnabled)
            {
                MainMusic.Play();
            }
        }
    }
}
