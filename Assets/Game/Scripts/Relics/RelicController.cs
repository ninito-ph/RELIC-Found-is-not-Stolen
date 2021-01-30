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

        [Header("Effect Parameters")] [SerializeField]
        private Effects relicEffect;

        [FormerlySerializedAs("relicEffectIntensity")] [SerializeField]
        private float relicEffectModifier;

        [Header("Fluff")]
        [SerializeField] private GameObject pickupEffect;
        [SerializeField] private GameObject spawnEffect;

        private Coroutine lifetimeRoutine;

        #endregion

        #region MonoBehaviour Implementation

        private void Start()
        {
            lifetimeRoutine = StartCoroutine(Lifetime());
        }

        private void OnTriggerEnter(Collider other)
        {
            // If other isn't player
            if (other.CompareTag("Player") != true) return;
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
            
            // Waits relic lifetime
            yield return new WaitForSeconds(relicLifetime);
            
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