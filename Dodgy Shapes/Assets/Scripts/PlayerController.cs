using UnityEngine;
using EZCameraShake;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [Range(10,30)]
    public float speed = 20f;
    [Range(10, 35)]
    public float maxSpeed = 30;
    [Range(0.001f, 0.01f)]
    public float speedUpCoefficient = 0.002f;
    [Range(2,5)]
    public float dodgeSpeed = 2; // this is the frequency of the joints
    public int hitPoints = 2;
    public int hitPointsMaximum = 3;
    public bool isDead;
    public ColorPalette playerAcPalette = 0;
    public bool changeScalePositive;

    public float scaleRate = 0.01f;

    public AudioClip smallHitSound;
    public AudioClip bigHitSound;
    public AudioClip moveSound;
    public AudioClip coinCollectSound;
    public AudioClip lifeCollectSound;

    [HideInInspector]
    public Transform playerTransform;
    private SpringJointsController joints;
    private Collider2D col;
    private ParticleManager particles;
    private Rigidbody2D rb;
    private ObjectController otherObjController;
    private SpriteRenderer obstacleRenderer;
    private SpawnerController spawner;
    private UIManager ui;
    private LifeBarManager lifeBar;
    private MysteryTokensManager tokenManager;

    [HideInInspector]
    public SpriteRenderer sprtRend;
    [HideInInspector]
    public float positionX;

    private float initialSpeedUpCoefficient;
    private float minSpeedUpCoefficient;
    private float speedBeforeDeath;

    private bool isAboutStopPos;// this is used to enable moving the player only when it is on its stop position
    private float stopPosition;
    public float stopPosOffset = 0.5f;

    //these are determined by the speed booster and used to return the speed to its original state
    private bool boostNow;
    private bool isBoosted;
    private float previousSpeed;
    private float newAddedSpeed;
    private float duration;
    private float minSpeed;
    private float timeCounter = 0;
    private bool isChanceChanged;

    private int startHitPoints;

    public bool isPaused;
    private float speedBeforePause;
    public void Awake()
    {
        this.col = this.GetComponent<Collider2D>();
        this.playerTransform = this.GetComponent<Transform>();
        this.sprtRend = this.GetComponentInChildren<SpriteRenderer>();
        this.rb = this.GetComponent<Rigidbody2D>();

        this.joints = GameObject.FindGameObjectWithTag("SpringJoints").GetComponent<SpringJointsController>();
        this.particles = GameObject.FindGameObjectWithTag("ParticleManager").GetComponent<ParticleManager>();
        this.spawner = GameObject.FindGameObjectWithTag("SpawnerController").GetComponent<SpawnerController>();
        this.ui = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
        lifeBar = GameObject.FindGameObjectWithTag("LifeBar").GetComponent<LifeBarManager>();
    }

    public void Start()
    {
        tokenManager = GameObject.FindGameObjectWithTag("TokenManager").GetComponent<MysteryTokensManager>();
        this.minSpeedUpCoefficient = this.speedUpCoefficient;
        minSpeed = speed;
        SetPosition();
        joints.ChangeFrequency(dodgeSpeed);
        stopPosition = positionX - stopPosOffset;
        startHitPoints = hitPoints;

        sprtRend.color = PlayerInfo.info.GetAcColorBySaturation(PlayerInfo.info.playerSaturation);
    }

    public void Update()
    {
        if (playerTransform.position.x < 0 && playerTransform.position.x <= -stopPosition)
        {
            isAboutStopPos = true;
        }
        else if (playerTransform.position.x > 0 && playerTransform.position.x >= stopPosition)
        {
            isAboutStopPos = true;
        }
        else
        {
            isAboutStopPos = false;
        }
    }

    public void FixedUpdate()
    {
        if (!isPaused)
        {
            Vector2 currentVelocity = this.rb.velocity;
            currentVelocity.y = this.speed;
            this.rb.velocity = currentVelocity;

            BoostSpeed();
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("CloseEnemiesDestroyer"))
        {
            return;
        }
        otherObjController = other.gameObject.GetComponent<ObjectController>();
        otherObjController.Kill();
        obstacleRenderer = otherObjController.spriteRend;

        ChangeHitPoints(-1);
        if (this.hitPoints == 0)
        {
            Die(other);
            //Camera.main.DOShakePosition(1, 3, 25);
            CameraShaker.Instance.ShakeOnce(2, 10, 0, 1);
            Vibration.Vibrate(100);
            MainAuidoManager.mainAudio.PlaySound(bigHitSound, 0.7f);
        }
        else
        {
            speed -= (85 / 100) * speed;
            particles.Play(other.contacts[0].point, 0, obstacleRenderer.color);
            //Camera.main.DOShakePosition(0.5f, 2.2f,25); 
            CameraShaker.Instance.ShakeOnce(1, 10, 0, 0.5f);
            Vibration.Vibrate(50);
            MainAuidoManager.mainAudio.PlaySound(smallHitSound, 0.55f);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CloseEnemiesDestroyer"))
        {
            return;
        }

        otherObjController = other.gameObject.GetComponent<ObjectController>();
        string tag = other.gameObject.tag;
        if (tag == "Life")
        {
            ChangeHitPoints(1);
            otherObjController.Kill();
            MainAuidoManager.mainAudio.PlaySound(lifeCollectSound, 0.4f);
        }
        else if (tag == "Coin")
        {
            other.GetComponent<CoinController>().AddSum();
            MainAuidoManager.mainAudio.PlaySound(coinCollectSound, 0.4f);
        }
        else if (tag == "Token")
        {
            tokenManager.AddTokens(1);
        }

        otherObjController.Kill();

    }

    public void MovePlayer()
    {
        if (!isDead && isAboutStopPos && !isPaused)
        { 
            this.joints.EnableDisableJoints();

            MainAuidoManager.mainAudio.PlaySound(moveSound, 0.3f);

        }
    }

    public void Pause()
    {
        isPaused = true;
        speedBeforePause = speed;
        speed = 0;
        rb.simulated = false;
    }        

    public void Resume()
    {
        isPaused = false;
        speed = speedBeforePause;
        rb.simulated = true;
    }

    private void BoostSpeed()
    {
        if (this.speed >= this.maxSpeed)
        {
            return;
        }

        if (boostNow)
        {
            if (!isBoosted)
            {
                this.previousSpeed = this.speed;
                this.speed += this.newAddedSpeed;
                isBoosted = true;
            }

            this.timeCounter += Time.fixedDeltaTime;

            if (timeCounter >= this.duration)
            {
                this.speed = this.previousSpeed;
                this.timeCounter = 0;
                boostNow = false;
                isBoosted = false;
            }
        }
        else
        {
            this.speed += this.speedUpCoefficient;
        }
    }

    private void SetPosition()
    {
        float cameraWidth = (Camera.main.orthographicSize * 2) * Camera.main.aspect;
        this.positionX = cameraWidth / 4;

        Vector2 currentPosition = this.playerTransform.position;
        currentPosition.x = - this.positionX;
        this.playerTransform.position = currentPosition;
    }

    private void Die(Collision2D other)
    {
        //currentScale = minScale;
        this.isDead = true;
        this.rb.simulated = false;
        this.speedBeforeDeath = this.speed;
        this.speed = 0;
        this.initialSpeedUpCoefficient = this.speedUpCoefficient;
        this.speedUpCoefficient = 0;
        this.col.enabled = false;
        this.joints.TurnoOffJoints();
        this.particles.PlayMainExplosion(other.contacts[0].point, obstacleRenderer.color);
        this.playerTransform.DOScale(0, 1.5f);
        ui.ShowPauseButtonAndTouch(false);
        Invoke("ShowDeadMenu", 2.5f);
    }

    public void Revive()
    {
        // SetPosition();
        this.isDead = false;
        this.rb.simulated = true;

        this.speed = this.speedBeforeDeath / 1.5f;
        if (speed < minSpeed)
        {
            speed = minSpeed;
        }

        this.speedUpCoefficient = this.initialSpeedUpCoefficient / 2;
        if (speedUpCoefficient < minSpeedUpCoefficient)
        {
            speedUpCoefficient = minSpeedUpCoefficient;
        }

        this.col.enabled = true;
        this.joints.TurnOnJoints(playerTransform.position.x < 0);
        this.hitPoints = startHitPoints;
        for (int i = 0; i < hitPoints; i++)
        {
            lifeBar.ShowNextLife();
        }
        //The revive sound is handled in UIManager
    }

    public void SpeedBoost(float speedBoost, float durationOfBoost = 0)
    {
        //if duration == 0 then the speed boost is constant
        if (durationOfBoost <= 0)
        {
            this.speed += speedBoost;
        }
        else
        {
            this.boostNow = true;

            this.newAddedSpeed = speedBoost;
            this.duration = durationOfBoost;
        }
    }

    public void Expand()
    {
        this.playerTransform.localScale = new Vector2(4, 4);
        Color col = sprtRend.color;
        col.a = 0;
        sprtRend.color = col;
        sprtRend.DOFade(1, 2);
        playerTransform.DOScale(0.65f, 2f);
    }

    private void ShowDeadMenu()
    {
        this.ui.ShowDeadMenu();
    }

    public void ChangeHitPoints(int addend)
    {
        if (addend > 0)
        {
            if (hitPoints < hitPointsMaximum)
            {
                this.hitPoints += addend;
                lifeBar.ShowNextLife();

                if (hitPoints == hitPointsMaximum)
                {
                    spawner.DontSpawnObject("Helper Life");
                    isChanceChanged = true;
                }

            }
        }
        else
        {
            hitPoints += addend;
            lifeBar.HideLife();

            if (hitPoints > 0 && isChanceChanged)
            {
                spawner.RestoreObjectToSpawn("Helper Life");
                isChanceChanged = false;
            }
        }
    }
}
