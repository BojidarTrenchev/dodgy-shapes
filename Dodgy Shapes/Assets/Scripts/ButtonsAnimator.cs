using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonsAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float fillSpeed = 5;
    public Image border;

    private Button mainButton;

    private bool startFill;
    private bool startDefill;
    private bool canDefill;


    public void Start()
    {
        mainButton = GetComponent<Button>();
    }

    void Update()
    {
        if (startFill)
        {
            if (border.fillAmount < 1)
            {
                border.fillAmount += Time.deltaTime * fillSpeed;
                canDefill = false;
            }
            else
            {
                startFill = false;
                border.fillAmount = 1;
                canDefill = true;
            }
        }

        if (startDefill && canDefill)
        {
            if (border.fillAmount > 0)
            {
                border.fillAmount -= Time.deltaTime * fillSpeed;
            }
            else
            {
                startDefill = false;
                border.fillAmount = 0;
            }
        }
    
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (mainButton.enabled && mainButton.interactable)
        {
            startFill = true;
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        startDefill = true;
    }
}
