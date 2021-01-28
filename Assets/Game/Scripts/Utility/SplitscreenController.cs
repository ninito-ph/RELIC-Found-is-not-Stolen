using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RELIC
{
    /// <summary>
    /// A controller script 
    /// </summary>
    public class SplitscreenController : MonoBehaviour
    {
        #region Field Declarations
        [Header("Cameras")]
        [Tooltip("The four cameras used in the game.")]
        [SerializeField]
        private Camera[] gameCameras;
        #endregion

        #region MonoBehaviour Implementation
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the amount of splitscreen cameras. Supports up to 4-way splitscreen.
        /// </summary>
        public void SetCameraAmount(int cameraAmount)
        {
            // Clamps cameraAmount to acceptable levels
            cameraAmount = (int)Mathf.Clamp((float)cameraAmount, 2f, 4f);

            switch (cameraAmount)
            {
                case 2:
                    // Enables and disables GameObjects
                    gameCameras[2].gameObject.SetActive(false);
                    gameCameras[3].gameObject.SetActive(false);

                    // Vertical double splitscreen coordinates for cameras
                    gameCameras[0].rect = new Rect(0f, 0f, 0.5f, 1f);
                    gameCameras[1].rect = new Rect(0.5f, 0f, 0.5f, 1f);
                    break;
                case 3:
                    // Enables and disables GameObjects
                    gameCameras[2].gameObject.SetActive(true);
                    gameCameras[3].gameObject.SetActive(false);

                    // Three-way splitscreen coordinates for cameras
                    gameCameras[0].rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
                    gameCameras[1].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                    gameCameras[2].rect = new Rect(0f, 0f, 0.5f, 0.5f);
                    break;
                case 4:
                    // Enables and disables GameObjects
                    gameCameras[2].gameObject.SetActive(true);
                    gameCameras[3].gameObject.SetActive(true);

                    // Four-way splitscreen coordinates for cameras
                    gameCameras[0].rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
                    gameCameras[1].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                    gameCameras[2].rect = new Rect(0f, 0f, 0.5f, 0.5f);
                    gameCameras[3].rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                    break;
                default:
                    Debug.LogError("Unsupported camera amount! Something went wrong, because this part of the code is supposed to be unreachable!");
                    break;
            }
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
