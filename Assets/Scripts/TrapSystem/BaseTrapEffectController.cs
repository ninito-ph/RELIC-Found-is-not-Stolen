using System.Collections;
using UnityEngine;

namespace RELIC
{
    public abstract class BaseTrapEffectController : MonoBehaviour
    {
        #region Field Declarations
        [Header("Trap Effect Properties")]
        [Tooltip("The trap effect's type (Normal, Repeating or Toggle).")]
        [SerializeField] private ETrapEffectType effectType;
        [Tooltip("The trap effect's duration (only applies to non-Toggle traps).")]
        [SerializeField] private float effectDuration;
        [Tooltip("How many times a trap should repeat its effects (only applies to Repeating traps).")]
        [SerializeField] private int effectRepeatAmount;


        private bool trapIsActive = false;
        #endregion

        #region Custom Methods
        public void ActivateTrap()
        {
            switch(effectType)
            {
                case ETrapEffectType.Normal:
                    StartCoroutine(ResolveNormalEffect());
                    break;
                case ETrapEffectType.Repeating:
                    StartCoroutine(ResolveRepeatingEffect());
                    break;
                case ETrapEffectType.Toggle:
                    StartCoroutine(ResolveToggleEffect());
                    break;
            }
        }

        virtual protected void EnableEffect()
        {

        }

        virtual protected void DisableEffect()
        {

        }
        #endregion

        #region Coroutines
        private IEnumerator ResolveNormalEffect()
        {
            EnableEffect();

            yield return new WaitForSeconds(effectDuration);

            DisableEffect();
        }

        private IEnumerator ResolveRepeatingEffect()
        {
            WaitForSeconds repeatInterval = new WaitForSeconds(effectDuration / (float)effectRepeatAmount);

            for(int i = 0; i < effectRepeatAmount; i++)
            {
                EnableEffect();

                yield return repeatInterval;
            }

            DisableEffect();
        }

        private IEnumerator ResolveToggleEffect()
        {
            if(trapIsActive)
            {
                DisableEffect();
                trapIsActive = false;
            }

            else
            {
                EnableEffect();
                trapIsActive = true;
            }

            yield return null;
        }
        #endregion
    }
}
