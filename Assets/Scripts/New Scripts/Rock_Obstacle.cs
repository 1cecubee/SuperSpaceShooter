using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock_Obstacle : MonoBehaviour, IDamageable
{
    [SerializeField] private float speed;
    private Vector2 direction;

    private void Start()
    {
        direction = Vector2.down;
    }

    private void Update()
    {
        transform.Translate(speed * direction * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IDamageable damageableRef = collision.GetComponent<IDamageable>();
            damageableRef.Damage(10);
            Destroy(gameObject);
        }
    }
}
