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

        [Header("Fluff")] [SerializeField] private AudioClip relicPickupSound;

        private Coroutine lifetimeRoutine;

        #endregion

        #region MonoBehaviour Implementation

        private void Start()
        {
            lifetimeRoutine = StartCoroutine(Lifetime());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") != true) return;
            MotorController player = other.GetComponent<MotorController>();
            player.SetRelic(relicEffect, relicEffectModifier, gameObject);
            AudioSource.PlayClipAtPoint(relicPickupSound, transform.position);

            Destroy(gameObject);
        }

        #endregion

        #region Coroutines

        private IEnumerator Lifetime()
        {
            yield return new WaitForSeconds(relicLifetime);
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