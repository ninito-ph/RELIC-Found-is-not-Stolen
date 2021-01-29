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
            playerAmount ++;
            playerInput.gameObject.name = "Player " + playerAmount;
        }

        public void OnPlayerLeave(PlayerInput playerInput)
        {
            playerAmount --;
        }
        #endregion
    }
}