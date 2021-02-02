using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace RELIC
{
    public class RelicController : MonoBehaviour
    {
        #region Field Declarations

        [Header("Relic Parameters")] [SerializeField]
        private float relicLifetime;

        [SerializeField] [Tooltip("How long the player must wait before picking the relic up")]
        private float pickupDelay = 1.3f;

        [Header("Effect Parameters")] [SerializeField]
        private Effects relicEffect;

        [FormerlySerializedAs("relicEffectIntensity")] [SerializeField]
        private float relicEffectModifier;

        [Header("Fluff")] [SerializeField] private GameObject pickupEffect;
        [SerializeField] private GameObject spawnEffect;
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private Vector3 rotationOffset;
        [SerializeField] private float rotationSpeed = 3f;

        [Tooltip("How many times the relic will blink before becoming pick-uppable")] [SerializeField]
        private int blinkAmount = 4;

        private bool canPickup = false;

        private Coroutine lifetimeRoutine;

        #endregion

        #region Properties

        public Effects RelicEffect
        {
            get => relicEffect;
            set => relicEffect = value;
        }

        public Vector3 RotationOffset
        {
            get => rotationOffset;
            set => rotationOffset = value;
        }

        #endregion

        #region MonoBehaviour Implementation

        private void Start()
        {
            transform.position += positionOffset;
            transform.rotation = Quaternion.Euler(rotationOffset.x, rotationOffset.y, rotationOffset.z);
            lifetimeRoutine = StartCoroutine(Lifetime());
            StartCoroutine(BlinkRelic());
        }

        private void Update()
        {
            transform.Rotate(new Vector3(0f, rotationSpeed * Time.deltaTime, 0f), Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            // If other isn't player or the relic can't be picked up yet
            if (other.CompareTag("Player") != true || canPickup == false) return;
            MotorController player = other.GetComponent<MotorController>();
            player.SetRelic(relicEffect, relicEffectModifier, gameObject);

            Instantiate(pickupEffect, transform.position, Quaternion.identity);
            StopAllCoroutines();
            gameObject.SetActive(false);
        }

        private void OnTriggerStay(Collider other)
        {
            // If other isn't player or the relic can't be picked up yet
            if (other.CompareTag("Player") != true || canPickup == false) return;
            MotorController player = other.GetComponent<MotorController>();

            if (Mathf.Approximately(player.StunTimer, 0f))
            {
                player.SetRelic(relicEffect, relicEffectModifier, gameObject);

                Instantiate(pickupEffect, transform.position, Quaternion.identity);
                StopAllCoroutines();
                gameObject.SetActive(false);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Re-starts a relic for reuse
        /// </summary>
        public void ReStart(Vector3 position)
        {
            gameObject.SetActive(true);
            canPickup = false;
            transform.position = position + positionOffset;
            transform.rotation = Quaternion.Euler(rotationOffset.x, rotationOffset.y, rotationOffset.z);
            lifetimeRoutine = StartCoroutine(Lifetime());
        }

        #endregion

        #region Coroutines

        private IEnumerator Lifetime()
        {
            // Shows spawn effect
            Instantiate(spawnEffect, transform.position, Quaternion.identity);

            yield return new WaitForSeconds(pickupDelay);
            canPickup = true;

            // Waits relic lifetime
            yield return new WaitForSeconds(Mathf.Max(1f, relicLifetime - pickupDelay));

            // Reuses spawn effect
            Instantiate(spawnEffect, transform.position, Quaternion.identity);

            // Destroys relic
            GameManager.gameManager.ReturnRelic(this);
        }

        private IEnumerator BlinkRelic()
        {
            // Gets the mesh renderer's material
            Material blinkMaterial = GetComponent<MeshRenderer>().material;
            int enableBlink = 0;
            WaitForSeconds interval = new WaitForSeconds((pickupDelay / blinkAmount));

            for (int blink = 0; blink <= blinkAmount; blink++)
            {
                blinkMaterial.SetInt("_Blink", enableBlink == 1 ? enableBlink = 0 : enableBlink = 1);
                yield return interval;
            }

            blinkMaterial.SetInt("_Blink", 0);
        }

        #endregion

        public enum Effects
        {
            None,
            Points,
            Speed,
            Dash,
            Shield,
            Stun,
            Trail
        }
    }
}