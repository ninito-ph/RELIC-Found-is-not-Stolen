using System.Collections;
using UnityEngine;

namespace RELIC
{
    public class BaseTrapActivationController : MonoBehaviour
    {
        #region Field Declarations
        [Header("Trap Button Animation Properties")]
        [Tooltip("The trap button object to be used.")]
        [SerializeField] private GameObject trapButtonObject;
        [Tooltip("The trap button's position and rotation when ready to be pressed.")]
        [SerializeField] private Transform trapButtonReadyTransform;
        [Tooltip("The trap button's position offset when pressed.")]
        [SerializeField] private Vector3 trapButtonPressedOffset;
        [Tooltip("The trap button's enable/disable animation duration.")]
        [SerializeField] private float trapButtonAnimationDuration = 1f;

        private Vector3 trapButtonReadyPosition;
        private Vector3 trapButtonPressedPosition;

        [Header("Trap Activation Properties")]
        [Tooltip("The trap effect to be activated.")]
        [SerializeField] private BaseTrapEffectController trapEffectController;
        [Tooltip("The delay before activating a trap.")]
        [SerializeField] float trapActivationDelay;
        [Tooltip("The trap activator's cooldown.")]
        [SerializeField] private float trapActivationCooldown;
        [Tooltip("The trap activator's effect when pressed.")]
        [SerializeField] private GameObject trapActivationEffectObject;
        [Tooltip("The trap activator's effect duration.")]
        [SerializeField] private float trapActivationEffectDuration;

        private AudioSource audioSource;

        private bool trapActivatorOnCooldown = false;
        #endregion

        #region Unity Methods
        void Start()
        {
            trapButtonReadyPosition = trapButtonReadyTransform.position;
            trapButtonPressedPosition = trapButtonReadyPosition + trapButtonPressedOffset;

            audioSource = GetComponent<AudioSource>();
        }

        virtual protected void OnTriggerEnter(Collider collider)
        {
            if(collider.CompareTag("Player") && !trapActivatorOnCooldown)
            {
                trapActivatorOnCooldown = true;
                StartCoroutine(TriggerTrapActivation());
            }
        }
        #endregion

        #region Coroutines
        private IEnumerator TriggerTrapActivation()
        {
            audioSource.Play();
//            trapActivationEffectObject.SetActive(true);

            StartCoroutine(AnimateButtonPressed());
//            StartCoroutine(DisableTrapActivationEffect());

            yield return new WaitForSeconds(trapActivationDelay);

            trapEffectController.ActivateTrap();

            yield return new WaitForSeconds(trapActivationCooldown - trapActivationDelay);

            StartCoroutine(AnimateButtonReady());
        }

        private IEnumerator AnimateButtonPressed()
        {
            float t = 0f;

            while (t <= 1f)
            {
                trapButtonObject.transform.position = Vector3.Lerp(trapButtonReadyPosition, trapButtonPressedPosition, t);

                float tIncrease = Time.deltaTime / trapButtonAnimationDuration;

                t += tIncrease;

                yield return new WaitForSeconds(tIncrease);
            }
        }

        private IEnumerator AnimateButtonReady()
        {
            float t = 0f;

            while (t <= 1f)
            {
                trapButtonObject.transform.position = Vector3.Lerp(trapButtonPressedPosition, trapButtonReadyPosition, t);

                float tIncrease = Time.deltaTime / trapButtonAnimationDuration;

                t += tIncrease;

                yield return new WaitForSeconds(tIncrease);
            }

            trapActivatorOnCooldown = false;
        }

        private IEnumerator DisableTrapActivationEffect()
        {
            yield return new WaitForSeconds(trapActivationEffectDuration);

            trapActivationEffectObject.SetActive(false);
        }
        #endregion
    }
}
