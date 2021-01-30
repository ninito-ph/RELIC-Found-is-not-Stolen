using System.Collections;
using UnityEngine;

namespace RELIC
{
    public class HiddenPassageEffect : BaseTrapEffectController
    {
        #region Field Declarations
        [Header("Hidden Passage Properties")]
        [Tooltip("The hidden passage object to be used.")]
        [SerializeField] private GameObject hiddenPassageObject;
        #endregion

        #region Base Class Methods
        protected override void EnableEffect()
        {
            hiddenPassageObject.SetActive(true);
        }

        protected override void DisableEffect()
        {
            hiddenPassageObject.SetActive(false);
        }
        #endregion
    }
}