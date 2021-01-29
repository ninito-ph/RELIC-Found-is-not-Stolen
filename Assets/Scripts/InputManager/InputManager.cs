using UnityEngine;
using UnityEngine.InputSystem;

namespace RELIC
{
    public class InputManager : MonoBehaviour
    {
        #region Field Declarations
        private int playerAmount = 0;
        #endregion

        #region Custom Methods
        public void OnPlayerJoin(PlayerInput playerInput)
        {
            GameManager.PlayerCount ++;
            playerInput.gameObject.name = "Player " + playerAmount;
            playerInput.transform.SetParent(GameManager.PlayerPrefabs);
        }

        public void OnPlayerLeave(PlayerInput playerInput)
        {
            GameManager.PlayerCount --;
        }
        #endregion
    }
}