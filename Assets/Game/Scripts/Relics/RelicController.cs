using System;
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

        private bool canPickup = false;

        private Coroutine lifetimeRoutine;

        #endregion

        #region MonoBehaviour Implementation

        private void Start()
        {
            transform.position += positionOffset;
            transform.rotation = Quaternion.Euler(rotationOffset.x, rotationOffset.y, rotationOffset.z);
            lifetimeRoutine = StartCoroutine(Lifetime());
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
            Destroy(gameObject);
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
            GameManager.gameManager.ReturnRelic(gameObject);
            Destroy(gameObject);
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