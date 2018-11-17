using UnityEngine;
using Lean.Touch;
using DG.Tweening;
using System.Collections.Generic;

public class SwipeInput : MonoBehaviour
{
    public Rigidbody2D checker;
    public Transform gradientLight;
    public float minYTouchPosition = -5;
    public float maxYTouchPosition = 5;

    public float deltaXMax = 10;
    public RectTransform colors;
    public SliderController slider;

    public float posX = 0f;
    private float colorPosX = 0;

    private StoreUIController ui;
    public Transform cam;
    private Transform currentObject;
    public static int currentIndex;

    public float refPixelWidth = 480;
    public float refSpeed = 40;
    public float speed;
    private float colorSpeed;
    private float refColorSpeed = 30;

    public AudioClip shapeSnapSound;

    private float maxXPosition;
    public void Awake()
    {
        TurnOnDrag();
    }

    public void Start()
    {       
        ui = GameObject.FindGameObjectWithTag("UIStore").GetComponent<StoreUIController>();
        float ratio = refPixelWidth / Camera.main.pixelWidth;
        speed = ratio * refSpeed;
        colorSpeed = ratio * refColorSpeed;

        maxXPosition = (PlayerInfo.info.allPlayerShapes.Length - 1) * StoreManager.spaceBetweenObjects;

        Vector3 pos = cam.position;
        pos.x = PlayerInfo.info.activePlayerShapeIndex * StoreManager.spaceBetweenObjects;
        pos.z = -10;
        cam.position = pos;
        posX = pos.x;

        slider.MaxValue = maxXPosition;
        slider.Value = posX;
    }

    public void FixedUpdate()
    {
        Vector2 b = checker.position;
        b.x = cam.position.x;
        checker.position = b;

        Vector2 a = gradientLight.position;
        a.x = cam.position.x;
        gradientLight.position = a;
    }

    public void TurnOnDrag()
    {
        posX = cam.position.x;
        LeanTouch.OnGesture += Drag;
    }

    public void TurnOffDrag()
    {
        LeanTouch.OnGesture = null;
    }

    public void TurnOnColorsDrag()
    {
        LeanTouch.OnGesture = null;
        LeanTouch.OnGesture += DragColors;
        slider.Interactable = false;
    }

    public void TurnOffColorsDrag()
    {
        LeanTouch.OnGesture = null;        
        LeanTouch.OnGesture += Drag;
        slider.Interactable = true;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name != "Scaler")
        {
            currentObject = other.GetComponent<Transform>();
            currentObject.DOScale(1.5f, 0.2f);

            ShapeChooserController shape = other.GetComponent<ShapeChooserController>();

            if (shape == null)
            {
                currentIndex = -1;
            }
            else
            {
                currentIndex = shape.shapeIndex;
                ui.ChangeButtonText(currentIndex, shape);
                ui.ShowBonuses(currentIndex);
            }

            MainAuidoManager.mainAudio.PlaySound(shapeSnapSound, 0.4f);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.name != "Scaler")
        {
            ui.HideBonuses();
            ui.ChangeButtonText(-1, null);

            other.GetComponent<Transform>().DOScale(1, 0.2f);
        }
    }

    public void DragColors(List<LeanFinger> fingers)
    {
        Vector2 fingerPosition = fingers[0].GetLastWorldPosition(0);

        if (fingerPosition.y > -5 && fingerPosition.y < 5)
        {
            float deltaX = fingers[0].ScreenDelta.x;//touch.deltaPosition.normalized.x;//initTouch.position.x - touch.position.x;
            colorPosX += deltaX * Time.deltaTime * colorSpeed;
            colorPosX = Mathf.Clamp(colorPosX, -400, 0);
            Vector3 pos = colors.anchoredPosition;
            pos.x = colorPosX;
            pos.y = 0;
            colors.anchoredPosition = pos;
        }
    }

    public void Drag(List<LeanFinger> fingers)
    {
        Vector2 fingerPosition = fingers[0].GetWorldPosition(-10);
        if (fingerPosition.y > minYTouchPosition && fingerPosition.y < maxYTouchPosition)
        {
            float deltaX = fingers[0].ScreenDelta.x;//touch.deltaPosition.normalized.x;//initTouch.position.x - touch.position.x;
            //ismovingRight = deltaX < 0 ? false : true;
            deltaX = Mathf.Clamp(deltaX, -deltaXMax, deltaXMax);
            posX -= deltaX * Time.deltaTime * speed;
            posX = Mathf.Clamp(posX, 0, maxXPosition);

            if (cam == null)
            {
                cam = Camera.main.GetComponent<Transform>();
            }

            if (!slider.isSliding)
            {
                MoveCam(posX);
                //slider.interactable = false;
                slider.Value = posX;
            }
        }
    }

    public void Slide(float value)
    {
        //slider.interactable = true;
        posX = value;
        MoveCam(posX);
    }

    private void MoveCam(float posX)
    {
        Vector3 pos = cam.position;
        pos.x = posX;
        pos.z = -10;
        cam.position = pos;
    }
}
