using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class StoreManager : MonoBehaviour
{
    public GameObject questionMark;
    public Transform particleTr;
    public ParticleSystem newUnlockedShapeParticle;
    public MoneyController moneyController;
    public MessageManager messages;
    public AllStoreShapesContainer allStoreShapesContainer;
    public int activeColorIndex;
    public SliderController slider;
    public static float spaceBetweenObjects = 8;

    private List<ShapeChooserController> unlockedStoreShapes;
    private Transform parentTr;
    private StoreUIController ui;
    private Transform camTr;
    private SwipeInput swipe;
    private Transform newlyUnlockedShape;
    private Tween camAnim;
    private bool isCamAnimAssigned;
    private Transform firstQuestionMark;
    private bool beginUnlockingAnimation;

    private float totalTime;
    private float delayInSeconds;
    //attached to object container
    public void Awake()
    {
        unlockedStoreShapes = new List<ShapeChooserController>();
        parentTr = GetComponent<Transform>();
        int firstQuestionMarkIndex = PlayerInfo.info.lastUnlockedShapeIndex + 1;
        for (int i = 0; i < AllStoreShapesContainer.allStoreShapesLength; i++)
        {
            if (i == firstQuestionMarkIndex)
            {
                firstQuestionMark = InstantiateObjects(i);
            }
            else
            {
                InstantiateObjects(i);
            }
        }
    }

    public void Start()
    {
        ui = GameObject.FindGameObjectWithTag("UIStore").GetComponent<StoreUIController>();
        camTr = Camera.main.GetComponent<Transform>();
        swipe = GameObject.FindGameObjectWithTag("Swipe").GetComponent<SwipeInput>();

        if (PlayerInfo.info.unlockNextShape)
        {
            UnlockNextShape();
            delayInSeconds = 1;
        }
    }

    private Transform InstantiateObjects(int shapeIndex)
    {
        Transform tr;
        allStoreShapesContainer.allStoreShapes[shapeIndex].shapeIndex = shapeIndex;
        if (shapeIndex <= PlayerInfo.info.lastUnlockedShapeIndex)
        {
            tr = Instantiate(allStoreShapesContainer.allStoreShapes[shapeIndex]).GetComponent<Transform>();
            unlockedStoreShapes.Add(tr.GetComponent<ShapeChooserController>());
        }
        else
        {
            tr = Instantiate(questionMark).GetComponent<Transform>();
        }
        tr.parent = parentTr;
        Vector2 pos = tr.position;
        pos.x = shapeIndex * spaceBetweenObjects;
        tr.position = pos;

        return tr;
    }

    public ShapeChooserController GetShape(int indexOfShape)
    {
        return unlockedStoreShapes[indexOfShape];
    }

    public bool IsShapeBought(int indexOfShape)
    {
        return PlayerInfo.info.boughtShapes.Contains(indexOfShape);
    }

    public bool isColorBought(int indexOfColor)
    {
        return PlayerInfo.info.boughtColorsIndeces.Contains(indexOfColor);
    }

    public bool BuyOrActivateColor(int indexOfColor, int priceForColor)
    {
        //the method returns true if the shape is bought and false if not
        if (PlayerInfo.info.boughtColorsIndeces.Contains(indexOfColor))
        {
            activeColorIndex = indexOfColor;
            ui.MoveCheckColorMark(indexOfColor);
            unlockedStoreShapes[SwipeInput.currentIndex].ChangeColor(indexOfColor);
            PlayerInfo.info.SaveData();
        }
        else
        {
            if (PlayerInfo.info.allCoins >= priceForColor)
            {
                PlayerInfo.info.boughtColorsIndeces.Add(indexOfColor);
                moneyController.ChangeMoney(-priceForColor);
                activeColorIndex = indexOfColor;
                ui.MoveCheckColorMark(indexOfColor);
                unlockedStoreShapes[SwipeInput.currentIndex].ChangeColor(indexOfColor);
                PlayerInfo.info.SaveData();
                return true;
            }
            else
            {
                messages.ShowErrorMessage("You don't have enough money :(");
            }

        }


        return false;
    }

    public void Buy(int indexOfShape)
    {
        PlayerInfo.info.boughtShapes.Add(indexOfShape);
        PlayerInfo.info.SaveData();
    }

    public void ChooseActiveShape(int indexInArray)
    {
        if (indexInArray < 0 || indexInArray > PlayerInfo.info.lastUnlockedShapeIndex)
        {
            Debug.LogError("You can't activate locked shape.");
        }

        PlayerInfo.info.ChooseActivePlayerShape(indexInArray);
        PlayerInfo.info.ChangeAcColor((ColorPalette)unlockedStoreShapes[indexInArray].accentColor);

        PlayerInfo.info.SaveData();
    }

    public void Update()
    {
        UnlockNextShapeAnimation();
    }

    private void UnlockNextShapeAnimation()
    {
        if (beginUnlockingAnimation)
        {
            totalTime += Time.deltaTime;
            if (totalTime >= delayInSeconds)
            {
                if (!isCamAnimAssigned)
                {
                    camAnim = camTr.DOMoveX(firstQuestionMark.position.x, 2);
                    slider.Interactable = false;
                    isCamAnimAssigned = true;
                }

                slider.Value = camTr.position.x;

                if (!camAnim.IsPlaying() && isCamAnimAssigned)
                {
                    newlyUnlockedShape.gameObject.SetActive(true);
                    particleTr.position = camTr.position;
                    slider.Value = camTr.position.x;
                    slider.Interactable = true;
                    newUnlockedShapeParticle.Play();
                    swipe.TurnOnDrag();
                    beginUnlockingAnimation = false;
                    isCamAnimAssigned = false;
                    totalTime = 0;
                }
            }
        }
    }

    public void UnlockNextShape()
    {
        PlayerInfo.info.unlockNextShape = false;
        MysteryTokensManager.ResetMysteryTokens();
        PlayerInfo.info.lastUnlockedShapeIndex++;
        newlyUnlockedShape = InstantiateObjects(PlayerInfo.info.lastUnlockedShapeIndex);
        newlyUnlockedShape.gameObject.SetActive(false);
        firstQuestionMark.gameObject.SetActive(false);
        beginUnlockingAnimation = true;
        swipe.TurnOffDrag();
        if (!GiftManager.isNewShapeColorChanged)
        {
            PlayerInfo.info.shapesAcColors[PlayerInfo.info.lastUnlockedShapeIndex] = PlayerInfo.info.boughtColorsIndeces[Random.Range(0, PlayerInfo.info.boughtColorsIndeces.Count)];
        }
        else
        {
            GiftManager.isNewShapeColorChanged = false;
        }
        

        PlayerInfo.info.SaveData();
    }
}
