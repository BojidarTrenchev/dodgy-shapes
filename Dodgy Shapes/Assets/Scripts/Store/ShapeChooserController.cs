using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ShapeChooserController : MonoBehaviour
{
    public int shapeIndex;
    public int price;
    public int accentColor = 0;
    public bool hasAd;

    [HideInInspector]
    public SpriteRenderer sprtRend;
    private Transform tr;

    public void Start()
    {
        accentColor = PlayerInfo.info.shapesAcColors[shapeIndex];

        sprtRend = GetComponentInChildren<SpriteRenderer>();
        ChangeColor(accentColor);
    }

    public void ChangeColor(int indexOfColor, bool animation = false)
    {
        accentColor = indexOfColor;
        Color color = PlayerInfo.info.GetAcColor(accentColor);
        PlayerInfo.info.shapesAcColors[shapeIndex] = accentColor;
        //  PlayerInfo.info.allPlayerShapes[SwipeInput.currentIndex].playerColor = color;
        if (PlayerInfo.info.activePlayerShapeIndex == shapeIndex)
        {
            PlayerInfo.info.ChangeAcColor((ColorPalette)accentColor);
        }

        if (animation)
        {
            sprtRend.DOColor(color, 0.5f);
        }
        else
        {
            sprtRend.color = color;
        }

    }
}
