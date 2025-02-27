using System.Timers;
using UnityEngine;

public class Gold_Obstacle : MonoBehaviour, IDamageable
{

    [SerializeField] private float speed, health, explosionForce;
    [SerializeField] private float elapsedTime, targetTime;

    [SerializeField] private GameObject coins, goldBar;

    public Vector2 direction;

    private void Start()
    {
        direction = Vector2.down;
    }

    private void Update()
    {
        transform.Translate(speed * direction * Time.deltaTime);

        if (health <= 10)
        {
            speed = 0;
            elapsedTime += Time.deltaTime;
        }

        if (elapsedTime >= targetTime)
        {
            Destroy(gameObject);
        }
    }

    public void Damage(float damageAmount)
    {
        float coinsValue = Random.value;
        health -= damageAmount;

        if (coinsValue < 0.5)
        {
            SpawnObstacle(coins);
        }
        else
        {
            SpawnObstacle(goldBar);
        }

        if (health <= 0F)
        {
            for (int i = 0; i < 10; i++)
            {
                SpawnObstacle(coins);
            }
            Destroy(gameObject);
        }
    }

    private void SpawnObstacle(GameObject prefab)
    {
        GameObject spawnObject = Instantiate(prefab, transform.position, default);

        Rigidbody2D rb = spawnObject.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = spawnObject.AddComponent<Rigidbody2D>();
        }
        Vector2 explosionDirection = UnityEngine.Random.insideUnitCircle.normalized;
        rb.AddForce(explosionDirection * explosionForce);
    }
}
