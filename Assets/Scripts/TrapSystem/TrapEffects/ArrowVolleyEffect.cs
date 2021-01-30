using System.Collections;
using UnityEngine;

namespace RELIC
{
    public class ArrowVolleyEffect : BaseTrapEffectController
    {
        #region Field Declarations
        [Header("Arrow Volley Properties")]
        [Tooltip("The arrow volley object to be used.")]
        [SerializeField] private GameObject arrowVolleyObject;
        [Tooltip("The arrow volley's initial position and rotation.")]
        [SerializeField] private Transform arrowVolleyInitialTransform;
        [Tooltip("The arrow volley's velocity.")]
        [SerializeField] private float arrowVelocity = 1f;
        [Tooltip("The arrow volley's lifetime.")]
        [SerializeField] private float arrowLifetime = 1f;

        private Rigidbody arrowVolleyRigidbody;
        #endregion

        #region Unity Methods
        void Start()
        {
            arrowVolleyRigidbody = arrowVolleyObject.GetComponent<Rigidbody>();
        }
        #endregion

        #region Base Class Methods
        protected override void EnableEffect()
        {
            arrowVolleyObject.transform.SetPositionAndRotation(arrowVolleyInitialTransform.position, arrowVolleyInitialTransform.rotation);
            arrowVolleyObject.SetActive(true);
            arrowVolleyRigidbody.velocity = arrowVolleyObject.transform.forward * arrowVelocity;
            StartCoroutine(DisableArrow());
        }
        #endregion

        #region Coroutines
        private IEnumerator DisableArrow()
        {
            yield return new WaitForSeconds(arrowLifetime);

            arrowVolleyObject.SetActive(false);
        }
        #endregion
    }
}
