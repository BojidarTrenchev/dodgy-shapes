using UnityEngine;
using UnityEngine.UI;

public class TextScaler : MonoBehaviour
{
    public Text text;
    public int maxSize;
    public int minSize;
    public int sizeCoefficient;

    private bool isMaximizing;
    private bool isMinimizing;

    public void Update()
    {
        if (text.fontSize < maxSize && isMaximizing)
        {
            text.fontSize += sizeCoefficient;
        }
        else
        {
            isMaximizing = false;
        }

        if (text.fontSize >= minSize && isMinimizing)
        {
            text.fontSize -= (int)sizeCoefficient;
        }
        else
        {
            isMinimizing = false;
        }
    }

    public void Maximaize()
    {
        isMaximizing = true;
    }

    public void Minimize()
    {
        isMinimizing = true;
    }
}
