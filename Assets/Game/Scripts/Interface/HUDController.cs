using System.Collections;
using TMPro;
using UnityEngine;

namespace RELIC
{
    public class HUDController : MonoBehaviour
    {
        #region Field Declarations
        public static HUDController hudController;

        [SerializeField] private GameObject timerObject;
        private TextMeshProUGUI timer;

        [SerializeField] private GameObject timerBackgroundObject;
        private RectTransform timerBackgroundRectTransform;

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
            gameDuration = Mathf.FloorToInt(GameManager.gameManager.GameDuration);

            timerBackgroundRectTransform = timerBackgroundObject.GetComponent<RectTransform>();

            StartCoroutine(TickTimer());
        }
        #endregion

        #region Coroutines
        private IEnumerator TickTimer()
        {
            WaitForSeconds oneSecond = new WaitForSeconds(1f);

            Vector2 timerBackgroundPreferredSize;

            for (int currentGameTime = gameDuration; currentGameTime > 0; currentGameTime--)
            {
                timer.text = currentGameTime.ToString();
                timerBackgroundPreferredSize = timer.GetPreferredValues();
                timerBackgroundRectTransform.rect.Set(0f, 0f, timerBackgroundPreferredSize.x, timerBackgroundPreferredSize.y);

                yield return oneSecond;
            }
        }
        #endregion
    }
}