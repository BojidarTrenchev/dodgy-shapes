using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class LifeBarManager : MonoBehaviour
{
    public Image[] lifeImages;
    private RectTransform[] lifeTranforms;
    public float durationOfAnimation = 1;
    public float yOffset = 10;

    private int lifeCount;
    private PlayerController playerController;
    private Sprite playerSprt;
    private Color playerColor;
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerSprt = playerController.sprtRend.sprite;
        lifeCount = playerController.hitPoints;
        lifeTranforms = new RectTransform[lifeImages.Length];
        playerColor = PlayerInfo.info.GetAcColorBySaturation(Saturation.Accent700);

        for (int i = 0; i < lifeImages.Length; i++)
        {
            lifeImages[i].sprite = playerSprt;
            lifeImages[i].color = playerColor;
            lifeTranforms[i] = lifeImages[i].GetComponent<RectTransform>();
        }

        Vector2 pos = new Vector2();
        for (int i = 0; i < lifeCount; i++)
        {
            lifeImages[i].enabled = true;
            pos = lifeTranforms[i].anchoredPosition;
            pos.y += yOffset;
            lifeTranforms[i].anchoredPosition = pos;
        }
    }

    public void ShowNextLife()
    {
        lifeCount++;
        lifeImages[lifeCount - 1].enabled = true;
        float y = lifeTranforms[lifeCount - 1].anchoredPosition.y;
        lifeTranforms[lifeCount - 1].DOAnchorPosY(y + yOffset, durationOfAnimation);
        lifeImages[lifeCount - 1].DOFade(1, durationOfAnimation);
    }


    public void HideLife()
    {
        float y = lifeTranforms[lifeCount - 1].anchoredPosition.y;
        lifeTranforms[lifeCount - 1].DOAnchorPosY(y - yOffset, durationOfAnimation);
        lifeImages[lifeCount - 1].DOFade(0, durationOfAnimation);
        lifeCount--;
    }
}
