using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RELIC
{
    public class LobbyManager : MonoBehaviour
    {
        #region Field Declarations

        [FormerlySerializedAs("playerPrefab")] [Header("Lobby Properties")] [Tooltip("The player prefab to be used.")] [SerializeField]
        private GameObject[] playerCharacters;

        #endregion

        #region Unity Methods

        void Update()
        {
            PlayerJoin();
            PlayerLeave();
            StartGame();
        }

        #endregion

        #region Custom Methods

        private void PlayerJoin()
        {
            if (Input.GetButtonDown("Join") && GameManager.gameManager.PlayerCount < 4)
            {
                GameManager.gameManager.PlayerCount++;
                InterfaceController.interfaceController.UpdateNumberOfPlayers(GameManager.gameManager.PlayerCount);
            }
        }

        private void PlayerLeave()
        {
            if (Input.GetButtonDown("Leave") && GameManager.gameManager.PlayerCount > 0)
            {
                GameManager.gameManager.PlayerCount--;
                InterfaceController.interfaceController.UpdateNumberOfPlayers(GameManager.gameManager.PlayerCount);
            }
        }

        private void StartGame()
        {
            if (Input.GetButtonDown("Start") && GameManager.gameManager.PlayerCount > 1)
            {
                for (int index = 0; index < GameManager.gameManager.PlayerCount; index++)
                {
                    GameObject newPlayer = Instantiate(playerCharacters[index]);
                    newPlayer.name = "Player" + index.ToString();
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