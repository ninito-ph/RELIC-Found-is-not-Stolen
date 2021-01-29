using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RELIC
{
    public class InputManager : MonoBehaviour
    {
        #region Field Declarations
        private int playerAmount = 0;

        public static InputManager inputManager;
        public static PlayerInputManager playerInputManager;
        #endregion

        #region Unity Methods
        void Awake() {
            inputManager = this;
            playerInputManager = GetComponent<PlayerInputManager>();
        }
        #endregion

        #region Custom Methods
        public void OnPlayerJoin(PlayerInput playerInput)
        {
            GameManager.gameManager.PlayerCount ++;
            playerInput.gameObject.name = "Player " + playerAmount;
            GameManager.gameManager.PlayerPrefabs.Add(playerInput.gameObject);
        }

        public void OnPlayerLeave(PlayerInput playerInput)
        {
            GameManager.gameManager.PlayerCount --;
        }
        #endregion
    }
}