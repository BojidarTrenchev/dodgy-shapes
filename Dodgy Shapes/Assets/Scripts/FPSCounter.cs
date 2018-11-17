using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public Text fpsText;
    public float timeInterval = 1;

    private float time;

    public void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        time += Time.unscaledDeltaTime;
        if (time >= timeInterval)
        {
            int fps = (int)(1 / Time.unscaledDeltaTime);
            fpsText.text = fps.ToString();
            time = 0;
        }
    }
}
