using System.Collections;
using System.Collections.Generic;
using Emp37.Utility;
using MoreMountains.Feedbacks;
using NUnit.Framework;
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
    public float currentHealth;

    [SerializeField] private float minBulletSpawnDuration = 0.2F;
    [SerializeField] private float maxBulletSpawnDuration = 0.1F;
    [SerializeField] private float currentBulletSpawnDuration;
    [SerializeField] private int numbersOfCoins;

    [Title("COMPONENT REF", Shades.Green)]
    [SerializeField] private Transform[] bulletSpawnPoints;
    [SerializeField] private Transform[] abilityProjectilePoints;
    [SerializeField] private CircleCollider2D magnetRange;

    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private Rigidbody2D defaultBullet;
    [SerializeField] private Rigidbody2D[] currentAbilityBullet;
    [SerializeField] private TMP_Text playerHealthText, coinsText, scoreText;
    [SerializeField] private GameObject bomb, shield, cannonBalls;
    [SerializeField] private GameObject[] mainMenuCanvas;
    [SerializeField] private Coins_Manager coins;


    [Title("M A C H I N E  G U N")]
    [SerializeField] private GameObject[] machineGunParticle;


    [Title("F L A M E  T H R O W E R")]
    [SerializeField] private ParticleSystem[] flameThrowerParticle;
    [SerializeField] private List<ParticleCollisionEvent> collisionEvent;


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
    [SerializeField] private MMFeedbacks healEffect;

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
        currentHealth = maxHealth;

        shield.gameObject.SetActive(false);
        currentBulletSpawnDuration = minBulletSpawnDuration;
        ChangeShip();
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


    public void heartHeal()
    {
        healEffect.PlayFeedbacks();
    }


    private void ChangeShip()
    {
        var trail = GetComponentInChildren<ParticleSystem>();

        switch (type)
        {
            case Type.Cannon:

                renderer.sprite = ships[1];
                cannonBalls.SetActive(true);
                ColorUtility.TryParseHtmlString("#6C6B68", out Color cannonColor);
                trail.startColor = cannonColor;
                break;

            case Type.Homing1:

                renderer.sprite = ships[2];
                ColorUtility.TryParseHtmlString("#4A663C", out Color homing1Color);
                StartCoroutine(SpawnAbilityBullet(3F, currentAbilityBullet[0], 5F));
                trail.startColor = homing1Color;
                break;
            case Type.Homing2:

                renderer.sprite = ships[3];
                ColorUtility.TryParseHtmlString("#C73538", out Color homing2Color);
                StartCoroutine(SpawnAbilityBullet(1F, currentAbilityBullet[1], 5F));
                trail.startColor = homing2Color;
                break;
            case Type.FlameThrower:

                renderer.sprite = ships[4];
                ColorUtility.TryParseHtmlString("#9B281A", out Color flameThrowerColor);
                flameThrowerParticle[0].gameObject.SetActive(true);
                flameThrowerParticle[1].gameObject.SetActive(true);
                trail.startColor = flameThrowerColor;
                break;
            case Type.MachineGun:

                renderer.sprite = ships[5];
                ColorUtility.TryParseHtmlString("#C73538", out Color machineGunColor);
                machineGunParticle[0].SetActive(true);
                machineGunParticle[1].SetActive(true);
                trail.startColor = machineGunColor;

                break;
            case Type.Laser:

                renderer.sprite = ships[6];
                ColorUtility.TryParseHtmlString("#3D5FAD", out Color laserColor);
                abilityProjectilePoints[0].rotation = Quaternion.Euler(0f, 0f, 25f);
                abilityProjectilePoints[1].rotation = Quaternion.Euler(0f, 0f, -25f);
                StartCoroutine(SpawnAbilityBullet(0.5F, currentAbilityBullet[3], 8F));
                trail.startColor = laserColor;
                break;
            case Type.Heart:

                renderer.sprite = ships[7];
                ColorUtility.TryParseHtmlString("#FA76E9", out Color heartColor);
                StartCoroutine(SpawnAbilityBullet(1F, currentAbilityBullet[4], 1.5F));
                trail.startColor = heartColor;
                break;
            case Type.Ice:

                renderer.sprite = ships[8];
                ColorUtility.TryParseHtmlString("#A4E0FE", out Color iceColor);
                StartCoroutine(SpawnAbilityBullet(1F, currentAbilityBullet[5], 7F));
                trail.startColor = iceColor;
                break;
            case Type.Wide:

                renderer.sprite = ships[9];
                ColorUtility.TryParseHtmlString("#FDD954", out Color wideColor);
                abilityProjectilePoints[0].rotation = Quaternion.Euler(0f, 0f, 25f);
                abilityProjectilePoints[1].rotation = Quaternion.Euler(0f, 0f, -25f);
                StartCoroutine(SpawnAbilityBullet(5F, currentAbilityBullet[2], 1F));
                trail.startColor = wideColor;
                break;
            default:
                ColorUtility.TryParseHtmlString("#FDD954", out Color baseColor);
                trail.startColor = baseColor;
                break;
        }
    }

    private IEnumerator SpawnBullet()
    {
        while (true)
        {
            var projectile = Instantiate(defaultBullet, bulletSpawnPoints[0].position, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().linearVelocity = bulletSpawnPoints[0].transform.forward * bulletSpeed * Time.deltaTime;
            projectile.AddForce(bulletSpawnPoints[0].up * bulletSpeed, ForceMode2D.Impulse);

            if (bulletSpread)
            {
                var projectile2 = Instantiate(defaultBullet, bulletSpawnPoints[1].position, Quaternion.identity);
                projectile2.GetComponent<Rigidbody2D>().linearVelocity = bulletSpawnPoints[1].transform.forward * bulletSpeed * Time.deltaTime;
                projectile2.AddForce(bulletSpawnPoints[1].up * bulletSpeed, ForceMode2D.Impulse);

                var projectile3 = Instantiate(defaultBullet, bulletSpawnPoints[2].position, Quaternion.identity);
                projectile3.GetComponent<Rigidbody2D>().linearVelocity = bulletSpawnPoints[2].transform.forward * bulletSpeed * Time.deltaTime;
                projectile3.AddForce(bulletSpawnPoints[2].up * bulletSpeed, ForceMode2D.Impulse);
            }
            yield return new WaitForSeconds(currentBulletSpawnDuration);
        }
    }

    private IEnumerator SpawnAbilityBullet(float spawnDuration, Rigidbody2D bulletType, float bulletSpeed)
    {
        while (true)
        {
            var projectile = Instantiate(bulletType, abilityProjectilePoints[0].position, Quaternion.LookRotation(abilityProjectilePoints[0].forward, abilityProjectilePoints[0].up));
            projectile.GetComponent<Rigidbody2D>().linearVelocity = abilityProjectilePoints[0].transform.forward * bulletSpeed * Time.deltaTime;
            projectile.AddForce(abilityProjectilePoints[0].up * bulletSpeed, ForceMode2D.Impulse);

            var projectile1 = Instantiate(bulletType, abilityProjectilePoints[1].position, Quaternion.LookRotation(abilityProjectilePoints[1].forward, abilityProjectilePoints[1].up));
            projectile1.GetComponent<Rigidbody2D>().linearVelocity = abilityProjectilePoints[1].transform.forward * bulletSpeed * Time.deltaTime;
            projectile1.AddForce(abilityProjectilePoints[1].up * bulletSpeed, ForceMode2D.Impulse);
            yield return new WaitForSeconds(spawnDuration);
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
