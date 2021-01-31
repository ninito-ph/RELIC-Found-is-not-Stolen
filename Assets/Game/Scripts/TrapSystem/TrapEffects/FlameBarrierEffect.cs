using System.Collections;
using UnityEngine;

namespace RELIC
{
    public class FlameBarrierEffect : BaseTrapEffectController
    {
        #region Field Declarations
        [Header("Flame Barrier Properties")]
        [Tooltip("The flame barrier object to be used.")]
        [SerializeField] private GameObject flameBarrierObject;
        #endregion

        #region Base Class Methods
        protected override void EnableEffect()
        {
            flameBarrierObject.SetActive(true);
        }

        protected override void DisableEffect()
        {
            flameBarrierObject.SetActive(false);
        }
        #endregion
    }
}