using System.Collections;
using UnityEngine;

public class Obstacle_Spawner : MonoBehaviour
{
    private Camera gameCamera;

    [SerializeField] private float minSpawnDuration, maxSpawnDuration;
    [SerializeField] private GameObject obstacle;

    private void Awake()
    {
        gameCamera = Camera.main;

    }

    private void Start()
    {
        StartCoroutine(SpawnObstacle());
    }

    private IEnumerator SpawnObstacle()
    {
        var obstacleRef = obstacle.GetComponent<Base_Obstacle>();
        while (true)
        {
            Color obstacleColour = new Color(Random.value, Random.value, Random.value, 1.0F);
            float randomScale = Random.Range(obstacleRef.minScale, obstacleRef.maxScale);

            float screenWidth = gameCamera.orthographicSize * 2f * gameCamera.aspect;    // VERTICAL POINTS 
            float spawnPosX = Random.Range(-screenWidth / 2, screenWidth / 2);
            float spawnPosY = gameCamera.orthographicSize + 2;

            Vector3 topLeft = gameCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));    // HORIZONTAL POINTS 
            Vector3 bottonLeft = gameCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
            Vector3 topRight = gameCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
            Vector3 bottonRight = gameCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
            float randomLeftPoint = Random.Range(topLeft.y, bottonLeft.y);
            float randomRightPoint = Random.Range(topRight.y, bottonRight.y);
            Vector3 randomLeftPosition = new Vector3(topLeft.x - 2, randomLeftPoint);
            Vector3 randomRightPosition = new Vector3(topRight.x + 2, randomRightPoint);



            float spawnDuration = Random.Range(minSpawnDuration, maxSpawnDuration);
            obstacle.GetComponent<SpriteRenderer>().color = obstacleColour;
            obstacleRef.maxScale = randomScale;
            Vector3 verticalSpawnLocation = new Vector3(spawnPosX, spawnPosY);
            Vector3 horizontalSpawnLocation = Random.value > 0.5F ? randomLeftPosition : randomRightPosition;


            Vector3 obstacleSpawnLocation;
            if (obstacleRef.currentObstacleType == Base_Obstacle.ObstacelType.horizontalObstacle)
            {
                obstacleSpawnLocation = horizontalSpawnLocation;
                Debug.Log("HORIZONTAL");
            }
            else
            {
                obstacleSpawnLocation = verticalSpawnLocation;
                Debug.Log("VERTICAL");
            }

            Instantiate(obstacle, obstacleSpawnLocation, Quaternion.identity);

            yield return new WaitForSeconds(spawnDuration);

        }

    }
}
