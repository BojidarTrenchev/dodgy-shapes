using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MysteryTokensManager : MonoBehaviour
{
    public static float nextMaxTokensPercent = 0.2f;

    public Text mysteryTokensText;
    public Image questionMark;
    public Image doneImage;

    public SpawnerController spawner;

    public int minFont = 12;
    public int maxFont = 17;

    public AudioClip mysteryTokenSound;
    [HideInInspector]
    public int tokensToAddLater;
    private bool beginAnim;
    private bool doCompletedTokensAnim;
    private int currentTokens;
    private int finalTokens;
    private int addend;

    private string maxTokensString;

    public void Start()
    {
        if (PlayerInfo.info.lastUnlockedShapeIndex == PlayerInfo.info.allPlayerShapes.Length - 1)
        {
            mysteryTokensText.gameObject.SetActive(false);
            questionMark.gameObject.SetActive(false);
            doneImage.gameObject.SetActive(false);
            return;
        }
        maxTokensString = "/" + PlayerInfo.info.maxMysteryTokens;
        mysteryTokensText.text = PlayerInfo.info.mysteryTokens + maxTokensString;

        if (PlayerInfo.info.mysteryTokens >= PlayerInfo.info.maxMysteryTokens)
        {
            mysteryTokensText.fontSize = maxFont;
            questionMark.enabled = false;
            doneImage.enabled = true;
            doneImage.DOFillAmount(1, 1.5f);
            PlayerInfo.info.unlockNextShape = true;
        }
    }

    public void Update()
    {
        if (beginAnim)
        {
            currentTokens += addend;
            if (currentTokens >= PlayerInfo.info.mysteryTokens)
            {
                mysteryTokensText.text = PlayerInfo.info.mysteryTokens + maxTokensString;
                mysteryTokensText.fontSize = minFont;
                beginAnim = false;

                if (doCompletedTokensAnim)
                {
                    mysteryTokensText.fontSize = maxFont;
                    questionMark.enabled = false;
                    doneImage.enabled = true;
                    doneImage.DOFillAmount(1, 1.5f);
                    doCompletedTokensAnim = false;
                }
            }
            else
            {
                mysteryTokensText.text = currentTokens + maxTokensString;
                mysteryTokensText.fontSize = maxFont;
            }

            if (!MainAuidoManager.mainAudio.IsPlayingSound)
            {
                MainAuidoManager.mainAudio.PlaySound(mysteryTokenSound);
            }

        }
    }

    public void ShowTokens()
    {
        questionMark.enabled = true;
        mysteryTokensText.enabled = true;
    }

    public void HideTokens()
    {
        questionMark.enabled = false;
        doneImage.enabled = false;
        mysteryTokensText.enabled = false;
    }

    public void AddTokens(int tokensToAdd)
    {
        currentTokens = PlayerInfo.info.mysteryTokens;
        PlayerInfo.info.mysteryTokens += tokensToAdd;

        if (PlayerInfo.info.mysteryTokens >= PlayerInfo.info.maxMysteryTokens)
        {
            doCompletedTokensAnim = true;
            if (spawner != null)
            {
                spawner.DontSpawnObject("HelperMysteryToken");
            }
            PlayerInfo.info.mysteryTokens = PlayerInfo.info.maxMysteryTokens;
            PlayerInfo.info.unlockNextShape = true;
        }

        beginAnim = true;
        addend = (int)(tokensToAdd * Time.deltaTime);
        if (addend < 1)
        {
            addend = 1;
        }
    }

    public static void ResetMysteryTokens()
    {
        int mysteryTokens = PlayerInfo.info.mysteryTokens;
        int maxMysteryTokens = PlayerInfo.info.maxMysteryTokens;
        if (mysteryTokens >= maxMysteryTokens)
        {
            PlayerInfo.info.mysteryTokens = 0;
            PlayerInfo.info.maxMysteryTokens = maxMysteryTokens + (int)(maxMysteryTokens * nextMaxTokensPercent);
        }
        
    }
}
