using System.Collections;
using Unity.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField, ReadOnly] private Vector3 targetScale = new Vector3(1f, 1f, 1f);
    [SerializeField, ReadOnly] private float circleColliderTarget = 2.5F;
    [SerializeField, ReadOnly] private float duration = .4f;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        transform.localScale = Vector3.zero;

        StartCoroutine(ScaleOverTime(transform, targetScale, duration, circleColliderTarget));
    }

    IEnumerator ScaleOverTime(Transform objectTransform, Vector3 endScale, float duration, float collisionScale)
    {
        Vector3 startScale = objectTransform.localScale;
        float time = 0f;

        if (circleCollider == null)
        {
            yield break;
        }

        float startRadius = 0f;
        float targetRadius = collisionScale;

        while (time < duration)
        {
            float t = time / duration;

            objectTransform.localScale = Vector3.Lerp(startScale, endScale, t);

            circleCollider.radius = Mathf.Lerp(startRadius, targetRadius, t);

            time += Time.deltaTime;
            yield return null;
        }

        objectTransform.localScale = endScale;
        circleCollider.radius = targetRadius;
    }

    private void Update()
    {
        if (transform.localScale == targetScale)
        {
            Destroy(gameObject);
        }
    }
}
