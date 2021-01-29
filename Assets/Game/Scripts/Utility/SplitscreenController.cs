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
                    gameCameras[0].rect = new Rect(0f, 0f, 0.5f, 1f);
                    gameCameras[1].rect = new Rect(0.5f, 0f, 0.5f, 1f);
                    break;
                case 3:
                    // Three-way splitscreen coordinates for cameras
                    gameCameras[0].rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
                    gameCameras[1].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                    gameCameras[2].rect = new Rect(0f, 0f, 0.5f, 0.5f);
                    break;
                case 4:
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
