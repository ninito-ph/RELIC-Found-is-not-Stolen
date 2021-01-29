using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RELIC
{
    public class RelicController : MonoBehaviour
    {
        #region Field Declarations

        [Header("Relic Parameters")]
        [SerializeField] private float relicLifetime;

        [Header("Effect Parameters")]
        [SerializeField] private Effects relicEffect;
        [SerializeField] private float relicEffectIntensity;
        
        [Header("Fluff")]
        [SerializeField] private AudioClip relicPickupSound;

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
            player.ActiveRelicEffect = relicEffect;
            player.RelicEffectModifier = relicEffectIntensity;
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

        #endregion
    }
}