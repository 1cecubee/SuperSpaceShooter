using UnityEngine;

public class IronBall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageableRef = collision.GetComponent<IDamageable>();

        var obstacle = collision.GetComponent<Base_Obstacle>();

        if (obstacle != null)
        {
            damageableRef.Damage(5F);
        }
    }
}
