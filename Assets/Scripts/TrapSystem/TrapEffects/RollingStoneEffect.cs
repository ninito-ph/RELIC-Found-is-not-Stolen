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
        [Tooltip("The rolling stone's velocity.")]
        [SerializeField] private float rollingStoneVelocity = 1f;

        private Rigidbody rollingStoneRigidbody;
        private SphereCollider rollingStoneCollider;
        private Vector3 rollingStoneAddForcePosition;
        #endregion

        #region Unity Methods
        void Start()
        {
            rollingStoneRigidbody = rollingStoneObject.GetComponent<Rigidbody>();
            rollingStoneCollider = rollingStoneObject.GetComponent<SphereCollider>();
            rollingStoneAddForcePosition = rollingStoneInitialTransform.transform.position + Vector3.up * rollingStoneCollider.radius;
        }
        #endregion

        #region Base Class Methods
        protected override void EnableEffect()
        {
            rollingStoneObject.transform.SetPositionAndRotation(rollingStoneInitialTransform.position, rollingStoneInitialTransform.rotation);
            rollingStoneObject.SetActive(true);
            rollingStoneRigidbody.AddForceAtPosition(rollingStoneObject.transform.forward * rollingStoneVelocity, rollingStoneAddForcePosition);
        }
        #endregion
    }
}