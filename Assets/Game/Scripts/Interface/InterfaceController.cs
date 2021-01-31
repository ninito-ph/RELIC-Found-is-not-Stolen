using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

namespace RELIC
{
    [RequireComponent(typeof(AudioSource))]
    public class InterfaceController : MonoBehaviour
    {
        #region Field Declarations

        // Singleton implementation
        public static InterfaceController interfaceController = null;

        // Dictionary containing all menus
        [Header("Menus")]
        [SerializeField] private string[] menuName;
        [SerializeField] private GameObject[] menuObject;
        private Dictionary<string, GameObject> menus = new Dictionary<string, GameObject>();
        
        // The sound receiver for menu sounds
        private AudioSource menuAudioSource;
        // Audio mixers
        [Header("Game Audio Mixer")]
        [SerializeField] private AudioMixer mainMixer;
        
        [Header("Lobby Parameters")] [SerializeField]
        private GameObject lobbyNumberOfPlayersObject;
        private TextMeshProUGUI lobbyNumberOfPlayers;

        [SerializeField] private GameObject lobbyStartGameControlsHelpTextObject;
        [SerializeField] private GameObject lobbyTextObject;

        // bool to store game state
        private bool gameIsPaused = false;

        #endregion

        #region Unity Methods

        void Start()
        {
            // Singleton initialization
            interfaceController = this;
            
            // Lobby initialization
            lobbyNumberOfPlayers = lobbyNumberOfPlayersObject.GetComponent<TextMeshProUGUI>();
            menuAudioSource = GetComponent<AudioSource>();
            
            // Links menu strings to menu objects
            for (int index = 0; index < menuObject.Length; index++)
            {
                menus.Add(menuName[index], menuObject[index]);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (gameIsPaused)
                {
                    menus["Pause"].gameObject.SetActive(false);
                    Time.timeScale = 1f;
                    gameIsPaused = false;
                }
                else
                {
                    menus["Pause"].gameObject.SetActive(true);
                    Time.timeScale = 0f;
                    gameIsPaused = true;
                }
            }
        }

        #endregion

        #region Public Methods

        // Public methods because Unity needs to be able to detect the method for button functionality

        // Switches to the desired menu
        public void SwitchToMenu(string desiredMenu)
        {
            // Sets the given menu active
            menus[desiredMenu].SetActive(true);

            // For each menu in the menus dictionary
            foreach (GameObject menu in menus.Values)
            {
                // Check if the menu is equal to the given menu
                if (menu != menus[desiredMenu])
                {
                    // If it isn't, disable it
                    menu.SetActive(false);
                }
            }

            PlayClickSound();
        }

        // Opens a menu without disabling the others
        public void OverlayMenu(string desiredMenu, bool mouseEnabled = true)
        {
            // Sets the given menu active
            menus[desiredMenu].SetActive(true);
        }

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
            SwitchToMenu("Background");
        }

        public void SetMaster(float sliderValue)
        {
            mainMixer.SetFloat("masterGroup", Mathf.Log10(sliderValue) * 20f);
        }
        
        public void SetMusic(float sliderValue)
        {
            mainMixer.SetFloat("musicGroup", Mathf.Log10(sliderValue) * 20f);
        }
        
        public void SetSFX(float sliderValue)
        {
            mainMixer.SetFloat("sfxGroup", Mathf.Log10(sliderValue) * 20f);
        }

        #endregion

        #region Private Methods

        // Gets UI volume and plays sound
        public void PlayClickSound()
        {
            menuAudioSource.Play();
        }

        #endregion
    }
}