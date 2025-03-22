using Emp37.Utility;
using UnityEngine;


public class Heart : MonoBehaviour
{

    [Title("S T A T S")]
    [SerializeField] private float healingAmount;

    [Title("C O M P O N E N T S")]
    [SerializeField] private Player_Controller playerController;

    [System.Obsolete]
    private void Awake()
    {
        playerController = FindObjectOfType<Player_Controller>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageableRef = collision.GetComponent<IDamageable>();

        var obstacle = collision.GetComponent<Base_Obstacle>();

        if (collision.gameObject.CompareTag("Damageable"))
        {
            if (obstacle != null)
            {
                damageableRef.Damage(healingAmount);
            }

            if (playerController != null)
            {
                playerController.currentHealth += healingAmount;
                playerController.heartHeal();
            }
            Destroy(gameObject);
        }
    }


    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
