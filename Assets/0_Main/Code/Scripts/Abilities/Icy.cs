using UnityEngine;

public class Icy : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Damageable"))
        {
            collision.TryGetComponent(out Base_Obstacle obstacle);

            //obstacle.currentSpeed = 0F;
            collision.TryGetComponent(out Rigidbody2D rb);
            rb.linearVelocity = Vector2.zero;
            Debug.Log("WORKING?");
        }

    }
}
