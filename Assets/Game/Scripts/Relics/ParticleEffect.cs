using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RELIC
{
    public class ParticleEffect : MonoBehaviour
    {
        #region Field Declarations

        [FormerlySerializedAs("particleSystem")] [SerializeField]
        private ParticleSystem particleEffect;

        private Coroutine lifetimeRoutine;

        #endregion

        #region MonoBehaviour Implementation

        // Start is called before the first frame update
        void Start()
        {
            particleEffect = GetComponent<ParticleSystem>();
            particleEffect.Play();
            StartCoroutine(Lifetime());
        }

        #endregion

        #region Coroutines

        private IEnumerator Lifetime()
        {
            yield return new WaitForSeconds(particleEffect.main.duration);
            Destroy(gameObject);
        }

        #endregion
    }
}