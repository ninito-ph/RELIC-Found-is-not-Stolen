using TMPro;
using UnityEngine;

namespace RELIC
{
    public class InterfaceController : MonoBehaviour
    {
        #region Field Declarations
        public static InterfaceController interfaceController = null;

        [SerializeField] private GameObject lobbyNumberOfPlayersObject;
        private TextMeshProUGUI lobbyNumberOfPlayers;

        [SerializeField] private GameObject lobbyStartGameControlsHelpTextObject;

        [SerializeField] private GameObject lobbyTextObject;
        #endregion

        #region Unity Methods
        void Start()
        {
            interfaceController = this;
            lobbyNumberOfPlayers = lobbyNumberOfPlayersObject.GetComponent<TextMeshProUGUI>();
        }
        #endregion

        #region Custom Methods
        public void UpdateNumberOfPlayers(int numberOfPlayers)
        {
            lobbyNumberOfPlayers.text = "Players: " + numberOfPlayers.ToString();
            CheckIfThereAreEnoughPlayers(numberOfPlayers);
        }

        public void CheckIfThereAreEnoughPlayers(int numberOfPlayers)
        {
            if (numberOfPlayers > 1)
            {
                lobbyStartGameControlsHelpTextObject.SetActive(true);
            }
            else
            {
                lobbyStartGameControlsHelpTextObject.SetActive(false);
            }
        }

        public void StartGame()
        {
            lobbyTextObject.SetActive(false);
            HUDController.hudController.gameObject.SetActive(true);
        }
        #endregion
    }
}
