using Emp37.Utility;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Title("References", Shades.Green)]
    [SerializeField] private new Camera camera;
    [SerializeField] private Transform[] corners = new Transform[4];
    [SerializeField, RequireObject] private Base_Obstacle obstaclePrefab;
    [SerializeField] private GameObject gold_Obstacle, rock_Obstacle;


    [Title("Values")]
    [SerializeField] private float offset;


    [Title("Timer", Shades.Skyblue)]
    [SerializeField] private float minimumDuration;
    [SerializeField] private float maximumDuration;
    [SerializeField, Readonly] private float elapsedTime;
    [SerializeField, Readonly] private float nextDuration;

    [Title("Edge Collider 2d", Shades.Yellow)]
    [SerializeField] private EdgeCollider2D edgeCollider;
    [SerializeField] private EdgeCollider2D edgeCollider2;


    private bool istouched = false;

    private void Reset()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            corners[i] = transform.GetChild(i);
        }
        camera = Camera.main;
    }

    private void Awake()
    {
        var position = camera.ViewportToWorldPoint(Vector2.one);

        corners[0].position = new(-position.x, -position.y); corners[1].position = new(-position.x, position.y); corners[2].position = new(position.x, position.y); corners[3].position = new(position.x, -position.y);
    }

    private void Start()
    {
        edgeCollider.transform.position = corners[1].position;
        edgeCollider2.transform.position = corners[2].position;

        Vector3 newScale = transform.localScale;
        newScale.x = Screen.height;
        edgeCollider.transform.localScale = newScale;
        edgeCollider2.transform.localScale = newScale;
    }

    private void LateUpdate()
    {
        if (elapsedTime < nextDuration)
        {
            elapsedTime += Time.deltaTime;
        }
        else
        {
            if (Input.touchCount > 0 && !istouched)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    istouched = true;
                }
            }

            if (istouched)
            {
                SpawnObstacle();
                nextDuration = Random.Range(0.5F, 3F);
                elapsedTime = 0F;
            }
        }
    }

    public void SpawnObstacle()
    {

        nextDuration = Random.Range(minimumDuration, maximumDuration);

        int level = Level_Manager.instance.currentLevel;

        int spawnDirection = 0;

        if (level == 1)
        {
            spawnDirection = 0;
        }
        else if (level > 5)
        {
            spawnDirection = Random.Range(0, 3);
        }

        Vector3 position = spawnDirection switch
        {
            0 => new(x: Random.Range(corners[1].position.x, corners[2].position.x), y: corners[1].position.y + offset),
            1 => new(x: corners[0].position.x - offset, y: Random.Range(corners[0].position.y, corners[1].position.y)),
            _ => new(x: corners[3].position.x + offset, y: Random.Range(corners[2].position.y, corners[3].position.y)),
        };
        float spawnTypeValue = Random.value;

        if (spawnTypeValue < 0.8F)
        {
            var obstacle = Instantiate(obstaclePrefab, position, default);
            Color obstacleColour = new Color(Random.value, 0F, Random.value, 1.0f);
            obstacle.GetComponent<SpriteRenderer>().color = obstacleColour;
            obstacle.direction = new(x: spawnDirection == 0 ? 0F : Mathf.Sign(position.x) * -1F, y: spawnDirection == 0 ? -1F : 0F);
        }
        else if (spawnTypeValue < 0.1)
        {
            //var rock = Instantiate(rock_Obstacle, position, default);
            var goldObstacle = Instantiate(gold_Obstacle, position, default);
        }

    }

}