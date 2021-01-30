using System.Collections.Generic;
using UnityEngine;

namespace RELIC
{
    public class LobbyManager : MonoBehaviour
    {
        #region Field Declarations
        [Header("Lobby Properties")]
        [Tooltip("The player prefab to be used.")]
        [SerializeField] private GameObject playerPrefab;
        #endregion

        #region Unity Methods
        void Update() {
            PlayerJoin();
            PlayerLeave();
            StartGame();
        }
        #endregion

        #region Custom Methods
        private void PlayerJoin()
        {
            if(Input.GetButtonDown("Join") && GameManager.gameManager.PlayerCount < 4)
            {
                GameManager.gameManager.PlayerCount ++;
                InterfaceController.interfaceController.UpdateNumberOfPlayers(GameManager.gameManager.PlayerCount);
            }
        }

        private void PlayerLeave()
        {
            if (Input.GetButtonDown("Leave") && GameManager.gameManager.PlayerCount > 0)
            {
                GameManager.gameManager.PlayerCount --;
                InterfaceController.interfaceController.UpdateNumberOfPlayers(GameManager.gameManager.PlayerCount);
            }
        }

        private void StartGame()
        {
            if (Input.GetButton("Start") && GameManager.gameManager.PlayerCount > 1)
            {
                for (int i = 0; i < GameManager.gameManager.PlayerCount; i++)
                {
                    GameObject newPlayer = Instantiate(playerPrefab);
                    newPlayer.name = "Player" + i;
                    GameManager.gameManager.PlayerPrefabs.Add(newPlayer);
                }

                GameManager.gameManager.gameObject.SetActive(true);

                InterfaceController.interfaceController.StartGame();

                gameObject.SetActive(false);
            }
        }
        #endregion
    }
}
