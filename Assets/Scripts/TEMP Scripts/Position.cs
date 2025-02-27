using UnityEngine;

public class Position : MonoBehaviour
{
    [SerializeField] private GameObject target;


    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, 10 * Time.deltaTime);
    }
}
