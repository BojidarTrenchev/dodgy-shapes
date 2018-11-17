using UnityEngine;
using System.Collections;

public class OnClickMove : MonoBehaviour
{
    private PlayerController player;
    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void Move()
    {
        player.MovePlayer();
    }
}
