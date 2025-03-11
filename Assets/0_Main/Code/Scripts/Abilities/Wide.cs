using UnityEngine;

public class Wide : MonoBehaviour
{



    public float targetScaleFactor = 1.5f;
    public float scaleSpeed = 1f;

    private Vector3 initialScale;
    private bool isScaling = false;

    [SerializeField] private float damageAmount = 1F;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageableRef = collision.GetComponent<IDamageable>();

        var obstacle = collision.GetComponent<Base_Obstacle>();

        if (obstacle != null)
        {
            damageableRef.Damage(damageAmount);
        }
    }


    void Start()
    {
        initialScale = transform.localScale;
    }


    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, initialScale * targetScaleFactor, scaleSpeed * Time.deltaTime);
        damageAmount += 0.005F;
    }
}

