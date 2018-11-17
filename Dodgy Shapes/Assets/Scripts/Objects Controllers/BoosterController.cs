using UnityEngine;

public class BoosterController : ObjectController
{
    private PlayerController player;

    public float speedBoost;
    public float durationOfBoost;

    private bool isAlreadyBoosted;   
    public override void Start()
    {
        base.Start();
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isAlreadyBoosted)
        {
            player.SpeedBoost(this.speedBoost, this.durationOfBoost);
            isAlreadyBoosted = true;
        }
    }
}
