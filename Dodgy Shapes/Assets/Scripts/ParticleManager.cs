using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public ParticleSystem mainExplosion;
    public ParticleSystem coinsParticle;
    public ParticleSystem[] particles;
    public Rigidbody2D particleCollidersRb;

    public Transform particleColTr1;
    public Transform particleColTr2;
    public RectTransform moneyTextPosition;

    private SpriteRenderer player;
    private ParticleSystem playerParticle;

    public void Start()
    {
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().sprtRend;
        playerParticle = mainExplosion.GetComponentInChildren<ParticleSystem>();

        float halfCamWidth = Camera.main.orthographicSize * Camera.main.aspect;
        Vector2 col1Pos = new Vector2(halfCamWidth, 0);
        Vector2 col2Pos = new Vector2(-halfCamWidth, 0);
        particleColTr1.position = col1Pos;
        particleColTr2.position = col2Pos;
    }

    public void Play(Vector2 position, int numberOfParticle, Color color)
    {
        particles[numberOfParticle].transform.position = position;
        //  particles[numberOfParticle].startColor = color;
        ParticleSystem.MainModule mainModule = particles[numberOfParticle].main;
        mainModule.startColor = color;

        if (particles[numberOfParticle].isPlaying)
        {
            particles[numberOfParticle].Clear();
        }

        particles[numberOfParticle].Play();

        Vector2 colPos = new Vector2(0, position.y);
        particleCollidersRb.position = colPos;
    }

    public void PlayCoinsParticle()
    {
        ParticleSystem.MainModule mainMod = coinsParticle.main;
        mainMod.startColor = player.color;
        if (coinsParticle.isPlaying)
        {
            coinsParticle.Clear();
        }
        coinsParticle.Play();

    }

    public void PlayMainExplosion(Vector2 position, Color color)
    {
        mainExplosion.transform.position = position;
        ParticleSystem.MainModule mainModule = mainExplosion.main;
        mainModule.startColor = color;
        mainModule = playerParticle.main;
        mainModule.startColor = player.color;
       // playerParticle.startColor = player.color;


        if (mainExplosion.isPlaying)
        {
            mainExplosion.Clear();
        }
        mainExplosion.Play(true);

        Vector2 colPos = new Vector2(0, position.y);
        particleCollidersRb.position = colPos;
    }
}
