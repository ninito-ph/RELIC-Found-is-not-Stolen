using UnityEngine;

namespace RELIC {
    public abstract class BaseTrapActivationController : MonoBehaviour
    {
        #region Field Declarations
        [Header("Trap Activation Properties")]
        [Tooltip("The trap effect to be activated.")]
        [SerializeField] private BaseTrapEffectController trapEffectController;
        [Tooltip("The delay before activating a trap.")]
        [SerializeField] float trapActivationDelay;
        #endregion

        #region Unity Methods
        virtual protected void OnTriggerEnter(Collider collider)
        {
            if(collider.CompareTag("Player") && trapEffectController.gameObject.activeSelf) {
                trapEffectController.gameObject.SetActive(true);
            }
        }
        #endregion
    }
}
