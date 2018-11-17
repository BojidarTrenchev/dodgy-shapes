using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityStandardAssets.ImageEffects;
using UnityEngine.Advertisements;
using DynamicLight2D;

enum ChooseButtonState
{
    Inactive, Active, Activate, Buy, Ad
}

public class StoreUIController : MonoBehaviour
{
    public Text money;
    public Text addedMoneyValue;
    public StoreManager store;
    public Animator moneyAnim;
    public Text priceText;
    public Image priceImage;
    public Button chooseButton;
    public Text adText;
    public Image adImage;
    public RectTransform[] bonuses;
    public GameObject lightening;
    public RectTransform colorContainer;
    public Button colorChangeButton;
    public GameObject colorMenuCloseButton1;
    public GameObject colorMenuCloseButton2;
    public RectTransform checkColorMark;
    public MessageManager messages;

    public Saturation lightSaturation = Saturation.Main200;
    public Saturation backgroundSaturation = Saturation.Main300;

    private float spaceBetweenColors = 25f;
    private SwipeInput swipe;

    private Text[] bonusesText;
    private Text chooseButtonText;
    private Image chooseButtonImage;

    private BlurOptimized blur;

    private ShapeChooserController currentShape;

    private ChooseButtonState currentButtonState;
    //private bool beginChanging;
    //private int currentSum;
    //private int extractSum;
    //private int finalSum;

    private readonly string adError = "Sorry, something went wrong :(";
    public void Start()
    {
        chooseButtonText = chooseButton.GetComponentInChildren<Text>();
        chooseButtonImage = chooseButton.GetComponent<Image>();
        
        blur = Camera.main.GetComponent<BlurOptimized>();
        swipe = GameObject.FindGameObjectWithTag("Swipe").GetComponent<SwipeInput>();
        money.text = PlayerInfo.info.allCoins.ToString();
        bonusesText = new Text[bonuses.Length];
        for (int i = 0; i < bonusesText.Length; i++)
        {
            bonusesText[i] = bonuses[i].GetComponent<Text>();
        }

        ChangeButtonText(PlayerInfo.info.activePlayerShapeIndex, store.GetShape(PlayerInfo.info.activePlayerShapeIndex));

        lightening.GetComponent<DynamicLight>().lightMaterial.color = PlayerInfo.info.GetMainColor(lightSaturation);
        Camera.main.backgroundColor = PlayerInfo.info.GetMainColor(backgroundSaturation);
    }

    public void Update()
    {
        //if (beginChanging)
        //{
        //    currentSum -= extractSum;
        //    if (currentSum <= finalSum)
        //    {
        //        money.text = finalSum.ToString();
        //        beginChanging = false;
        //    }
        //    else
        //    {
        //        money.text = currentSum.ToString();
        //    }
        //}

        if (Input.GetKeyUp(KeyCode.Escape) && LoadingManager.canUseBackButton)
        {
            GoToMainMenu();
        }
    }

    public void GoToMainMenu()
    {
        LoadingManager.Load(0);
    }

    public void OnClick()
    {
        MainAuidoManager.mainAudio.PlayClickSound();
        int currentIndex = SwipeInput.currentIndex;

        currentShape = store.GetShape(currentIndex);
        chooseButton.interactable = false;
        if (currentButtonState == ChooseButtonState.Activate)
        {
            store.ChooseActiveShape(currentIndex);
            ChangeButtonText(currentIndex, currentShape);
        }
        else if (currentButtonState == ChooseButtonState.Ad)
        {
            ShowRewardedAd();
        }
        else if (currentButtonState == ChooseButtonState.Buy)
        {
            store.Buy(currentIndex);
            ChangeButtonText(currentIndex, currentShape);
            store.moneyController.ChangeMoney(-currentShape.price);
        }


        //if (store.IsShapeBought(currentIndex))
        //{
        //    if (currentIndex != PlayerInfo.info.activePlayerShapeIndex)
        //    {
        //        //the shape is activated
        //        store.ChooseActiveShape(currentIndex);
        //        ChangeButtonText(currentIndex, currentShape);
        //    }
        //}
        //else
        //{
        //    if (currentIndex <= PlayerInfo.info.lastUnlockedShapeIndex)
        //    {
        //        if (isAdReady)
        //        {
        //            ShowRewardedAd();
        //        }
        //        else if (PlayerInfo.info.allCoins >= currentShape.price)
        //        {
        //            store.Buy(currentIndex);
        //            ChangeButtonText(currentIndex, currentShape);
        //            store.moneyController.ChangeMoney(-currentShape.price);
        //        }
        //        else
        //        {
        //            messages.ShowErrorMessage(notEnoughMoney);
        //        }

        //    }
        //}
        //chooseButton.interactable = false;

    }

