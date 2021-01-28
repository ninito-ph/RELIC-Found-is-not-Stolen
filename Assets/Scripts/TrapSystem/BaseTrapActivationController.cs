using System.Collections;
using UnityEngine;

namespace RELIC
{
    public class BaseTrapActivationController : MonoBehaviour
    {
        #region Field Declarations
        [Header("Trap Activation Properties")]
        [Tooltip("The trap effect to be activated.")]
        [SerializeField] private BaseTrapEffectController trapEffectController;
        [Tooltip("The delay before activating a trap.")]
        [SerializeField] float trapActivationDelay;
        [Tooltip("The trap activator's cooldown.")]
        [SerializeField] private float trapActivationCooldown;

        private bool trapActivatorOnCooldown = false;
        #endregion

        #region Unity Methods
        virtual protected void OnTriggerEnter(Collider collider)
        {
            if(collider.CompareTag("Player") && !trapActivatorOnCooldown)
            {
                StartCoroutine(TriggerTrapActivation());
            }
        }
        #endregion

        #region Coroutines
        private IEnumerator TriggerTrapActivation()
        {
            yield return new WaitForSeconds(trapActivationDelay);

            trapEffectController.ActivateTrap();
            trapActivatorOnCooldown = true;

            yield return new WaitForSeconds(trapActivationCooldown - trapActivationDelay);

            trapActivatorOnCooldown = false;
        }
        #endregion
    }
}
