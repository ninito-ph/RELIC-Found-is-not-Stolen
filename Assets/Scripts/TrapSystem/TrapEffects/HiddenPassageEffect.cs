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
        [Tooltip("The wall objects to be disabled/enabled according to the hidden passage's state.")]
        [SerializeField] private GameObject[] wallObjectsArray;
        #endregion

        #region Base Class Methods
        protected override void EnableEffect()
        {
            foreach(GameObject wallObject in wallObjectsArray)
            {
                wallObject.SetActive(false);
            }

            hiddenPassageObject.SetActive(true);
        }

        protected override void DisableEffect()
        {
            foreach(GameObject wallObject in wallObjectsArray)
            {
                wallObject.SetActive(true);
            }

            hiddenPassageObject.SetActive(false);
        }
        #endregion
    }
}