using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public SpriteRenderer leftBackground;
    public SpriteRenderer rightBackground;
    private SpriteRenderer player;

    private Saturation leftBackgroundSaturation;
    private Saturation rightBackgroundSaturation;

    private const Saturation brightBackgroundSaturation = Saturation.Main200;
    private const Saturation darkBackgroundSaturation = Saturation.Main300;

    private const int countOfMainSaturations = 10;
    private const int countOfAccentSaturations = 4;

    public Sprite playerSprite;
    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().sprtRend;
        playerSprite = player.sprite;
      //  ChangeSprtRendererColor(player, PaletteType.Accent, PlayerInfo.info.playerSaturation);
    }

    public void Start()
    {

        bool isLeftDarker = Random.Range(0, 2) == 0 ? true : false;
        if (isLeftDarker)
        {
            leftBackgroundSaturation = darkBackgroundSaturation;
            rightBackgroundSaturation = brightBackgroundSaturation;
        }
        else
        {
            leftBackgroundSaturation = brightBackgroundSaturation;
            rightBackgroundSaturation = darkBackgroundSaturation;
        }
        ChangeSprtRendererColor(leftBackground, PaletteType.Main, leftBackgroundSaturation);
        ChangeSprtRendererColor(rightBackground, PaletteType.Main, rightBackgroundSaturation);
    }
    public void ChangeEnemyColor(SpriteRenderer enemySprtRenderer)//, bool isLeft)
    {
        int enemyColorSaturation = Random.Range((int)darkBackgroundSaturation + 4, (int)Saturation.Main900);
        //TODO: helper items color

        ChangeSprtRendererColor(enemySprtRenderer, PaletteType.Main, (Saturation)enemyColorSaturation);
    }

    public void ChangeHelperItemColor(SpriteRenderer sprtRend, Saturation accentSaturation, float alpha = 1)
    {
        ChangeSprtRendererColor(sprtRend, PaletteType.Accent, accentSaturation, alpha);
    }

    private static void ChangeSprtRendererColor(SpriteRenderer spriteRnd, PaletteType palette, Saturation saturation, float alpha = 1)
    {
        if (palette == PaletteType.Main && saturation >= Saturation.Accent100)
        {
            Debug.LogError("The palette type Main doesn't match with Accent Saturation");
        }
        if (palette == PaletteType.Accent && saturation < Saturation.Accent100)
        {
            Debug.LogError("The palette type Accent doesn't match with Main Saturation");
        }

        Color newColor;

        if (palette == PaletteType.Main)
        {
           // spriteRnd.color = mainPalette[(int)saturation];
           newColor = PlayerInfo.info.GetMainColor(saturation);
        }
        else
        {
           // spriteRnd.color = accentPalette[(int)saturation];
            newColor = PlayerInfo.info.GetAcColorBySaturation(saturation);
        }

        newColor.a = alpha;
        spriteRnd.color = newColor;
    }
}
