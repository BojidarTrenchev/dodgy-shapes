using UnityEngine;
using System.Collections;

public class CloseEnemiesDestroyer : MonoBehaviour
{
    public Collider2D col;
    public Rigidbody2D rb;

    public void DestroyCloseEnemies(Vector2 position)
    {
        rb.position = position;
        rb.isKinematic = false;
        col.enabled = true;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyObjectController enemy = other.GetComponent<EnemyObjectController>();
            enemy.Kill();

            col.enabled = false;
            rb.isKinematic = true;
        }
    }
}
