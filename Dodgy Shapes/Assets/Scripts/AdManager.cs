using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{
    //this is also a HIGH SCORE MANAGER
    public Text adText;
    public Text paymentText;
    public UIManager ui;
    public MessageManager messages;
    public Button adButton;
    public Button payButton;
    public Button moneyRewardButton;
    public Button adMoneyButton;
    public Text adMoneyText;
    public Text moneyRewardButtonText;
    public Text boastText;
    public Text moneyRewardText;

    public CloseEnemiesDestroyer closeEnmiesDestroyer;

    public ParticleManager particles;

    public int sumToPay = 3500;
    public int reviveLimit = 3;
    public int maxAdRewardMoneySum = 600;


    private Animator anim;
    private PlayerController player;

    private bool isLifeAdShown;
    private bool isReviveAdShown;
    //private bool isLifeAd;
    private int reviveTimes = 1;

    private int moneyReward;
    private bool isFreeMoneyTaken;
    private bool isMoneyAd;
    private int freeMoneySum;

    private string noCoinsError = "You don't have enough coins :(";
    private string noInternetError = "Sorry, something went wrong :(";
   // private string lifeAdText = "Add 1 hit point!";
    private string reviveAdtext = "Continue playing for free!";

    public void Start()
    { 
        anim = GetComponent<Animator>();
        adText = GetComponentInChildren<Text>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    //public void ShowLifeAd()
    //{
    //    //isLifeAd = true;
    //    //if (!isLifeAdShown && Advertisement.IsReady("rewardedVideo") && player.hitPoints < player.hitPointsMaximum)
    //    //{
    //    //    adText.text = lifeAdText;
    //    //    adButton.interactable = true;
    //    //    anim.SetBool("ShowAd", true);
    //    //}    
    //}


    public void ShowMoneyAd()
    {
        bool giveFreeMoney = PlayerInfo.info.CanGiveFreeMoney();
        if (giveFreeMoney)
        {
            giveFreeMoney = Advertisement.IsReady("rewardedVideo");
        }

        if (giveFreeMoney)
        {
            isMoneyAd = true;
            anim.SetBool("ShowFreeMoneyAd", true);
            adMoneyButton.interactable = true;
            PlayerPrefs.DeleteKey("freeMoneySum");
            freeMoneySum = PlayerPrefs.GetInt("freeMoneySum", 0);
            if (freeMoneySum == 0 || isFreeMoneyTaken)
            {
                freeMoneySum = MoneyController.RoundSumWithZeroes(Random.Range(100, maxAdRewardMoneySum), 1);
                PlayerPrefs.SetInt("freeMoneySum", freeMoneySum);
                PlayerPrefs.Save();
                isFreeMoneyTaken = false;
            }
            adMoneyText.text = freeMoneySum.ToString();
        }
    }         
    
    // Here is handled the New High Score Message!!!!!!!!!!!!!
    public void ShowReviveButton()
    {
       // isLifeAd = false;
        if (!isReviveAdShown && Advertisement.IsReady("rewardedVideo"))
        {
            adButton.interactable = true;
            adText.text = reviveAdtext;
            anim.SetBool("ShowAd", true);
            isMoneyAd = false;

        }
        else if (reviveTimes <= reviveLimit)
        {
            paymentText.text = sumToPay.ToString();

            payButton.interactable = true;
            anim.SetBool("ShowPaymentButton", true);

            reviveTimes++;
            isMoneyAd = false;
        }


        ShowNewHighScore();
    }

    public void HideAd()
    {
        anim.SetBool("ShowAd", false);
        anim.SetBool("ShowPaymentButton", false);
        anim.SetBool("ShowNewHighScore", false);
        anim.SetBool("ShowFreeMoneyAd", false);
        adButton.interactable = false;
        payButton.interactable = false;
        moneyRewardButton.interactable = false;
        adMoneyButton.interactable = false;
    }

    private void Revive()
    {
        ui.RevivePlayer();
        isReviveAdShown = true;
        HideAd();

        closeEnmiesDestroyer.DestroyCloseEnemies(player.playerTransform.position);
    }

    //private void AddLife()
    //{
    //    player.ChangeHitPoints(1);
    //    isLifeAdShown = true;
    //    HideAd();
    //}

    private void GetFreeMoney()
    {
        ui.ChangeAllCoinsPositive(freeMoneySum);
        Values.allCoins += freeMoneySum;
        Values.SaveValues();

        MainAuidoManager.mainAudio.PlayClickSound();
        isMoneyAd = false;
        HideAd();
        PlayerInfo.info.SetNewLastDateMoney();

    }

    public void ClickPay()
    {
        if (Values.allCoins < sumToPay)
        {
            payButton.interactable = false;
            messages.ShowErrorMessage(noCoinsError);
        }
        else
        {
            ui.ChangeAllCoinsNegative(sumToPay);

            Values.allCoins -= sumToPay;
            Values.SaveValues();
            sumToPay += (sumToPay / 2);
            Invoke("Revive", 2);
            payButton.interactable = false;
            moneyRewardButton.interactable = false;
        }
        MainAuidoManager.mainAudio.PlayClickSound();
    }

    public void AddMoneyReward()
    {
        MainAuidoManager.mainAudio.PlayClickSound();

        ui.ChangeAllCoinsPositive(moneyReward);

        Values.allCoins += moneyReward;
        Values.SaveValues();
        moneyRewardButton.interactable = false;
        adButton.interactable = false;
        payButton.interactable = false;

        particles.PlayCoinsParticle();
    }

    public void ShowRewardedAd()
    {
        MainAuidoManager.mainAudio.PlayClickSound();
        var options = new ShowOptions { resultCallback = HandleShowResult };
        Advertisement.Show("rewardedVideo", options);
        adButton.interactable = false;
        moneyRewardButton.interactable = false;
        adMoneyButton.interactable = false;
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                //if (isLifeAd)
                //{
                //    AddLife();
                //}
                if (isMoneyAd)
                {
                    GetFreeMoney();
                }
                else
                {
                    Revive();
                }
                break;
            case ShowResult.Skipped:
                break;
            case ShowResult.Failed:
                messages.ShowErrorMessage(noInternetError);
                break;
        }
    }

    private void ShowNewHighScore()
    {
        if (Values.score > Values.highScore)
        {
            moneyReward = Values.score * 5;
            boastText.text = GetBoastText(Values.score);
            moneyRewardText.text = Values.score + " x 5 = " + moneyReward;
            moneyRewardButtonText.text = "get " + moneyReward;
            moneyRewardButton.interactable = true;
            anim.SetBool("ShowNewHighScore", true);
        }
    }

    private string GetBoastText(int score)
    {
        string boast = string.Empty;
        if (score <=20)
        {
            boast = "Nice!";
        }
        else if (score <= 50)
        {
            boast = "Good!";
        }
        else if (score <= 80)
        {
            boast = "Very Good!";
        }
        else if (score <= 100)
        {
            boast = "Awesome!";
        }
        else if (score <= 135)
        {
            boast = "Excellent!";
        }
        else if (score <= 160)
        {
            boast = "WOW!";
        }
        else if (score <= 200)
        {
            boast = "Outstanding!";
        }
        else if (score <= 250)
        {
            boast = "Phenomenal!";
        }
        else if (score <= 300)
        {
            boast = "You're a legend!";
        }
        else if (score <= 350)
        {
            boast = "WOOOOOOW!";
        }
        else if (score <= 450)
        {
            boast = "BEAST!";
        }
        else if (score >= 450)
        {
            boast = "Extraterrestrial!";
        }
        return boast;
    }
}
