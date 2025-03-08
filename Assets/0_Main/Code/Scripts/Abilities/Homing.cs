using UnityEngine;

public class Homing : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private float damageAmount;

    [SerializeField] private float speed = 1F;
    [SerializeField] private float rotateSpeed = 200F;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        target = GameObject.FindGameObjectWithTag("Damageable");
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector2 direction = (Vector2)target.transform.position - rb.position;

            direction.Normalize();
            float rotateAmount = Vector3.Cross(direction, transform.up).z;

            rb.angularVelocity = -rotateAmount * rotateSpeed;

            rb.linearVelocity = transform.up * speed;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageableRef = collision.GetComponent<IDamageable>();
        var baseObstacelRef = collision.GetComponent<Base_Obstacle>();

        if (collision.gameObject.CompareTag("Damageable"))
        {
            if (baseObstacelRef != null)
            {
                damageableRef.Damage(damageAmount);
                Destroy(gameObject);
            }
        }
    }


    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}