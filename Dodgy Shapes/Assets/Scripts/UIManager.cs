using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public Button pauseButton;
    public Image touchSpace;
    public AdManager ads;
    public Text allCoinsOldValue;
    public Text allCoinsNewValue;
    public Text addedValue;
    public Text highscoreText;
    public MessageManager messageSystem;
    public RectTransform newHighScoreMessage;
    public Text newHighScoreText;
    public AudioClip reviveSound;

    private Animator highScoreAnim;
    private Animator allCoinsAnim; 
    private Animator anim;
    private BlurOptimized blur;
    private GraphicRaycaster raycaster;

    private PlayerController player;
    private bool revivePlayer;
    private bool isNewShapeMessageShown;
    private bool isInDeadMenu;
    public void Start()
    {
        blur = Camera.main.GetComponent<BlurOptimized>();
        anim = GetComponent<Animator>();
        allCoinsAnim = allCoinsOldValue.GetComponentInParent<Animator>();
        highScoreAnim = highscoreText.GetComponentInParent<Animator>();
        raycaster = GetComponent<GraphicRaycaster>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        allCoinsOldValue.text = Values.allCoins.ToString();
        highscoreText.text = Values.highScore.ToString();
       
        player.Pause();
        raycaster.enabled = false;
        blur.enabled = true;
        touchSpace.enabled = false;
        // ads.ShowLifeAd();
        ads.ShowMoneyAd();
        isInDeadMenu = true;
    }

    public void Update()
    {
        if (LoadingManager.canUseBackButton)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (messageSystem.isInNewShapesMessage)
                {
                    messageSystem.HideNewShapeMessage();
                }
                else
                {
                    if (pauseButton.interactable)
                    {
                        PauseGame();
                    }

                    if (isInDeadMenu)
                    {
                        LoadingManager.Load(0);
                    }
                }

            }
        }


        if (LoadingManager.hasLoaded)
        {
            raycaster.enabled = true;
            LoadingManager.hasLoaded = false;
        }

        if (messageSystem.isNewShapeMessageHidden)
        {
            messageSystem.isNewShapeMessageHidden = false;
            ShowMenu(true);
            ads.ShowReviveButton();
            anim.SetBool("IsDead", true);
            ChangeAllCoinsPositive(Values.currentCoins);

            if (Values.score > Values.highScore)
            {
                //newHighScoreMessage.DOScale(1, 1.5f);
                //newHighScoreMessage.DOAnchorPosY(-55, 1.5f);
                //newHighScoreText.DOFade(1, 1.5f);
                highscoreText.text = Values.score.ToString();
            }
        }
    }

    public void LoadMainMenu()
    {
        LoadingManager.Load(0);
    }

    public void PauseGame()
    {
        MainAuidoManager.mainAudio.PlayClickSound();
        player.Pause();
        ShowMenu(true);
        anim.SetBool("IsPaused", true);
        //ads.ShowLifeAd();
        ads.ShowMoneyAd();
    }

    public void ResumeGame()
    {
        MainAuidoManager.mainAudio.PlayClickSound();
        ShowMenu(false);
        anim.SetBool("IsPaused", false);
        ads.HideAd();
        isInDeadMenu = false;
    }

    public void ShowDeadMenu()
    {
        if (PlayerInfo.info.mysteryTokens >= PlayerInfo.info.maxMysteryTokens && !isNewShapeMessageShown)
        {
            messageSystem.ShowNewUnlockedShapeMessage();
            isNewShapeMessageShown = true;
        }
        else
        {
            ShowMenu(true);
            ads.ShowReviveButton();
            anim.SetBool("IsDead", true);
            ChangeAllCoinsPositive(Values.currentCoins);

            if (Values.score > Values.highScore)
            {
                //newHighScoreMessage.DOScale(1, 1.5f);
                //newHighScoreMessage.DOAnchorPosY(-55, 1.5f);
                //newHighScoreText.DOFade(1, 1.5f);
                highscoreText.text = Values.score.ToString();
            }
        }

        Values.SaveValues();
        isInDeadMenu = true;
    }

    public void HideDeadMenu()
    {
        ShowMenu(false);
        //the ad is hidden in in AdManager
        anim.SetBool("IsDead", false);

        if (newHighScoreText.color.a > 0)
        {
            newHighScoreMessage.DOAnchorPosY(-40, 1);
            newHighScoreText.DOFade(0, 1);
        }
        allCoinsOldValue.text = Values.allCoins.ToString();
        player.Expand();
        isInDeadMenu = false;
    }
    public void Retry()
    {
        LoadingManager.Load(1);
    }

    public void RevivePlayer()
    {
        MainAuidoManager.mainAudio.PlaySound(reviveSound);
        revivePlayer = true;
        HideDeadMenu();
    }

    //this method is used by an animation (ShowCounter)
    public void StartPlaying()
    {
        if (revivePlayer)
        {
            player.Revive();
            revivePlayer = false;
        }
        else
        {
            player.Resume();
        }

        ShowPauseButtonAndTouch(true);
    }

    public void ShowPauseButtonAndTouch(bool show)
    {
        pauseButton.interactable = show;
        touchSpace.enabled = show;
    }

    public void ChangeAllCoinsPositive(int sumToAdd)
    {
        allCoinsNewValue.text = (sumToAdd + Values.allCoins).ToString();
        addedValue.text = "+ " + sumToAdd.ToString();
        allCoinsAnim.SetTrigger("ChangeAllCoinsValue");
    }

    public void ChangeAllCoinsNegative(int sumToPay)
    {
        allCoinsNewValue.text = (Values.allCoins - sumToPay).ToString();
        addedValue.text = "- " + sumToPay;
        allCoinsAnim.SetTrigger("ChangeAllCoinsValue");
    }      

    private void ShowMenu(bool showMenu)
    {
        highScoreAnim.SetBool("ShowHighscore", showMenu);
        allCoinsAnim.SetBool("ShowAllCoins", showMenu);
        blur.enabled = showMenu;
        if (showMenu)
        {
            pauseButton.interactable = false;
            touchSpace.enabled = false;
        }
    }
}
