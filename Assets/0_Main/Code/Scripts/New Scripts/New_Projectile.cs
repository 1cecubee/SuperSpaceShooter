using UnityEngine;
using static Base_Obstacle;

public class New_Projectile : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageableRef = collision.GetComponent<IDamageable>();
        var baseObstacelRef = collision.GetComponent<Base_Obstacle>();
        var goldObstacleRef = collision.GetComponent<Gold_Obstacle>();
        if (collision.gameObject.CompareTag("Damageable"))
        {
            if (baseObstacelRef != null)
            {
                damageableRef.Damage(1F);
                Destroy(gameObject);
            }
            else if (goldObstacleRef != null)
            {
                damageableRef.Damage(1F);
                Destroy(gameObject);
            }
            Destroy(gameObject);
        }

    }

}

//    public ObstacelType currentObstacleType;

//    public enum ObstacelType
//    {
//        verticalObstacle,
//        horizontalObstacle,
//        shrinkObstacle,
//        stopAtRandomLocationObtacle,
//        smokeObstacle,
//    }

//    private void Start()
//    {
//        currentObstacleType = GetRandomEnumType<ObstacelType>();
//    }

//    private T GetRandomEnumType<T>()
//    {
//        Array values = Enum.GetValues(typeof(T));
//        int randomIndex = UnityEngine.Random.Range(0, values.Length);
//        return (T)values.GetValue(randomIndex);
//    }

//}

