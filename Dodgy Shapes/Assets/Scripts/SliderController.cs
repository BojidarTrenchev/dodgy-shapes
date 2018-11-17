using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Slider slider;
    public Image handleImage;
    public Image lineImage;
    public bool isSliding;
    public AllStoreShapesContainer allShapes;
    public void Start()
    {
        handleImage.sprite = allShapes.allStoreShapes[PlayerInfo.info.activePlayerShapeIndex].GetComponentInChildren<SpriteRenderer>().sprite;
        handleImage.color = PlayerInfo.info.GetAcColorBySaturation(Saturation.Accent700);
        lineImage.color = PlayerInfo.info.GetMainColor(Saturation.Main600);
    }

    public float Value
    {
        get
        {
            return slider.value;
        }

        set
        {
            slider.value = value;
        }
    }

    public float MaxValue
    {
        get
        {
            return slider.maxValue;
        }
        set
        {
            slider.maxValue = value;
        }
    }
    
    public bool Interactable
    {
        get
        {
            return slider.interactable;
        }
        set
        {
            slider.interactable = value;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSliding = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSliding = false;
    }
}
