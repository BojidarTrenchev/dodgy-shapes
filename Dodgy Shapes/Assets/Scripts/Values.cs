using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Values : MonoBehaviour
{
    public static int score;
    public static int highScore;
    public static int currentCoins;
    public static int shownCoins;
    public static int allCoins;

    public Text oldValueText;
    public AudioClip coinSound;
    private Text textScore;
    private Text textMoney;

    private const int mainSceneIndex = 1;
    private const int storeSceneIndex = 2;


    //attached to money game object
    public void Awake()
    {       
        highScore = PlayerInfo.info.highScore;
        allCoins = PlayerInfo.info.allCoins;
    }

    public void Start()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex == mainSceneIndex)
        {
            textScore = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Text>();
            textMoney = GameObject.FindGameObjectWithTag("MoneyText").GetComponent<Text>();
        }
    }

    public void MultiplyScore(int multiplier)
    {
        score*= multiplier;
        textScore.text = score.ToString();
    }

    public void AddPoints(int points)
    {
        score += points;
        textScore.text = score.ToString();
    }

    public void AddCoins(int value)
    {
        currentCoins += value;
        shownCoins += value;
        textMoney.text = shownCoins.ToString();
    }

    public void SubstractCoins(int value)
    {
        currentCoins -= value;
        shownCoins -= value;
        textMoney.text = shownCoins.ToString();
    }

    public void MultiplyCoins(int multiplier)
    {
        currentCoins *= multiplier;
        shownCoins *= multiplier;
        textMoney.text = shownCoins.ToString();
    }

    public void OnDestroy()
    {
        score = 0;
        currentCoins = 0;
        shownCoins = 0;
    }

    public static void SaveValues()
    {
        allCoins += currentCoins;
        currentCoins = 0;

        PlayerInfo.info.allCoins = allCoins;

        if (score > highScore)
        {
            PlayerInfo.info.highScore = score;
        }
        PlayerInfo.info.SaveData();
    }

    public void ChangeOldNumber()
    {
        oldValueText.text = allCoins.ToString();
    }

    public void PlayCoinSound()
    {
        MainAuidoManager.mainAudio.PlaySound(coinSound);
    }
}
