using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;

public class GiftManager : MonoBehaviour
{
    public static bool isNewShapeColorChanged;

    public CanvasGroup giftGroup;
    public GameObject shapePrize;
    public GameObject moneyPrize;
    public GameObject storeButton;
    public RectTransform[] giftTransforms;

    public int chanceForToken = 30;
    public int chanceForShape = 5;
    private Image[] giftImages;
    private Button[] giftButtons;

    private Text prizeMoneyOrTokenText;
    public Image prizeMoneyImage;
    public Image prizeTokenImage;
    private Text prizeShapeText;
    public RectTransform prizeShapeTextTr;
    private Image prizeShapeImage;
    public Transform prizeShapeImageTr;

    public int addedTokens;
    public int addedSum;
    public int maxMoneyPrize = 500;

    public AudioClip giftSound;
    public AudioClip newShapeSound;
    public AudioClip newTokenOrMoneySound;

    private bool beginAnim;
    private bool isFadeAnimDone;
    private bool isChosenGiftMoveDone;
    private int chosenGiftIndex;

    private Tween fadeAnim;
    private Tween chosenGiftMoveAnim;
    private Tween chosenGiftFadeAnim;
    private bool isPrizeAnimaDone;
    private Animator giftAnim;
    private bool isMoveAnimDone;
    private bool isShape;
    private bool isToken;
    private AllStoreShapesContainer allStoreShapesContainer;
    public bool isAnimationDone;

    private ParticleSystem giftParticle;

    private int buttonsArrayLength;
    public void Awake()
    {
        giftGroup.alpha = 0;
        giftGroup.interactable = false;
        giftGroup.blocksRaycasts = false;
    }

    public void Start()
    {
        buttonsArrayLength = giftTransforms.Length;
        giftImages = new Image[buttonsArrayLength];
        giftButtons = new Button[buttonsArrayLength];
        for (int i = 0; i < buttonsArrayLength; i++)
        {
            giftImages[i] = giftTransforms[i].GetComponent<Image>();
            Color c = giftImages[i].color;
            c.a = 0;
            giftImages[i].color = c;

            giftButtons[i] = giftTransforms[i].GetComponent<Button>();

            //if (i == buttonsArrayLength / 2 + 1)
            //{
            //    spaceYBetweenGifts = giftTransforms[i].position.y;
            //}
        }

        prizeMoneyOrTokenText = moneyPrize.GetComponentInChildren<Text>();
        prizeShapeImage = shapePrize.GetComponentInChildren<Image>();
        prizeShapeText = shapePrize.GetComponentInChildren<Text>();
        giftAnim = giftGroup.GetComponent<Animator>();

        giftParticle = GameObject.FindGameObjectWithTag("GiftParticle").GetComponent<ParticleSystem>();
        allStoreShapesContainer = GameObject.FindGameObjectWithTag("AllStoreShapesContainer").GetComponent<AllStoreShapesContainer>();
    }

    public void ShowGiftMenu()
    {
        giftGroup.alpha = 1;
        giftGroup.interactable = true;
        giftGroup.blocksRaycasts = true;

        for (int i = 0; i < buttonsArrayLength; i++)
        {
            giftImages[i].DOFade(1, 0.3f);
        }
    }

    public void OnGiftClick(int numberOfGift)
    {
        chosenGiftIndex = numberOfGift;

        for (int i = 0; i < buttonsArrayLength; i++)
        {
            giftButtons[i].interactable = false;
        }

        DeterminePrize();
        beginAnim = true;
        MainAuidoManager.mainAudio.PlaySound(giftSound, 0.45f);
        PlayerInfo.info.SetNewLastDateGift();
    }

    public void Update()
    {
        GiftsAnimation();
    }

    public void HideGiftGroup()
    {
        giftGroup.gameObject.SetActive(false);
    }

