using System;
using UnityEngine;


    public class Collectibles : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private SpriteRenderer sp;
        [SerializeField] private GameObject bomb;
        [SerializeField] private Sprite[] pickupsSprites;

        private Vector2 direction;

        public enum powerUps
        {
            SPower,   // max projectile 
            FPower,  // increase firerate 
            HPower, // increase health 
            BPower, // bomb 
            MPower, // Magnet
            XPower, // X2 Points
            OPower, // shield
        }

        public powerUps currentPowerUps;

        private void Awake()
        {
            sp = GetComponent<SpriteRenderer>();
        }


        private void Start()
        {
            //currentPowerUps = RandomPowerUps<powerUps>();
            currentPowerUps = GetPowerUpsBasedOnLevels();
        }

        private void Update()
        {
            if (currentPowerUps == powerUps.SPower)
            {
                sp.sprite = pickupsSprites[0];
            }
            if (currentPowerUps == powerUps.HPower)
            {
                sp.sprite = pickupsSprites[1];
            }
            if (currentPowerUps == powerUps.FPower)
            {
                sp.sprite = pickupsSprites[2];
            }
            if (currentPowerUps == powerUps.BPower)
            {
                sp.sprite = pickupsSprites[3];
            }
            if (currentPowerUps == powerUps.MPower)
            {
                sp.sprite = pickupsSprites[4];
            }
            if (currentPowerUps == powerUps.XPower)
            {
                sp.sprite = pickupsSprites[5];
            }
            if (currentPowerUps == powerUps.OPower)
            {
                sp.sprite = pickupsSprites[6];
            }
        }

        private void LateUpdate()
        {
            transform.Translate(0F, -speed * Time.deltaTime, 0F);
        }

        private T RandomPowerUps<T>()
        {
            Array values = Enum.GetValues(typeof(T));
            int randomIndex = UnityEngine.Random.Range(0, values.Length);
            return (T)values.GetValue(randomIndex);

        }

        private powerUps GetPowerUpsBasedOnLevels()
        {
            int level = Level_Manager.instance.currentLevel;
            powerUps[] allowedTypes;

            if (level == 1)
            {
                allowedTypes = new powerUps[] { powerUps.SPower, powerUps.HPower };
            }
            else if (level >= 3)
            {
                allowedTypes = new powerUps[] { powerUps.SPower, powerUps.HPower, powerUps.FPower };
            }
            else if (level >= 9)
            {
                allowedTypes = new powerUps[] { powerUps.SPower, powerUps.HPower, powerUps.FPower, powerUps.OPower };
            }
            else if (level >= 12)
            {
                allowedTypes = new powerUps[] { powerUps.SPower, powerUps.HPower, powerUps.FPower, powerUps.OPower, powerUps.MPower };
            }
            else if (level >= 15)
            {
                allowedTypes = new powerUps[] { powerUps.SPower, powerUps.HPower, powerUps.FPower, powerUps.OPower, powerUps.MPower, powerUps.XPower };
            }
            else if (level >= 20)
            {
                allowedTypes = new powerUps[] { powerUps.SPower, powerUps.HPower, powerUps.FPower, powerUps.OPower, powerUps.MPower, powerUps.XPower, powerUps.BPower };
            }
            else
            {
                allowedTypes = (powerUps[])Enum.GetValues(typeof(powerUps));
            }
            int randomIndex = UnityEngine.Random.Range(0, allowedTypes.Length);
            return allowedTypes[randomIndex];
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var playerRef = collision.GetComponent<Player_Controller>();
            if (collision.gameObject.CompareTag("Player"))
            {
                Destroy(gameObject);
            }
        }
    }
