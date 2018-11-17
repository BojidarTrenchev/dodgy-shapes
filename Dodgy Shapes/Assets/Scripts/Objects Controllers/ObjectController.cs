using UnityEngine;

//all spawned objects should inherit this class
public class ObjectController : MonoBehaviour
{
    // if this is negative the object shrinks, if it is positive the object expands
    public float scaleRate = -0.01f; 
    public float minOrMaxScale = 0;

    //this value is added to the YDistance for the next object in SpawnerController
    //this variable should only be used with special objects like speed boosters which require additional distance in front of them 
    public float additionalYDistance = 0;

    // this determines whether the object has a specified spawn rate or not, mostly the good objects have it
    public bool hasARateChance;

    //if hasARateChance = false, the value of ChanceInPercent doesn't matter
    [Range(0, 1000)]
    public int chance;

    public AudioClip onPlayerHitSound;

    protected bool isDead;

    public int initialChance;
    [HideInInspector]
    public SpriteRenderer spriteRend;
    [HideInInspector]
    public Transform mainTransform;

    private Transform spriteTr;
    private Collider2D[] colliders;

    private Transform playerTr;
    protected Values values;
    protected ColorManager colorManager;
    private bool isPointAdded;
    private bool isScaled;
    private bool isScaling = true;
    public void Awake()
    {
        this.spriteTr = this.GetComponentsInChildren<Transform>()[1];
        this.mainTransform = this.GetComponent<Transform>();
        this.colliders = this.GetComponents<Collider2D>();
        this.spriteRend = this.GetComponentInChildren<SpriteRenderer>();
        this.playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        this.values = GameObject.FindGameObjectWithTag("Values").GetComponent<Values>();
    }

    public virtual void Start()
    {
        this.colorManager = GameObject.FindGameObjectWithTag("ColorManager").GetComponent<ColorManager>();
    }

    public virtual void Update()
    {
        if (isDead)
        {
            if (!isScaled)
            {
                //Timing.RunCoroutine((_ChangeScale(this.spriteTr, this.scaleRate, this.minOrMaxScale)));
                ChangeScale(this.spriteTr,this.spriteRend, this.scaleRate, this.minOrMaxScale);
            }

        }
        else
        {
            if (playerTr.position.y >= mainTransform.position.y && !isPointAdded && !mainTransform.CompareTag("Coin"))
            {
                values.AddPoints(1);
                isPointAdded = true;
            }
        }
    }

    public void Kill()
    {
        isDead = true;
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].isTrigger = true;
        }

        MainAuidoManager.mainAudio.PlaySound(onPlayerHitSound);
    }

    private void ChangeScale(Transform transformObj, SpriteRenderer sprtRend, float rate, float minOrMaxScale)
    {
        // minOrMaxScale determines the min shrink value of size or the max expand value of size
        Vector2 scale = transformObj.localScale;

        if (isScaling)
        {
            scale.x += rate;
            scale.y += rate;

            if (minOrMaxScale < 1)
            {
                isScaling = scale.x >= minOrMaxScale;
            }
            else
            {
                isScaling = scale.x <= minOrMaxScale;
            }

            if (!isScaling)
            {
                spriteRend.enabled = false;
                scale = Vector2.one;
                isScaled = true;
            }
            else
            {
                transformObj.localScale = scale;
            }
        }
    }
}
