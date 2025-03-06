using System.Collections;
using Emp37.Utility;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Player_Controller : MonoBehaviour, IDamageable
{
    [Title("PLAYER STATS", Shades.Red)]
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

    [Title("COMPONENT REF", Shades.Green)]
    [SerializeField] private Transform[] bulletSpawnPointt;
    [SerializeField] private CircleCollider2D magnetRange;

    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private Rigidbody2D defaultBulletPrefab;
    [SerializeField] private Rigidbody2D icyBulletPrefab;
    [SerializeField] private Rigidbody2D currentBulletPrefab;
    [SerializeField] private TMP_Text playerHealthText, coinsText, scoreText;
    [SerializeField] private GameObject bomb, shield;
    [SerializeField] private GameObject[] mainMenuCanvas;
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

    [Title("F E E L", Shades.Orange)]
    [SerializeField] private MMFeedbacks blinkEffect;
    [SerializeField] private MMFeedbacks shieldBlinkEffect;

    [Title("SHIP SELECTOR")]
    [SerializeField] private Sprite[] ships;

    public enum Type
    {
        Base,
        Cannon,
        Homing1,
        Homing2,
        FlameThrower,
        MachineGun,
        Laser,
        Heart,
        Ice,
        Wide,
    }

    [SerializeField] private Type type;


    private void Awake()
    {
        playerHealthText = GetComponentInChildren<TMP_Text>();

        coins = FindAnyObjectByType<Coins_Manager>();
        renderer = GetComponent<SpriteRenderer>();
        shield = GameObject.Find("Shield");
    }

    void Start()
    {
        maxHealth = Mathf.Clamp(maxHealth, 1, targetHealth);

        score = Level_Manager.instance.currentLevel;

        mainCamera = Camera.main;
        currentBulletPrefab = defaultBulletPrefab;
        currentHealth = maxHealth;

        shield.gameObject.SetActive(false);
        currentBulletSpawnDuration = minBulletSpawnDuration;
        StartCoroutine(SpawnBullet());

        ChangeShip();
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
            mainMenuCanvas[0].SetActive(false);
            mainMenuCanvas[1].SetActive(false);
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
        ChangeLevel();
    }


    private void ChangeShip()
    {
        switch (type)
        {
            case Type.Cannon:

                renderer.sprite = ships[1];
                break;

            case Type.Homing1:

                renderer.sprite = ships[2];
                break;
            case Type.Homing2:

                renderer.sprite = ships[3];
                break;
            case Type.FlameThrower:

                renderer.sprite = ships[4];
                break;
            case Type.MachineGun:

                renderer.sprite = ships[5];
                break;
            case Type.Laser:

                renderer.sprite = ships[6];
                break;
            case Type.Heart:

                renderer.sprite = ships[7];
                break;
            case Type.Ice:

                renderer.sprite = ships[8];
                break;
            case Type.Wide:

                renderer.sprite = ships[9];
                break;
            default:
                break;
        }
    }


    private IEnumerator SpawnBullet()
    {
        while (true)
        {
            var projectile = Instantiate(currentBulletPrefab, bulletSpawnPointt[0].position, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().linearVelocity = bulletSpawnPointt[0].transform.forward * bulletSpeed * Time.deltaTime;
            projectile.AddForce(bulletSpawnPointt[0].up * bulletSpeed, ForceMode2D.Impulse);

            if (bulletSpread)
            {
                var projectile2 = Instantiate(currentBulletPrefab, bulletSpawnPointt[1].position, Quaternion.identity);
                projectile2.GetComponent<Rigidbody2D>().linearVelocity = bulletSpawnPointt[1].transform.forward * bulletSpeed * Time.deltaTime;
                projectile2.AddForce(bulletSpawnPointt[1].up * bulletSpeed, ForceMode2D.Impulse);

                var projectile3 = Instantiate(currentBulletPrefab, bulletSpawnPointt[2].position, Quaternion.identity);
                projectile3.GetComponent<Rigidbody2D>().linearVelocity = bulletSpawnPointt[2].transform.forward * bulletSpeed * Time.deltaTime;
                projectile3.AddForce(bulletSpawnPointt[2].up * bulletSpeed, ForceMode2D.Impulse);
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
                currentHealth += score;
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
        shieldBlinkEffect.PlayFeedbacks();
        yield return new WaitForSeconds(1.2F);
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
    public void Damage(float damgeAmount)    // IDamageable function 
    {
        Handheld.Vibrate();
        currentHealth -= damgeAmount;

        blinkEffect.PlayFeedbacks();

        if (currentHealth <= 0)
        {
            Handheld.Vibrate();
            Destroy(gameObject);
        }
    }


    public void ChangeLevel()
    {
        if (currentScore >= targetScore)
        {
            currentScore = 0;
            targetScore += 50;
            progressSlider.maxValue += 50;
            score++;
            Level_Manager.instance.AdvanceLevel();
        }
    }
}
