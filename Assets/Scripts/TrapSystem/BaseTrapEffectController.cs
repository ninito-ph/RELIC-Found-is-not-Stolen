using System.Collections;
using UnityEngine;

namespace RELIC {
    public abstract class BaseTrapEffectController : MonoBehaviour
    {
        #region Field Declarations
        [Header("Trap Effect Properties")]
        [Tooltip("The trap effect's type (DamageOverTime or InstantDamage).")]
        [SerializeField] private ETrapEffectType effectType;
        [Tooltip("The trap effect's duration.")]
        [SerializeField] private float effectDuration;
        [Tooltip("The trap effect's number of ticks (only applies to DamageOverTime traps).")]
        [SerializeField] private int effectTicks;
        [Tooltip("The trap effect's cooldown.")]
        [SerializeField] private float effectCooldown;
        #endregion

        #region Unity Methods
        virtual protected void OnEnable() {
            switch(effectType) {
                case ETrapEffectType.DamageOverTime:
                    ResolveEffect(effectDuration, effectCooldown);
                    break;

                case ETrapEffectType.InstantDamage:
                    ResolveEffectOverTime(effectDuration, effectTicks, effectCooldown);
                    break;
            }
        }
        #endregion

        #region Custom Methods
        virtual protected void EnableEffect() {

        }

        virtual protected void TickEffect() {

        }

        virtual protected void DisableEffect() {

        }
        #endregion

        #region Coroutines
        private IEnumerator ResolveEffect(float duration, float cooldown) {
            EnableEffect();

            yield return new WaitForSeconds(duration);

            DisableEffect();

            yield return new WaitForSeconds(cooldown - duration);

            this.gameObject.SetActive(false);
            //TODO: allow trap to be activated again (though SetActive might be enough)
        }

        private IEnumerator ResolveEffectOverTime(float duration, int ticks, float cooldown) {
            WaitForSeconds tickInterval = new WaitForSeconds(duration / (float)ticks);

            for(int i = 0; i < ticks; i++) {
                TickEffect();
                yield return tickInterval;
            }

            DisableEffect();

            yield return new WaitForSeconds(cooldown - duration);

            this.gameObject.SetActive(false);
            //TODO: allow trap to be activated again (though SetActive might be enough)
        }
        #endregion
    }
}
