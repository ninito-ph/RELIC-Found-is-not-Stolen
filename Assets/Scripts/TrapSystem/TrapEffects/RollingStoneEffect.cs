using UnityEngine;

namespace RELIC
{
    public class RollingStoneEffect : BaseTrapEffectController
    {
        #region Field Declarations
        [Header("Rolling Stone Properties")]
        [Tooltip("The rolling stone object to be used.")]
        [SerializeField] private GameObject rollingStoneObject;
        [Tooltip("The rolling stone's initial position and rotation.")]
        [SerializeField] private Transform rollingStoneInitialTransform;
        [Tooltip("The rolling stone's torque.")]
        [SerializeField] private float rollingStoneTorque = 1f;

        private Rigidbody rollingStoneRigidbody;
        #endregion

        #region Unity Methods
        void Start()
        {
            rollingStoneRigidbody = rollingStoneObject.GetComponent<Rigidbody>();
        }
        #endregion

        #region Base Class Methods
        protected override void EnableEffect()
        {
            rollingStoneObject.transform.SetPositionAndRotation(rollingStoneInitialTransform.position, rollingStoneInitialTransform.rotation);
            rollingStoneObject.SetActive(true);
            rollingStoneRigidbody.AddTorque(rollingStoneObject.transform.forward * rollingStoneTorque);
        }
        #endregion
    }
}