    public void ShowColorsMenu()
    {
        if (store.IsShapeBought(SwipeInput.currentIndex))
        {
            MainAuidoManager.mainAudio.PlayClickSound();
            swipe.TurnOnColorsDrag();
            colorChangeButton.gameObject.SetActive(false);
            chooseButton.interactable = false;

            if (chooseButtonImage.enabled)
            {
                Color col = chooseButtonImage.color;
                col.a = 0.5f;
                chooseButtonImage.color = col;
            }
            else if (adImage.enabled)
            {
                Color col = adImage.color;
                col.a = 0.3f;
                adImage.color = col;
            }
            colorMenuCloseButton1.SetActive(true);
            colorMenuCloseButton2.SetActive(true);
            blur.enabled = true;
            colorContainer.gameObject.SetActive(true);
            Vector2 pos = checkColorMark.anchoredPosition;
            pos.x = store.GetShape(SwipeInput.currentIndex).accentColor * spaceBetweenColors;
            checkColorMark.anchoredPosition = pos;

            colorContainer.DOAnchorPosY(0, 0.2f);

            for (int i = 0; i < bonusesText.Length; i++)
            {
                if (bonusesText[i].color.a == 1)
                {
                    bonusesText[i].enabled = false;
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void MoveCheckColorMark(int indexOfColor)
    {
        checkColorMark.DOAnchorPosX(indexOfColor * spaceBetweenColors, 0.2f);
    }

    public void HideColorsMenu()
    {
        MainAuidoManager.mainAudio.PlayClickSound();
        colorContainer.DOAnchorPosY(10, 0.2f);
        colorContainer.gameObject.SetActive(false);
        colorChangeButton.gameObject.SetActive(true);
        chooseButton.interactable = true;
        if (chooseButtonImage.enabled)
        {
            Color col = chooseButtonImage.color;
            col.a = 1;
            chooseButtonImage.color = col;
        }
        else if (adImage.enabled)
        {
            Color col = adImage.color;
            col.a = 1;
            adImage.color = col;
        }
        colorMenuCloseButton1.SetActive(false);
        colorMenuCloseButton2.SetActive(false);
        blur.enabled = false;
        swipe.TurnOffColorsDrag();

        for (int i = 0; i < bonusesText.Length; i++)
        {
            if (!bonusesText[i].enabled)
            {
                bonusesText[i].enabled = true;
            }
            else
            {
                break;
            }
        }
    }

    public void OnDestroy()
    {
        swipe.TurnOffColorsDrag();
    }

    public void ChangeButtonText(int indexOfShape, ShapeChooserController currentStoreShape)
    {
        //make last unlocked shape index

        if (indexOfShape > -1 && indexOfShape <= PlayerInfo.info.lastUnlockedShapeIndex && currentStoreShape != null)
        {
            //if (adText.enabled)
            //{
            //    adText.enabled = false;
            //    adImage.enabled = false;
            //}

            //chooseButton.gameObject.SetActive(true);
            //chooseButtonImage.enabled = true;
            ////chooseButtonText.enabled = true;
            //chooseButtonImage.DOFade(1, 0.6f);

            //chooseButton.interactable = true;



            if (store.IsShapeBought(indexOfShape))
            {
                //priceText.enabled = false;
                //priceImage.enabled = false;
                //chooseButtonText.enabled = true;
                if (indexOfShape == PlayerInfo.info.activePlayerShapeIndex)
                {
                    //chooseButtonImage.enabled = false;
                    //chooseButton.interactable = false;

                    //chooseButtonText.text = "active";
                    currentButtonState = ChooseButtonState.Active;
                }
                else
                {
                   // chooseButtonText.text = "activate";
                    currentButtonState = ChooseButtonState.Activate;
                    //chooseButtonImage.enabled = true;
                    //chooseButtonImage.DOFade(1, 0.6f);
                   // chooseButton.interactable = true;
                }
               // chooseButtonText.DOFade(1, 0.6f);
            }
            else
            {
                if (currentStoreShape.hasAd && Advertisement.IsReady("rewardedVideo"))
                {
                    //isAdReady = true;

                    //chooseButtonText.enabled = false;
                    //chooseButtonImage.enabled = false;
                    //adImage.enabled = true;
                    //adText.enabled = true;

                    //adImage.DOFade(1, 0.6f);
                    //adText.DOFade(1, 0.6f);
                    //chooseButton.interactable = true;
                    currentButtonState = ChooseButtonState.Ad;
                }
                else
                {
                    //isAdReady = false;
                    //priceText.enabled = true;
                    //priceImage.enabled = true;
                    //priceText.text = currentStoreShape.price.ToString();

                    //Color c = priceText.color;
                    //c.a = 0;
                    //priceText.color = c;

                    //c = priceImage.color;
                    //c.a = 0;
                    //priceImage.color = c;

                    //priceText.DOFade(1, 0.6f);
                    //priceImage.DOFade(1, 0.6f);

                    //chooseButtonText.enabled = true;
                    //chooseButtonText.text = "buy";
                    currentButtonState = ChooseButtonState.Buy;
                    //chooseButtonText.DOFade(1, 0.6f);
                    //chooseButtonImage.enabled = true;
                    //chooseButtonImage.DOFade(1, 0.6f);
                    //if (PlayerInfo.info.allCoins < currentStoreShape.price)
                    //{
                    //    chooseButton.interactable = false;
                    //}
                    //else
                    //{
                    //    //chooseButton.interactable = true;
                    //}
                }
            }

        }
        else
        {
            //priceText.enabled = false;
            //priceImage.enabled = false;

            //chooseButtonText.enabled = false;
            //chooseButtonImage.enabled = false;
            //chooseButton.interactable = false;
            //adText.enabled = false;
            //adImage.enabled = false;
            currentButtonState = ChooseButtonState.Inactive;
            //chooseButton.gameObject.SetActive(false);
        }

        int price;
        if (currentStoreShape == null)
        {
            price = 0;
        }
        else
        {
            price = currentStoreShape.price;
        }
        ShowButton(currentButtonState, price);
    }

    private void ShowButton(ChooseButtonState state, int price)
    {
        //if (adText.enabled)
        //{
        //    adText.enabled = false;
        //    adImage.enabled = false;
        //}

        //if (priceText.enabled)
        //{
        //    priceText.enabled = false;
        //    priceImage.enabled = false;
        //}

        if (state == ChooseButtonState.Buy)
        {

            chooseButtonText.enabled = true;
            chooseButtonText.text = "buy";

            chooseButtonText.DOFade(1, 0.6f);


            priceText.enabled = true;
            priceImage.enabled = true;
            priceText.text = price.ToString();

            Color c = priceText.color;
            c.a = 0;
            priceText.color = c;

            c = priceImage.color;
            c.a = 0;
            priceImage.color = c;

            priceText.DOFade(1, 0.6f);
            priceImage.DOFade(1, 0.6f);

            adText.enabled = false;
            adImage.enabled = false;


            if (PlayerInfo.info.allCoins < price)
            {
                chooseButton.interactable = false;
                chooseButtonImage.DOFade(0.3f, 0.6f);
                chooseButtonImage.enabled = true;
            }
            else
            {
                chooseButton.interactable = true;
                chooseButtonImage.DOFade(1, 0.6f);
                chooseButtonImage.enabled = true;
            }


        }
        else if (state == ChooseButtonState.Ad)
        {

            adImage.enabled = true;
            adText.enabled = true;
            adImage.DOFade(1, 0.6f);
            adText.DOFade(1, 0.6f);

            chooseButtonImage.enabled = false;
            chooseButtonText.enabled = false;

            priceText.enabled = false;
            priceImage.enabled = false;

            chooseButton.interactable = true;
        }
        else if (state == ChooseButtonState.Activate)
        {

            chooseButtonImage.enabled = true;
            chooseButtonText.enabled = true;
            chooseButtonText.text = "activate";
            chooseButtonImage.DOFade(1, 0.6f);
            chooseButtonText.DOFade(1, 0.6f);

            priceText.enabled = false;
            priceImage.enabled = false;

            adText.enabled = false;
            adImage.enabled = false;

            chooseButton.interactable = true;
        }
        else if (state == ChooseButtonState.Active)
        {
            chooseButtonText.enabled = true;
            chooseButtonText.text = "active";
            chooseButtonText.DOFade(1, 0.6f);

            chooseButtonImage.enabled = false;
            priceText.enabled = false;
            priceImage.enabled = false;

            adText.enabled = false;
            adImage.enabled = false;
            chooseButton.interactable = false;

        }
        else
        {
            chooseButton.interactable = false;
            chooseButtonImage.enabled = false;
            chooseButtonText.enabled = false;
            adText.enabled = false;
            adImage.enabled = false;

            priceText.enabled = false;
            priceImage.enabled = false;
        }

    }

    public void ShowBonuses(int indexInArray)
    {

        if (indexInArray > PlayerInfo.info.lastUnlockedShapeIndex || indexInArray == PlayerInfo.info.activePlayerShapeIndex)
        {
            return;
        }
        string[] shapeBonuses = PlayerInfo.info.GetBonuses(indexInArray);
        if (shapeBonuses == null)
        {
            return;
        }

        int bonusesLength = shapeBonuses.Length;
        Color c;
        for (int i = 0; i < bonusesText.Length; i++)
        {
            // bonusesText[i].enabled = true;
            if (i < bonusesLength)
            {
                bonusesText[i].text = shapeBonuses[i];
                bonuses[i].DOAnchorPosY(-i * 10 + 100, 0.3f);
                c = bonusesText[i].color;
                c.a = 1;
                bonusesText[i].color = c;
            }
            else
            {
                c = bonusesText[i].color;
                c.a = 0;
                bonusesText[i].color = c;
            }

        }
    }

    public void HideBonuses()
    {
        Color c;
        for (int i = 0; i < bonuses.Length; i++)
        {
            if (bonusesText[i].color.a == 0)
            {
                break;
            }
            Vector2 pos = bonuses[i].anchoredPosition;
            pos.y += 10;
            bonuses[i].anchoredPosition = pos;
            c = bonusesText[i].color;
            c.a = 0;
            bonusesText[i].color = c;
        }
    }

    private void ShowRewardedAd()
    {
        var options = new ShowOptions { resultCallback = HandleShowResult };
        Advertisement.Show("rewardedVideo", options);
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                store.Buy(currentShape.shapeIndex);
                ChangeButtonText(currentShape.shapeIndex, currentShape);
                break;
            case ShowResult.Skipped:
                break;
            case ShowResult.Failed:
                messages.ShowErrorMessage(adError);
                break;
        }
    }
}
