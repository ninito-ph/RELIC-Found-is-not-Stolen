using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RELIC
{
    /// <summary>
    /// A controller script for splitscreen
    /// </summary>
    public class SplitscreenController : MonoBehaviour
    {
        #region Field Declarations
        private List<Camera> gameCameras = new List<Camera>();
        public static SplitscreenController splitscreenController;
        #endregion

        #region Unity Methods
        void Awake()
        {
            splitscreenController = this;
        }
        #endregion

        #region Public Methods
        public void GetCameras()
        {
            GameObject[] cameraObjects = GameObject.FindGameObjectsWithTag("MultiplayerCamera");

            foreach (GameObject cameraObject in cameraObjects)
            {
                gameCameras.Add(cameraObject.GetComponent<Camera>());
            }
        }

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
                    // Vertical double splitscreen coordinates for cameras
                    gameCameras[0].rect = new Rect(0.25175f, 0.5117f, 0.482f, 0.465f);
                    gameCameras[1].rect = new Rect(0.25175f, 0.023f, 0.482f, 0.465f);
                    // total border size (percentage) = 0.036
                    // total width border size (pixels) = 0.036 x 1980 = 71.28
                    // desired total height border size (pixels) = 71.28 / 1020 ~= 0.07
                    break;
                case 3:
                    // Three-way splitscreen coordinates for cameras
                    gameCameras[0].rect = new Rect(0.012f, 0.5117f, 0.482f, 0.465f);
                    gameCameras[1].rect = new Rect(0.506f, 0.5117f, 0.482f, 0.465f);
                    gameCameras[2].rect = new Rect(0.012f, 0.023f, 0.482f, 0.465f);
                    break;
                case 4:
                    // Four-way splitscreen coordinates for cameras
                    gameCameras[0].rect = new Rect(0.012f, 0.5117f, 0.482f, 0.465f);
                    gameCameras[1].rect = new Rect(0.506f, 0.5117f, 0.482f, 0.465f);
                    gameCameras[2].rect = new Rect(0.012f, 0.023f, 0.482f, 0.465f);
                    gameCameras[3].rect = new Rect(0.506f, 0.023f, 0.482f, 0.465f);
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
