using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace RELIC
{
    public class GameManager : MonoBehaviour
    {
        #region Field Declarations

        public static GameManager gameManager = null;

        [Header("Game Parameters")] [SerializeField]
        private float gameDuration = 120f;

        private int playerCount = 0;

        [Header("Item Parameters")] [SerializeField]
        private float itemRespawnTime = 10f;

        private int activeIdols = 0;
        private bool activeRelic = false;

        [Header("Spawn Parameters")] private List<GameObject> playerPrefabs = new List<GameObject>();
        [SerializeField] private GameObject vase;
        [SerializeField] private Transform[] playerSpawnPoints;

        [FormerlySerializedAs("playerRespawnDelay")] [SerializeField]
        private float playerRespawnStun = 5f;

        [SerializeField] private Transform[] vaseSpawnPoints;
        [SerializeField] private float vaseSpawnDelay;
        [SerializeField] private float spawnCheckRadius = 1f;
        private int[] playerScores = new int[4];

        [SerializeField] [Tooltip("The relics used in the game. Element 0 should always be at the ")]
        private GameObject[] spawnableRelics;

        private Coroutine spawnVaseRoutine;
        private Coroutine spawnPlayerRoutine;

        [Space] [Header("Game Events")] [SerializeField]
        private UnityEvent onGameStart;

        [SerializeField] private UnityEvent onGameEnd;

#if UNITY_EDITOR
        [Header("Debug")] [SerializeField] private bool enableSpawnZoneDisplay = false;
#endif

        #endregion

        #region Properties

        public List<GameObject> PlayerPrefabs
        {
            get => playerPrefabs;
            set => playerPrefabs = value;
        }

        public int[] PlayerScores
        {
            get => playerScores;
            set => playerScores = value;
        }

        public int PlayerCount
        {
            get => playerCount;
            set => playerCount = value;
        }

        public float GameDuration
        {
            get => gameDuration;
            set => gameDuration = value;
        }

        public Transform[] PlayerSpawnPoints
        {
            get => playerSpawnPoints;
            set => playerSpawnPoints = value;
        }

        public float PlayerRespawnStun
        {
            get => playerRespawnStun;
            set => playerRespawnStun = value;
        }

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
        public string GetWinnerScore()
        {
            // Gets the highest score using LINQ
            int largestScore = playerScores.Max();

            if (largestScore == 0)
            {
                return "Search for the relic!";
            }

            // Gets the index of the highest score using LINQ
            int winningPlayer = System.Array.IndexOf(playerScores, largestScore) + 1;

            string winnerString = "Player " + winningPlayer.ToString() + " has " + largestScore.ToString() + " points!";
            return winnerString;
        }

        /// <summary>
        /// Returns a relic to the spawnpool
        /// </summary>
        public void ReturnRelic(RelicController relic)
        {
            // Marks the the relic as inactive
            if (relic.RelicEffect == RelicController.Effects.Points)
            {
                activeRelic = false;
            }
            else
            {
                activeIdols++;
            }

            Destroy(relic.gameObject);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Starts the game by spawning all players and vases
        /// </summary>
        private void StartGame()
        {
            // Spawns players and vases
            spawnPlayerRoutine = StartCoroutine(SpawnPlayers());
            spawnVaseRoutine = StartCoroutine(SpawnVases());
            // Raises game start events
            onGameStart.Invoke();
        }

        /// <summary>
        /// Checks whether a given position is occupied.
        /// </summary>
        /// <param name="bitwiseLayermask">A bitwise layer mask</param>
        /// <param name="position">The position to check in</param>
        /// <param name="overlaps">The array used to store overlaps in</param>
        /// <returns>Whether the position is occupied or not by something in the given layer, at the given position</returns>
        private bool IsLocationFree(int bitwiseLayermask, Vector3 position, Collider[] overlaps)
        {
            return Physics.OverlapSphereNonAlloc(position, spawnCheckRadius, overlaps, bitwiseLayermask) > 0;
        }

        /// <summary>
        /// Randomly picks a relic from the spawnable relics array
        /// </summary>
        /// <returns>The relic GameObject</returns>
        private GameObject PickRelic()
        {
            // If the relic is not active, prioritize spawning it
            if (activeRelic == false)
            {
                activeRelic = true;
                return spawnableRelics[0];
            }

            // If there are not 4 items active, spawn them
            if (activeIdols < 4)
            {
                activeIdols++;
                return spawnableRelics[Random.Range(1, spawnableRelics.Length - 1)];
            }

            return null;
        }

        /// <summary>
        /// Generates and returns a random quaternion rotation
        /// </summary>
        /// <param name="rotateX">Whether to randomize rotation on the X axis</param>
        /// <param name="rotateY">Whether to randomize rotation on the Y axis</param>
        /// <param name="rotateZ">Whether to randomize rotation on the Z axis</param>
        /// <returns>A randomly rotated Quaternion</returns>
        private Quaternion GetRandomRotation(bool rotateX, bool rotateY, bool rotateZ)
        {
            // Declares random rotations
            float rotationX = 0f;
            float rotationY = 0f;
            float rotationZ = 0f;

            if (rotateX == true)
            {
                rotationX = Random.Range(0f, 360f);
            }

            if (rotateY == true)
            {
                rotationY = Random.Range(0f, 360f);
            }

            if (rotateZ == true)
            {
                rotationZ = Random.Range(0f, 360f);
            }

            return Quaternion.Euler(rotationX, rotationY, rotationZ);
        }

        #endregion

        #region Coroutines

        /// <summary>
        /// A game timer that waits the duration of the game and ends it
        /// </summary>
        private IEnumerator GameTimer()
        {
            // Waits match duration
            WaitForSeconds routineWait = new WaitForSeconds(gameDuration);
            yield return routineWait;

            // Notifies of game end
            onGameEnd.Invoke();
        }

        /// <summary>
        /// Spawns players
        /// </summary>
        private IEnumerator SpawnPlayers()
        {
            // Spawns all the players
            for (int index = 0; index < playerCount; index++)
            {
                playerPrefabs[index].transform
                    .SetPositionAndRotation(playerSpawnPoints[index].position, Quaternion.identity);
                playerPrefabs[index].SetActive(true);
                playerPrefabs[index].GetComponent<MotorController>().PlayerIndex = index;
            }

            yield break;
        }

        // what the fuck, this method is a fucking leviathan
        /// <summary>
        /// Spawns vases periodically in randomized spawnpoints.
        /// </summary>
        private IEnumerator SpawnVases()
        {
            // Caches WaitForSeconds object
            WaitForSeconds interval = new WaitForSeconds(vaseSpawnDelay);
            // Creates dump array to store SphereOverlapNonAlloc in
            Collider[] dumpArray = new Collider[10];
            // Caches LayerMask
            int bitwiseLayerMask = LayerMask.GetMask("Entities");

            // Saves the array in a modifiable list
            List<Transform> remainingPointsToSpawn = vaseSpawnPoints.ToList();

            while (true)
            {
                // Picks a random index in the remainingPointsToSpawn array
                int randomIndex = Random.Range(0, remainingPointsToSpawn.Count - 1);

                while (IsLocationFree(bitwiseLayerMask, remainingPointsToSpawn[randomIndex].position, dumpArray))
                {
                    // Removes occupied point from list
                    remainingPointsToSpawn.Remove(remainingPointsToSpawn[randomIndex]);

                    // Waits and refreshes the list before trying again
                    if (remainingPointsToSpawn.Count == 0)
                    {
                        yield return new WaitForSeconds(vaseSpawnDelay * 5f);
                        remainingPointsToSpawn = vaseSpawnPoints.ToList();
                    }

                    // Re-randomizes the spawnpoint index
                    randomIndex = Random.Range(0, remainingPointsToSpawn.Count - 1);

                    yield return null;
                }

                // Spawns vase and gives it a random relic
                VaseController vaseController = Instantiate(vase, remainingPointsToSpawn[randomIndex].position,
                    GetRandomRotation(false, true, false)).GetComponent<VaseController>();
                vaseController.ContainedItem = PickRelic();

                yield return interval;
            }
        }

        #endregion
    }
}