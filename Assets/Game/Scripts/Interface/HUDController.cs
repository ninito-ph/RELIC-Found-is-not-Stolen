using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace RELIC
{
    public class HUDController : MonoBehaviour
    {
        #region Field Declarations

        public static HUDController hudController;

        [SerializeField] private GameObject timerObject;
        [FormerlySerializedAs("scoreCount")] [SerializeField] private GameObject scoreCountObject;
        private TextMeshProUGUI timer;
        private TextMeshProUGUI scoreCount;

        private int gameDuration;

        #endregion

        #region Unity Methods

        void Awake()
        {
            hudController = this;
            gameObject.SetActive(false);
        }

        void Start()
        {
            timer = timerObject.GetComponent<TextMeshProUGUI>();
            scoreCount = scoreCountObject.GetComponent<TextMeshProUGUI>();
            gameDuration = Mathf.FloorToInt(GameManager.gameManager.GameDuration);

            StartCoroutine(UpdateHUD());
        }

        #endregion

        #region Coroutines

        private IEnumerator UpdateHUD()
        {
            WaitForSeconds oneSecond = new WaitForSeconds(1f);

            for (int currentGameTime = gameDuration; currentGameTime > 0; currentGameTime--)
            {
                timer.text = currentGameTime.ToString();
                scoreCount.text = GameManager.gameManager.GetWinnerScore();

                yield return oneSecond;
            }
        }

        #endregion
    }
}