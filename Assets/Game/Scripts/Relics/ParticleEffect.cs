using UnityEngine;
using UnityEngine.Serialization;

namespace RELIC
{
    [RequireComponent(typeof(AudioSource), typeof(ParticleSystem))]
    public class ParticleEffect : MonoBehaviour
    {
        #region Field Declarations

        [Header("Effect Parameters")] private ParticleSystem particleEffect;

        private AudioSource audioSource;

        [SerializeField] [Range(0.01f, 2f)] private float audioPitchUpperBound = 1;
        [SerializeField] [Range(0.01f, 2f)] private float audioPitchLowerBound = 1;

        private Coroutine lifetimeRoutine;

        #endregion

        #region MonoBehaviour Implementation

        // Start is called before the first frame update
        void Start()
        {
            // Caches components
            particleEffect = GetComponent<ParticleSystem>();
            audioSource = GetComponent<AudioSource>();

            // Play sound effect and particle effect
            particleEffect.Play();
            audioSource.pitch = Random.Range(Mathf.Min(audioPitchLowerBound, audioPitchUpperBound),
                Mathf.Max(audioPitchLowerBound, audioPitchUpperBound));
            audioSource.Play();

            Destroy(gameObject, Mathf.Max(particleEffect.main.duration, audioSource.clip.length));
        }

        #endregion
    }
}