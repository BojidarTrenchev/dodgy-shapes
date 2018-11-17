using UnityEngine;
using System.Collections;
using DynamicLight2D;

public class TouchRotate : MonoBehaviour
{
    public static bool isRotatingByDrag = true;

    public DynamicLight dynamicLight;
    public Transform tr;
    public Rigidbody2D rb;
    public float maxRotationSpeed = 500;
    public float minRotationSpeed = 20;
    public float rotationSpeed;
    public float dragRotationMultiplyer = 30;
    public float refPixelWidth = 480;
    private float baseAngle;
    private float rotation;

    private float previousRotation;
    private bool isDragging;
    private bool isClockwise;
    private MeshRenderer lightMesh;

    public void Start()
    {
        lightMesh = dynamicLight.meshRenderer;
        rb.angularVelocity = minRotationSpeed;
        float ratio =  Camera.main.pixelWidth / refPixelWidth;
        dragRotationMultiplyer *= ratio;
    }

    public void Update()
    {
        if (Input.touchCount > 0 && isRotatingByDrag)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 worldTouchPos = Camera.main.ScreenToWorldPoint(touch.position);
            if (worldTouchPos.x > 0 || worldTouchPos.x < -12 || worldTouchPos.y > 10 || worldTouchPos.y < -5)
            {
                isDragging = false;
                lightMesh.enabled = true;
                return;
            }
            if (touch.phase == TouchPhase.Began)
            {
                Vector2 pos = Camera.main.WorldToScreenPoint(tr.position);
                pos = touch.position - pos;
                baseAngle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
                baseAngle -= Mathf.Atan2(tr.right.y, tr.right.x) * Mathf.Rad2Deg;
                isDragging = true;
                previousRotation = rb.rotation;
                lightMesh.enabled = true;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
                pos = touch.position - pos;

                rotation = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg - baseAngle;

                float difference = (rb.rotation - previousRotation);
                if (difference != 0)
                {
                    if (difference > 0)
                    {
                        isClockwise = false;
                    }
                    else
                    {
                        isClockwise = true;
                    }
                }

                previousRotation = rb.rotation;
                rotationSpeed = touch.deltaPosition.magnitude * dragRotationMultiplyer;
                if (rotationSpeed > maxRotationSpeed)
                {
                    rotationSpeed = maxRotationSpeed;
                }

                isDragging = true;
                lightMesh.enabled = false;
            }
            else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
                lightMesh.enabled = true;
            }

        }
    }

    public void FixedUpdate()
    {
        if (isDragging)
        {
            if (rotation != 0)
            {
                rb.rotation = rotation;
                rb.angularVelocity = 0;

                rotation = 0;
            }

        }
        else
        {
            if (rotationSpeed > 0)
            {
                rb.angularVelocity = rotationSpeed;
            }

            if (rotationSpeed > minRotationSpeed)
            {
                rotationSpeed -= 1;
            }
            else
            {
                rotationSpeed = minRotationSpeed;
            }

            if (isClockwise)
            {
                rb.angularVelocity *= -1;
            }
        }
    }
}