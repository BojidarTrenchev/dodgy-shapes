using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;
using DynamicLight2D;

public class MainMenuManager : MonoBehaviour
{
    public DynamicLight lightening;
    public Transform playerPosition;
    public Text highScore;
    public MoneyController moneyController;
    public MysteryTokensManager tokenManager;
    public RectTransform giftGroup;
    public Canvas mainCanvas;
    public Transform luckyButton;
//    public Transform freeMoneyButton;
  //  public Text freeMoneyText;
   // public Transform buttonPos1;
  //  public Transform buttonPos2;
    public MessageManager messages;
    public RectTransform settings;
    public AllStoreShapesContainer allStoreShapesContainer;
    public float rotationSpeed = 100;
    public int maxRewardedMoneyPrize = 750;
    public Image sfxImage;
    public Sprite sfxOn;
    public Sprite sfxOff;
    public Image musicImage;
    public Sprite musicOn;
    public Sprite musicOff;
    public Image vibrationImage;
    public Sprite vibrationOn;
    public Sprite vibrationOff;
    public GameObject exitBox;

    public Saturation backgroundSaturation = Saturation.Main500;
    public Saturation lightSaturation = Saturation.Main400;

    public Text version;
    private Rigidbody2D playerRb;

    private GiftManager gifts;
    private BlurOptimized blur;
    private RectTransform canvasTr;

    string smtWentWrongError = "Sorry, something went wrong :(";

    //  private int freeMoneySum;
    // private bool isFreeMoneyTaken;
    // private string smtWentWrongError = "Sorry, something went wrong :(";
    private bool isInSettings;
    private bool isInGift;
    private bool initialSoundSettings;
    private bool initialMusicSettings;

    void Start()
    {
        bool giveFreeGift = PlayerInfo.info.CanGiveFreeGift() && Advertisement.IsReady("rewardedVideo");
        // bool giveFreeMoney = PlayerInfo.info.CanGiveFreeMoney() && Advertisement.IsReady("rewardedVideo");

        if (giveFreeGift)
        {
            gifts = Instantiate(giftGroup).GetComponent<GiftManager>();
            giftGroup = gifts.GetComponent<RectTransform>();
            giftGroup.SetParent(mainCanvas.GetComponent<RectTransform>(),false);
            luckyButton.gameObject.SetActive(true);
           // luckyButton.position = buttonPos1.position;
        }

        //if (giveFreeMoney)
        //{
        //    freeMoneyButton.gameObject.SetActive(true);
        //    PlayerPrefs.DeleteKey("freeMoneySum");
        //    freeMoneySum = PlayerPrefs.GetInt("freeMoneySum", 0);
        //    if (freeMoneySum == 0 || isFreeMoneyTaken)
        //    {
        //        freeMoneySum = MoneyController.RoundSumWithZeroes(Random.Range(100, maxRewardedMoneyPrize), 1);
        //        PlayerPrefs.SetInt("freeMoneySum", freeMoneySum);
        //        PlayerPrefs.Save();
        //        isFreeMoneyTaken = false;
        //    }
        //    freeMoneyText.text = freeMoneySum.ToString();
        //    if (giveFreeGift)
        //    {
        //        freeMoneyButton.position = buttonPos2.position;
        //    }
        //    else
        //    {
        //        freeMoneyButton.position = buttonPos1.position;
        //    }

        //}


        Time.timeScale = 1;
        lightening.lightMaterial.color = PlayerInfo.info.GetMainColor(lightSaturation);
        Camera.main.backgroundColor = PlayerInfo.info.GetMainColor(backgroundSaturation);
        
        Transform player = Instantiate(allStoreShapesContainer.allStoreShapes[PlayerInfo.info.activePlayerShapeIndex].gameObject).GetComponent<Transform>();
        player.parent = GetComponent<Transform>();
        player.localScale = Vector2.one;
        player.localPosition = Vector2.zero;

        highScore.text = PlayerInfo.info.highScore.ToString();
        blur = Camera.main.GetComponent<BlurOptimized>();


        if (MainAuidoManager.isPlayingSound)
        {
            sfxImage.sprite = sfxOn;
        }
        else
        {
            sfxImage.sprite = sfxOff;
        }

        if (MainAuidoManager.isPlayingMusic)
        {
            musicImage.sprite = musicOn;
        }
        else
        {
            musicImage.sprite = musicOff;
        }

        if (Vibration.canVibrate)
        {
            vibrationImage.sprite = vibrationOn;
        }
        else
        {
            vibrationImage.sprite = vibrationOff;
        }

        version.text = Application.version;
    }

