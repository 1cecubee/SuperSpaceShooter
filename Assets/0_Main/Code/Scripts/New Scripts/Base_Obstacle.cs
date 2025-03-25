using MoreMountains.Feedbacks;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Base_Obstacle : MonoBehaviour, IDamageable
{
    [Header("OBSTACLE STATS")]
    [SerializeField]
    private float currentHealth;

    [SerializeField] private float minHealth = 1F;
    [SerializeField] private float maxHealth;
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    public float currentSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private int score;

    [Header("COMPONENT REF")]
    [SerializeField]
    private Rigidbody2D r2b;

    [SerializeField] private TMP_Text heatlhText, scoreText;
    [SerializeField] private GameObject powerUps, coins, goldBar, scorePopUp, scoreProgressBar, blastObject;
    [SerializeField] private Player_Controller playerRef;
    [SerializeField] private MMFeedbacks blinkEffect;

    public ObstacelType currentObstacleType;
    public float minScale = 0.4F;
    public float maxScale = 1F;
    public Vector2 direction;

    private Camera gameCamera;
    private bool decreaseScaleOnHit, isVisible;
    private float explosionForce = 100F;

    [SerializeField]
    public enum ObstacelType
    {
        verticalObstacle,
        horizontalObstacle, // bounceable / non -bounceable 
        shrinkObstacle,
        stopAtRandomLocationObtacle,
        smokeObstacle,
    }

    private void Awake()
    {
        r2b = GetComponent<Rigidbody2D>();
        heatlhText = GetComponentInChildren<TMP_Text>();
        playerRef = FindFirstObjectByType<Player_Controller>();
        scoreProgressBar = GameObject.Find("Score - Text");
        gameCamera = Camera.main;
    }


    private void Start()
    {
        Vector3 screenCenter = gameCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        currentObstacleType = GetObstacleTypeBasedOnLevel();
        Debug.Log("ObstacleType is " + currentObstacleType);
        currentHealth = maxHealth;

        scorePopUp.GetComponent<TMP_Text>().text = ("+" + playerRef.score);

        ObstacleBehaviour();
        ChangeScale();
    }

    private void Update()
    {

        heatlhText.text = currentHealth.ToString("00");
        if (scorePopUp != null)
        {
            if (playerRef.x2Ability == true)
            {
                scorePopUp.GetComponent<TMP_Text>().color = Color.green;
            }
            else
            {
                scorePopUp.GetComponent<TMP_Text>().color = Color.white;
            }
        }

        if (currentHealth <= 0)
        {
            float value = UnityEngine.Random.value;
            int spawnCount = UnityEngine.Random.Range(1, 4);


            if (value <= 0.7f)
            {
                float collectibleValue = UnityEngine.Random.value;

                if (collectibleValue <= 0.7f)
                {
                    for (int i = 0; i < spawnCount; i++)
                    {
                        float coinsValue = UnityEngine.Random.value;

                        if (coinsValue <= 0.9F)
                        {
                            SpawnAndExplode(coins);
                        }
                        else
                        {
                            SpawnAndExplode(goldBar);
                        }
                    }
                }
                else
                {
                    Instantiate(powerUps, transform.position, Quaternion.identity);
                }
            }

            Instantiate(scorePopUp, transform.position, Quaternion.identity);

            if (playerRef != null)
            {
                playerRef.currentScore += playerRef.score;
                Color colour = new Color(UnityEngine.Random.value, 0F, UnityEngine.Random.value, 1.0f);

                gameObject.GetComponent<SpriteRenderer>().color = colour;

                scoreProgressBar.TryGetComponent(out Animator scoreAnimation);
                scoreAnimation.SetTrigger("PlayAnimation");
            }
            DestroyFunction();
        }

        moveObstacle(UnityEngine.Random.Range(minScale, maxScale));
    }

    private void SpawnAndExplode(GameObject prefab)
    {
        GameObject spawnedObject = Instantiate(prefab, transform.position, Quaternion.identity);

        Rigidbody2D rb = spawnedObject.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = spawnedObject.AddComponent<Rigidbody2D>();
        }

        Vector2 explosionDirection = UnityEngine.Random.insideUnitCircle.normalized;
        float torqueAmount = UnityEngine.Random.Range(5F, 8F);
        rb.AddTorque(torqueAmount);
        rb.AddForce(explosionDirection * explosionForce);
    }


    private ObstacelType GetObstacleTypeBasedOnLevel()
    {
        int level = Level_Manager.instance.currentLevel;

        ObstacelType[] allowedTypes;

        if (level == 1)
        {
            allowedTypes = new ObstacelType[] { ObstacelType.verticalObstacle };
        }

        else if (level >= 2 && level <= 5)
        {
            allowedTypes = new ObstacelType[]
                { ObstacelType.verticalObstacle, ObstacelType.horizontalObstacle, ObstacelType.smokeObstacle };
        }
        else if (level >= 5 && level <= 10)
        {
            allowedTypes = new ObstacelType[]
            {
                ObstacelType.verticalObstacle, ObstacelType.horizontalObstacle, ObstacelType.smokeObstacle,
                ObstacelType.shrinkObstacle
            };
        }
        else
        {
            allowedTypes = (ObstacelType[])Enum.GetValues(typeof(ObstacelType));
        }

        int randomIndex = UnityEngine.Random.Range(0, allowedTypes.Length);
        return allowedTypes[randomIndex];
    }

    protected void ObstacleBehaviour()
    {
        switch (currentObstacleType)
        {
            case ObstacelType.horizontalObstacle:
                HorizontalObstacle();
                break;

            case ObstacelType.shrinkObstacle:
                ShrinkObstacle();
                break;

            case ObstacelType.stopAtRandomLocationObtacle:
                StopAtRandomLocationObstacle();
                break;

            case ObstacelType.smokeObstacle:
                SmokeObstacle();
                break;
            default:
                VerticallyMoveObstacle();
                break;
        }
    }


    #region OBSTACLE FUNCTIONS

    private void VerticallyMoveObstacle()
    {
        currentHealth = maxHealth = UnityEngine.Random.Range(1, 10);
        minScale = 0.4F;
        maxScale = 0.7F;
        minSpeed = 1F;
        maxSpeed = 3F;
        Debug.Log("VerticallMoveObstacle");
        //   float verticalSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);
        currentSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);
    }

    private void HorizontalObstacle()
    {
        currentHealth = maxHealth = UnityEngine.Random.Range(1, 6);
        minScale = 0.4F;
        maxScale = 0.7F;
        minSpeed = 1F;
        maxSpeed = 5F;
        Debug.Log("HorizontallyMoveObstacle");
        float horizontalSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);
    }

    private void ShrinkObstacle()
    {
        currentHealth = maxHealth = UnityEngine.Random.Range(10, 20);
        minScale = 0.8F;
        maxScale = 1F;
        minSpeed = 2F;
        maxSpeed = 7F;
        Debug.Log("ShrinkOBstacle");
        decreaseScaleOnHit = true;
    }

    private void StopAtRandomLocationObstacle()
    {
        currentHealth = maxHealth = UnityEngine.Random.Range(40, 50);
        minScale = 1.3F;
        maxScale = 1.3F;
        minSpeed = .2F;
        maxSpeed = .8F;
        Debug.Log("StopAtRandomLocationObstacle");
    }

    private void SmokeObstacle()
    {
        currentHealth = maxHealth = UnityEngine.Random.Range(5, 15);
        minScale = 0.6F;
        maxScale = 0.8F;
        minSpeed = 1F;
        maxSpeed = 4F;
        Debug.Log("SmokeObstacle");
    }

    #endregion


    public void moveObstacle(float speed)
    {
        speed = UnityEngine.Random.Range(minSpeed, maxSpeed);

        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void ChangeScale()
    {
        float healthRatio = (float)(currentHealth - 1) / (maxHealth - 1);
        healthRatio = Mathf.Clamp01(healthRatio);
        transform.localScale = Vector3.Lerp(new Vector3(minScale, minScale, minScale),
            new Vector3(maxScale, maxScale, maxScale), healthRatio);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == ("Bomb"))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            IDamageable damageableRef = collision.GetComponent<IDamageable>();
            damageableRef.Damage(currentHealth);
            Destroy(gameObject);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        currentHealth -= 0.1F;
    }

    public void Damage(float damageAmount)
    {
        if (decreaseScaleOnHit)
        {
            ChangeScale();
        }

        if (isVisible)
        {
            currentHealth -= damageAmount;
            blinkEffect.PlayFeedbacks();
        }
    }

    private IEnumerator ObstacleRotation()
    {
        while (true)
        {
            transform.Rotate(new Vector3(0F, 0F, rotationSpeed * Time.deltaTime));
            yield return null;
        }
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
        Destroy(gameObject);
    }

    private void OnBecameVisible()
    {
        isVisible = true;
    }

    private void DestroyFunction()
    {
        TryGetComponent(out CircleCollider2D collision);
        TryGetComponent(out SpriteRenderer spriteRenderer);
        collision.enabled = false;
        spriteRenderer.enabled = false;
        r2b.linearVelocity = Vector3.zero;
        Instantiate(blastObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}