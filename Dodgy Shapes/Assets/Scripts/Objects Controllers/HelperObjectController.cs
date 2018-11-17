using UnityEngine;

public class HelperObjectController : ObjectController
{
    public float alphaRate = 0.3f;
    public float alpha = 1;
    public Saturation saturation = Saturation.Accent100;
    public bool usePlayerSprite;
    private Color sprtColor;
    private bool isAlphaZero;
    private bool hasAParticle;
    private ParticleSystem particle;

    public override void Start()
    {
        base.Start();
        colorManager.ChangeHelperItemColor(spriteRend, saturation, alpha);
        particle = GetComponentInChildren<ParticleSystem>();
        if (particle != null)
        {
            hasAParticle = true;
        }

        if (usePlayerSprite)
        {
            spriteRend.sprite = colorManager.playerSprite;
        }
    }
    public override void Update()
    {
        base.Update();

        if (this.isDead)
        {
            //Timing.RunCoroutine(_ChangeAlphaChannel());

            if (hasAParticle)
            {
                particle.Stop();
                hasAParticle = false;
            }
            if (!isAlphaZero)
            {
                ChangeAlphaChannel();
            }

        }
    }

    //private IEnumerator<float> _ChangeAlphaChannel()
    //{
    //    Color sprtColor = this.spriteRend.color;
    //    while (sprtColor.a > 0)
    //    {
    //        sprtColor.a -= this.alphaRate;
    //        this.spriteRend.color = sprtColor;
    //        yield return 0;
    //    }
    //}

    private void ChangeAlphaChannel()
    {
        sprtColor = this.spriteRend.color;
        if (sprtColor.a > 0)
        {
            sprtColor.a -= this.alphaRate;
        }
        else
        {
            sprtColor.a = 0;
            isAlphaZero = true;
        }
        this.spriteRend.color = sprtColor;
    }
}