    public void Update()
    {

        if (!isInSettings && !isInGift)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ShowExitBox();
            }
        }

        if (gifts != null)
        {
            if (gifts.CanHideGiftGroup())
            {
                if (Input.GetButton("Fire1") || Input.GetKeyDown(KeyCode.Escape))
                {
                    ShowMainCanvas();
                    isInGift = false;
                    gifts.isAnimationDone = false;
                }
            }
        }

        if (isInSettings)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && LoadingManager.canUseBackButton)
            {
                HideSettings();
            }
        }

    }

    public void TurnOnOffSFX()
    {
        MainAuidoManager.isPlayingSound = !MainAuidoManager.isPlayingSound;

        if (MainAuidoManager.isPlayingSound)
        {
            sfxImage.sprite = sfxOn;
        }
        else
        {
            sfxImage.sprite = sfxOff;
        }        
    }

    public void TurnOnOffMusic()
    {
        MainAuidoManager.mainAudio.StartStopMusic();

        if (MainAuidoManager.isPlayingMusic)
        {
            musicImage.sprite = musicOn;
        }
        else
        {
            musicImage.sprite = musicOff;
        }
    }

    public void TurnOnOffVibration()
    {
        Vibration.canVibrate = !Vibration.canVibrate;

        if (Vibration.canVibrate)
        {
            vibrationImage.sprite = vibrationOn;
        }
        else
        {
            vibrationImage.sprite = vibrationOff;
        }
    }

    public void ShowSettings()
    {
        MainAuidoManager.mainAudio.PlayClickSound();
        messages.HideObjects();
        blur.enabled = true;
        settings.gameObject.SetActive(true);
        isInSettings = true;
        initialSoundSettings = MainAuidoManager.isPlayingSound;
        initialMusicSettings = MainAuidoManager.isPlayingMusic;
        TouchRotate.isRotatingByDrag = false;
    }       

    public void HideSettings()
    {
        messages.ShowObjects();
        blur.enabled = false;
        settings.gameObject.SetActive(false);
        isInSettings = false;

        MainAuidoManager.mainAudio.PlayClickSound();

        if (initialSoundSettings != MainAuidoManager.isPlayingSound || initialMusicSettings != MainAuidoManager.isPlayingMusic)
        {
            PlayerInfo.info.SaveData();
            initialSoundSettings = MainAuidoManager.isPlayingSound;
            initialMusicSettings = MainAuidoManager.isPlayingMusic;
        }
        TouchRotate.isRotatingByDrag = true;
    }

    public void UnlockAllShape()
    {
        PlayerInfo.info.lastUnlockedShapeIndex = 43;
    }

    private void ShowExitBox()
    {
        messages.HideObjects();
        blur.enabled = true;
        exitBox.SetActive(true);
    }

    public void HideExitBox()
    {
        exitBox.SetActive(false);
        blur.enabled = false;
        messages.ShowObjects();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void ShowMainCanvas()
    {
        if (gifts.addedSum > 0)
        {
            moneyController.ChangeMoney(gifts.addedSum);
            PlayerInfo.info.SaveData();
        }

        if (gifts.addedTokens > 0)
        {
            tokenManager.AddTokens(gifts.addedTokens);
            PlayerInfo.info.SaveData();
        }

        gifts.HideGiftGroup();

        messages.ShowObjects();
        blur.enabled = false;
        TouchRotate.isRotatingByDrag = true;
    }

    public void ShowGiftCanvas()
    {
        var options = new ShowOptions { resultCallback = HandleShowResult };
        Advertisement.Show("rewardedVideo", options);
        MainAuidoManager.mainAudio.PlayClickSound();
        isInGift = true;

        //if (freeMoneyButton.gameObject.activeSelf)
        //{
        //    freeMoneyButton.position = buttonPos1.position;
        //}
    }

    //public void ShowRewardedAd()
    //{
    //    var options = new ShowOptions { resultCallback = HandleShowResult };
    //    Advertisement.Show("rewardedVideo", options);
    //    MainAuidoManager.mainAudio.PlayClickSound();
    //}

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:

                luckyButton.gameObject.SetActive(false);

                messages.HideObjects();
                blur.enabled = true;
                gifts.ShowGiftMenu();
                TouchRotate.isRotatingByDrag = false;
                //moneyController.ChangeMoney(freeMoneySum);
                //freeMoneyButton.gameObject.SetActive(false);
                //isFreeMoneyTaken = true;
                //PlayerInfo.info.SetNewLastDateMoney();
                //PlayerInfo.info.SaveData();
                break;
            case ShowResult.Skipped:
                break;
            case ShowResult.Failed:
                messages.ShowErrorMessage(smtWentWrongError);
                isInGift = false;
                break;
        }
    }
}
