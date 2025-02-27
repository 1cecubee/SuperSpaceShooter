using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Player_Controller : MonoBehaviour, IDamageable
{

    [Header("PLAYER STATS")]
    [SerializeField] private Slider progressSlider;

    [SerializeField] private float bulletSpeed;

    [SerializeField] private float targetScore;
    [SerializeField] private float maxHealth;
    [SerializeField] private float targetHealth;
    [SerializeField] private float currentHealth;

    [SerializeField] private float minBulletSpawnDuration = 0.2F;
    [SerializeField] private float maxBulletSpawnDuration = 0.1F;
    [SerializeField] private float currentBulletSpawnDuration;
    [SerializeField] private int numbersOfCoins;

    [Header("COMPONENT REF")]
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private Transform bulletSpawnPoint2;
    [SerializeField] private Transform bulletSpawnPoint3;
    [SerializeField] private CircleCollider2D magnetRange;

    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private Rigidbody2D defaultBulletPrefab;
    [SerializeField] private Rigidbody2D icyBulletPrefab;
    [SerializeField] private Rigidbody2D currentBulletPrefab;
    [SerializeField] private TMP_Text playerHealthText, coinsText, scoreText;
    [SerializeField] private GameObject bomb, shield, mainMenuCanvas;
    [SerializeField] private Coins_Manager coins;


    private float magnetTimer;
    private float shieldTimer;
    private float fireRateTimer;
    private float bulletSpreadTimer;
    private float x2Timer;

    private bool bulletSpread;
    private Camera mainCamera;
    private Vector3 touchLocation;

    public int currentScore;
    public int score = 1;
    public bool x2Ability, magnetAbility;

    private void Awake()
    {
        playerHealthText = GetComponentInChildren<TMP_Text>();

        coins = FindObjectOfType<Coins_Manager>();
        renderer = GetComponent<SpriteRenderer>();
        shield = GameObject.Find("Shield");
    }

    void Start()
    {
        maxHealth = Mathf.Clamp(maxHealth, 1, targetHealth);

        mainCamera = Camera.main;
        currentBulletPrefab = defaultBulletPrefab;
        currentHealth = maxHealth;

        shield.gameObject.SetActive(false);
        currentBulletSpawnDuration = minBulletSpawnDuration;
        StartCoroutine(SpawnBullet());
    }

    void Update()
    {
        playerHealthText.text = currentHealth.ToString("00");   // Setting currentHealth to text 
        coinsText.text = numbersOfCoins.ToString("00");
        scoreText.text = currentScore.ToString("00");

        progressSlider.value = Mathf.Lerp(currentScore, targetScore, 0);


        #region T O U C H  I N P U T 
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            mainMenuCanvas.SetActive(false);
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began)
            {
                touchLocation = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.nearClipPlane));
                touchLocation.z = 0;
            }
            transform.position = Vector3.MoveTowards(transform.position, touchLocation, 20f * Time.deltaTime);
        }
        #endregion
    }

    private void LateUpdate()
    {
        currentHealth = Mathf.Clamp(currentHealth, 1, 99);
    }

    private IEnumerator SpawnBullet()
    {
        while (true)
        {
            var projectile = Instantiate(currentBulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().linearVelocity = bulletSpawnPoint.transform.forward * bulletSpeed * Time.deltaTime;
            projectile.AddForce(bulletSpawnPoint.up * bulletSpeed, ForceMode2D.Impulse);

            if (bulletSpread)
            {
                var projectile2 = Instantiate(currentBulletPrefab, bulletSpawnPoint2.position, Quaternion.identity);
                projectile2.GetComponent<Rigidbody2D>().linearVelocity = bulletSpawnPoint2.transform.forward * bulletSpeed * Time.deltaTime;
                projectile2.AddForce(bulletSpawnPoint2.up * bulletSpeed, ForceMode2D.Impulse);

                var projectile3 = Instantiate(currentBulletPrefab, bulletSpawnPoint3.position, Quaternion.identity);
                projectile3.GetComponent<Rigidbody2D>().linearVelocity = bulletSpawnPoint3.transform.forward * bulletSpeed * Time.deltaTime;
                projectile3.AddForce(bulletSpawnPoint3.up * bulletSpeed, ForceMode2D.Impulse);
            }

            yield return new WaitForSeconds(currentBulletSpawnDuration);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var powerUpsRef = collision.GetComponent<Collectibles>();

        if (collision.gameObject.CompareTag("Coins"))
        {
            numbersOfCoins += 10;
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Gold Bar"))
        {
            numbersOfCoins += 20;
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("PowerUps"))
        {
            if (powerUpsRef.currentPowerUps == Collectibles.powerUps.SPower)
            {
                bulletSpreadTimer = 0F;
                StartCoroutine(BulletSpread());
            }
            if (powerUpsRef.currentPowerUps == Collectibles.powerUps.FPower)
            {
                fireRateTimer = 0F;
                StartCoroutine(IncreaseFireRate());
            }
            if (powerUpsRef.currentPowerUps == Collectibles.powerUps.HPower)
            {
                currentHealth++;
            }
            if (powerUpsRef.currentPowerUps == Collectibles.powerUps.BPower)
            {
                Instantiate(bomb, transform.position, Quaternion.identity);
            }
            if (powerUpsRef.currentPowerUps == Collectibles.powerUps.MPower)
            {
                magnetTimer = 0F;
                StartCoroutine(Magnet());
            }
            if (powerUpsRef.currentPowerUps == Collectibles.powerUps.XPower)
            {
                x2Timer = 0F;
                StartCoroutine(scoreX2());
            }
            if (powerUpsRef.currentPowerUps == Collectibles.powerUps.OPower)
            {
                shieldTimer = 0F;
                StartCoroutine(Shield());
            }
            Destroy(collision.gameObject);
        }
    }


    private IEnumerator BulletSpread()
    {
        while (bulletSpreadTimer < 10F)
        {
            bulletSpread = true;
            bulletSpreadTimer += Time.deltaTime;
            yield return null;
        }
        bulletSpread = false;
    }


    private IEnumerator IncreaseFireRate()
    {
        while (fireRateTimer < 10F)
        {
            currentBulletSpawnDuration = maxBulletSpawnDuration;
            fireRateTimer += Time.deltaTime;
            yield return null;
        }

        currentBulletSpawnDuration = minBulletSpawnDuration;
    }

    private IEnumerator Shield()
    {
        while (shieldTimer < 10F)
        {
            gameObject.tag = "Untagged";
            shield.gameObject.SetActive(true);
            shieldTimer += Time.deltaTime;
            yield return null;
        }

        gameObject.tag = "Player";
        shield.gameObject.SetActive(false);
    }

    private IEnumerator Magnet()
    {
        magnetAbility = true;

        while (magnetTimer < 10F)
        {
            magnetTimer += Time.deltaTime;
            yield return null;
        }
        magnetAbility = false;
    }

    private IEnumerator scoreX2()
    {
        score *= 2;
        while (x2Timer < 10F)
        {
            x2Ability = true;
            x2Timer += Time.deltaTime;
            yield return null;
        }
        x2Ability = false;
        score /= 2;
    }

    private IEnumerator BlinkEffect()
    {
        for (int i = 0; i < 3; i++)
        {
            shield.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.25F);

            shield.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25F);
        }
    }
    public void Damage(float damgeAmount)    // IDamageable function 
    {
        Handheld.Vibrate();
        currentHealth -= damgeAmount;
        if (currentHealth <= 0)
        {
            Handheld.Vibrate();
            Destroy(gameObject);
        }
    }
}
