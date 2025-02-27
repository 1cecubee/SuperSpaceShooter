using UnityEngine;
using UnityEngine.Jobs;

public class Coins_Manager : MonoBehaviour
{
    [SerializeField] private Player_Controller playerRef;


    private void Awake()
    {
        playerRef = FindObjectOfType<Player_Controller>();
    }

    private void Update()
    {
        if (playerRef.magnetAbility == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerRef.transform.position, 10 * Time.deltaTime);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
