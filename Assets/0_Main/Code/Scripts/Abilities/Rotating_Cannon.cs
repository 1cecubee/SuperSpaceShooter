using UnityEngine;

public class Rotating_Cannon : MonoBehaviour
{
    [SerializeField] private float speed = 10;

    void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        transform.Rotate(0f, 0f, speed * 10 * Time.deltaTime);
    }
}
