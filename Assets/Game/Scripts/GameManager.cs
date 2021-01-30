using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RELIC
{
    public class GameManager : MonoBehaviour
    {
        #region Field Declarations
        public static GameManager gameManager = null;

        [Header("Game Parameters")]
        [SerializeField] private float gameDuration = 120f;
        private int playerCount = 0;

        [Header("Item Parameters")]
        [SerializeField] private float itemRespawnTime = 10f;
        private int activeIdols = 0;
        private bool activeRelic = false;
        [SerializeField] private GameObject vase;

        [Header("Spawn Parameters")]
        private List<GameObject> playerPrefabs = new List<GameObject>();
        [SerializeField] private Transform[] playerSpawnPoints;
        [SerializeField] private Transform[] vaseSpawnPoints;
        [SerializeField] private float spawnCheckRadius = 1f;
        private int[] playerScores = new int[4];

        private Coroutine spawnVaseRoutine;
        private Coroutine spawnPlayerRoutine;
        [Space]

        [Header("Game Events")]
        [SerializeField] private UnityEvent onGameStart;
        [SerializeField] private UnityEvent onGameEnd;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool enableSpawnZoneDisplay = false;
#endif
        #endregion

        #region Properties
        public List<GameObject> PlayerPrefabs { get => playerPrefabs; set => playerPrefabs = value; }
        public int[] PlayerScores { get => playerScores; set => playerScores = value; }
        public int PlayerCount { get => playerCount; set => playerCount = value; }
        #endregion

        #region MonoBehaviour Implementation
        // Sets up the game before start
        private void Awake()
        {
            gameManager = this;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            StartGame();
            SplitscreenController.splitscreenController.GetCameras();
            SplitscreenController.splitscreenController.SetCameraAmount(playerCount);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Draws spheres in the spawn zones
            if (enableSpawnZoneDisplay == true)
            {
                Gizmos.color = new Color(0f, 1f, 0f, 0.33f);
                foreach (Transform transform in playerSpawnPoints)
                {
                    Gizmos.DrawSphere(transform.position, spawnCheckRadius);
                }

                Gizmos.color = new Color(0f, 0f, 1f, 0.33f);
                foreach (Transform transform in vaseSpawnPoints)
                {
                    Gizmos.DrawSphere(transform.position, spawnCheckRadius);
                }
            }
        }
#endif
        #endregion

    #region Public Methods
    /// <summary>
    /// Adds score to a given player
    /// </summary>
    /// <param name="playerIndex">The index of the desired player</param>
    /// <param name="score">The score of the desired player</param>
    public void AddScore(int playerIndex, int score)
    {
        playerScores[playerIndex] += score;
    }

        /// <summary>
        /// Generates a string with the winner's name and his score
        /// </summary>
        /// <returns>A string with the winner's name and his score</returns>
        public string GetWinnerString()
        {
            // Gets the highest score using LINQ
            int largestScore = playerScores.Max();
            // Gets the index of the highest score using LINQ
            int winningPlayer = System.Array.IndexOf(playerScores, largestScore) + 1;

            string winnerString = "The winner is Player " + winningPlayer + "! \n" + largestScore + " points earned";
            return winnerString;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Starts the game by spawning all players and vases
        /// </summary>
        private void StartGame()
        {
            spawnPlayerRoutine = StartCoroutine(SpawnPlayers());

            // Spawns vases
            spawnVaseRoutine = StartCoroutine(SpawnVases());
        }

        /// <summary>
        /// Spawns an object in a random point
        /// </summary>
        private GameObject SpawnInRandomPoint(Transform[] randomPoints, GameObject objectToSpawn)
        {
            // 9 is the largest amount of entities theoretically possible to be in a spawn zone (4 players + 5 dropped relics)
            Collider[] overlaps = new Collider[9];

            Vector3 randomPoint = randomPoints[(int)Random.Range(0f, randomPoints.Length - 1)].position;

            // Bitwise int
            int entityMask = LayerMask.GetMask("Entities");

            // This could possibly end up in an infinite loop, and its actually very bad performance-wise
            while (Physics.OverlapSphereNonAlloc(randomPoint, spawnCheckRadius, overlaps, entityMask) > 0)
            {
                randomPoint = randomPoints[(int)Random.Range(0f, randomPoints.Length - 1)].position;
            }

            objectToSpawn.transform.SetPositionAndRotation(randomPoint, Quaternion.identity);
            return objectToSpawn;
        }
        #endregion

        #region Coroutines
        /// <summary>
        /// A game timer that waits the duration of the game and ends it
        /// </summary>
        /// <param name="gameDuration">How long the game should endure</param>
        private IEnumerator GameTimer()
        {
            // Notifies of game start
            onGameStart.Invoke();

            // Waits match duration
            WaitForSeconds routineWait = new WaitForSeconds(gameDuration);
            yield return routineWait;

            // Notifies of game end
            onGameEnd.Invoke();
        }

    /// <summary>
    /// Spawns players in a random point
    /// </summary>
    private IEnumerator SpawnPlayers()
    {
        // Spawns all the players
        for (int index = 0; index < playerCount; index++)
        {
            GameObject player = SpawnInRandomPoint(playerSpawnPoints, PlayerPrefabs[index]);
            player.GetComponent<MotorController>().PlayerIndex = index;
            spawnedPlayer.name = "Player" + index;
        }

            yield break;
        }

        /// <summary>
        /// Spawns vases with items inside
        /// </summary>
        /// <returns></returns>
        private IEnumerator SpawnVases()
        {
            WaitForSeconds spawnInterval = new WaitForSeconds(itemRespawnTime);

            while (true)
            {
                if (activeRelic == false)
                {
                    GameObject relicVase = SpawnInRandomPoint(vaseSpawnPoints, vase);
                    // VaseController relicVaseController = relicVase.GetComponent<VaseController>();
                    // relicVaseController.ContainedItem = gameItems.Relic;

                    activeRelic = true;
                }

                // Spawns idols until all idols are active
                for (int index = activeIdols; index < 4; index++)
                {
                    GameObject idolVase = SpawnInRandomPoint(vaseSpawnPoints, vase);
                    // VaseController idolVaseController = idolVaseController.GetComponent<VaseController>();
                    // idolVaseController.ContainedItem = gameItems.RandomIdol;

                    activeIdols++;
                }

                yield return spawnInterval;
            }
        }
        #endregion
    }
}