    public bool CanHideGiftGroup()
    {
        if (isAnimationDone)
        {
            if (storeButton.activeSelf)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    public void FinishAnimation()
    {
        isAnimationDone = true;
        if (isShape)
        {
            PlayerInfo.info.SaveData();
        }

    }

    public void GoToStore()
    {
        LoadingManager.Load(2);
    }

    private void GiftsAnimation()
    {
        if (beginAnim)
        {
            if (!isFadeAnimDone)
            {
                for (int i = 0; i < buttonsArrayLength; i++)
                {
                    if (i != chosenGiftIndex)
                    {
                        fadeAnim = giftImages[i].DOFade(0, 0.5f);
                    }
                }
                isFadeAnimDone = true;
            }

            if (!fadeAnim.IsPlaying() && isFadeAnimDone)
            {
                if (!isMoveAnimDone)
                {
                    if (giftTransforms[chosenGiftIndex].position.y != 0)
                    {
                        giftTransforms[chosenGiftIndex].DOAnchorPosY(0, 0.8f);
                    }
                    chosenGiftMoveAnim = giftTransforms[chosenGiftIndex].DOScale(3, 1.5f);
                    isMoveAnimDone = true;
                }
            }


            if (chosenGiftMoveAnim != null && !chosenGiftMoveAnim.IsPlaying() && isMoveAnimDone)
            {
                if (!isPrizeAnimaDone)
                {

                    giftImages[chosenGiftIndex].enabled = false;
                    if (isShape)
                    {
                        prizeShapeText.enabled = true;
                        prizeShapeImage.enabled = true;
                       giftAnim.SetTrigger("StartNewShape");
                        MainAuidoManager.mainAudio.PlaySound(newShapeSound, 0.5f);
                        storeButton.SetActive(true);
                    }
                    else
                    {
                        if (isToken)
                        {
                            prizeTokenImage.enabled = true;
                        }
                        else
                        {
                            prizeMoneyImage.enabled = true;
                        }

                        prizeMoneyOrTokenText.enabled = true;
                        giftAnim.SetTrigger("StartNewMoney");
                        MainAuidoManager.mainAudio.PlaySound(newTokenOrMoneySound, 0.5f);
                    }
                    giftParticle.Play();
                    isPrizeAnimaDone = true;
                }
            }
        }
    }

    private void DeterminePrize()
    {
        bool canGiveNextShape = PlayerInfo.info.lastUnlockedShapeIndex < PlayerInfo.info.allPlayerShapes.Length - 1 && !PlayerInfo.info.unlockNextShape;
        int excessedTokens = PlayerInfo.info.maxMysteryTokens - PlayerInfo.info.mysteryTokens;
        int randomNumber = Random.Range(0, 101);
        int top = 0;

        if (canGiveNextShape)
        {
            top += chanceForShape;

            if (top > randomNumber)
            {
                isShape = true;
            }
            else
            {
                isShape = false;
                top += chanceForToken;
                if (top > randomNumber && excessedTokens > 2)
                {
                    isToken = true;
                }
                else
                {
                    isToken = false;
                }
            }
        }


       // isShape = (randomNumber <= chanceForShape ? true : false) && canGiveNextShape;
        addedSum = 0;
        addedTokens = 0;

        if (isShape)
        {
            int currentShapeIndex = PlayerInfo.info.lastUnlockedShapeIndex + 1;
            PlayerInfo.info.shapesAcColors[currentShapeIndex] = PlayerInfo.info.boughtColorsIndeces[Random.Range(0, PlayerInfo.info.boughtColorsIndeces.Count)];
            // prizeShapeImage.sprite = PlayerInfo.info.allStoreShapes[currentShapeIndex].GetComponentInChildren<SpriteRenderer>().sprite;
            isNewShapeColorChanged = true;

            prizeShapeImage.sprite = allStoreShapesContainer.allStoreShapes[currentShapeIndex].GetComponentInChildren<SpriteRenderer>().sprite;
            prizeShapeImage.color = PlayerInfo.info.allAcColors[PlayerInfo.info.shapesAcColors[currentShapeIndex]];
            PlayerInfo.info.unlockNextShape = true;
        }
        else
        {
            // int excessedTokens = PlayerInfo.info.maxMysteryTokens - PlayerInfo.info.mysteryTokens;
            //isToken = (randomNumber <= chanceForToken ? true : false) && canGiveNextShape && excessedTokens > 2;
            if (isToken)
            {
                int tokens = Random.Range(1,  excessedTokens);
                addedTokens = tokens;
                prizeMoneyOrTokenText.text = tokens.ToString();
            }
            else
            {
                int sum = Random.Range(50, maxMoneyPrize);
                sum = MoneyController.RoundSumWithZeroes(sum, 1);
                addedSum = sum;
                prizeMoneyOrTokenText.text = sum.ToString();
            }
        }
    }   
}
