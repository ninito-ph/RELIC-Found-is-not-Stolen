using System.Collections;
using UnityEngine;

namespace RELIC
{
    public class SpikeEffect : BaseTrapEffectController
    {
        #region Field Declarations
        [Header("Spike Properties")]
        [Tooltip("The spike object to be used.")]
        [SerializeField] private GameObject spikeObject;
        [Tooltip("The spike's position and rotation when disabled.")]
        [SerializeField] private Transform spikeDisabledTransform;
        [Tooltip("The spike's position offset when enabled.")]
        [SerializeField] private Vector3 spikeEnabledOffset;
        [Tooltip("The spike's enable/disable animation duration.")]
        [SerializeField] private float spikeAnimationDuration = 1f;

        private Vector3 spikeDisabledPosition;
        private Vector3 spikeEnabledPosition;
        #endregion

        #region Unity Methods
        void Start()
        {
            spikeDisabledPosition = spikeDisabledTransform.position;
            spikeEnabledPosition = spikeDisabledPosition + spikeEnabledOffset;
        }
        #endregion

        #region Base Class Methods
        protected override void EnableEffect()
        {
            StartCoroutine(AnimateSpikeMovement(spikeDisabledPosition, spikeEnabledPosition));
        }

        protected override void DisableEffect()
        {
            StartCoroutine(AnimateSpikeMovement(spikeEnabledPosition, spikeDisabledPosition));
        }
        #endregion

        #region Coroutines
        private IEnumerator AnimateSpikeMovement(Vector3 initialPosition, Vector3 finalPosition)
        {
            float t = 0f;
            while (t <= 1f)
            {
                spikeObject.transform.position = Vector3.Lerp(initialPosition, finalPosition, t);

                float tIncrease = Time.deltaTime / spikeAnimationDuration;

                t += tIncrease;

                yield return new WaitForSeconds(tIncrease);
            }
        }
        #endregion
    }
}