using System.Collections;
using UnityEngine;

namespace RELIC
{
    public class MotorController : MonoBehaviour
    {
        #region Field Declarations

        // Movement
        private CharacterController characterController;
        private Vector3 moveDirection = Vector3.zero;
        private Vector3 dashDirection = Vector3.zero;

        // Dash
        private bool dashReady = true;
        private bool dashActive = false;

        // Stun
        private float stunTimer = 0f;

        //Relic
        private RelicController.Effects activeRelicEffect = RelicController.Effects.None;
        private float relicEffectModifier;
        private GameObject relic;
        private Coroutine relicEffecTimer;

        // Other
        private int playerIndex;
        private bool shieldActive = false;

        // Inspector
        [Header("Movement Parameters")] [Tooltip("The distance a player can move normally.")] [SerializeField]
        private float speed = 1f;

        [Tooltip("The distance multiplier a player gets when dashing.")] [SerializeField]
        private float dashSpeedMultiplier = 3f;

        [Tooltip("The dash's duration.")] [SerializeField]
        private float dashDuration = 1f;

        [Tooltip("The cooldown after using a dash.")] [SerializeField]
        private float dashCooldown = 5f;

        [Tooltip("Should direction control during dashing be enabled?")] [SerializeField]
        private bool moveDuringDash = false;

        [Header("Active Relic Effects")] [SerializeField]
        private GameObject fireTrail;

        [SerializeField] [Tooltip("How long should the player wait before triggering the relic effect")]
        private float relicTickInterval = 1;

        [Header("Fluff")] [SerializeField] private AudioClip stunAudio;

        [SerializeField] private AudioClip dashAudio;
        [SerializeField] private AudioClip stealAudio;

        public int PlayerIndex
        {
            get => playerIndex;
            set => playerIndex = value;
        }

        public float StunTimer
        {
            get => stunTimer;
            set => stunTimer = value;
        }

        public bool DashActive
        {
            get => dashActive;
            set => dashActive = value;
        }

        public RelicController.Effects ActiveRelicEffect
        {
            get => activeRelicEffect;
            set => activeRelicEffect = value;
        }

        public float RelicEffectModifier
        {
            get => relicEffectModifier;
            set => relicEffectModifier = value;
        }

        public GameObject Relic
        {
            get => relic;
            set => relic = value;
        }

        #endregion

        #region Unity Methods

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            MovePlayer();
            Dash();
            TickStun();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                MotorController otherPlayer = other.gameObject.GetComponent<MotorController>();
                HandlePlayerCollision(otherPlayer);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the player's current Relic
        /// </summary>
        /// <param name="activeRelicEffect">The effect of the active Relic</param>
        /// <param name="relicEffectModifier">The numeric modifier for the relic's effect</param>
        /// <param name="relic">The relic's gameObject</param>
        public void SetRelic(RelicController.Effects activeRelicEffect, float relicEffectModifier, GameObject relic)
        {
            this.activeRelicEffect = activeRelicEffect;
            this.relicEffectModifier = relicEffectModifier;
            this.relic = relic;

            StartCoroutine(TickRelicEffect());
        }

        /// <summary>
        /// Stuns a player for a given amount of seconds
        /// </summary>
        /// <param name="stunAmount">The amount of seconds to stun for</param>
        public void Stun(float stunAmount)
        {
            if (dashActive == false && shieldActive == false)
            {
                stunTimer += stunAmount;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Robs a relic from a player
        /// </summary>
        private void RobRelic(MotorController player)
        {
            Instantiate(player.Relic, player.transform.position, Quaternion.identity);

            player.Relic = null;
            player.RelicEffectModifier = 0;
            player.ActiveRelicEffect = RelicController.Effects.None;
        }

        /// <summary>
        /// Moves the player accourding to his input
        /// </summary>
        private void MovePlayer()
        {
            // Defines the current input
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

            // Defines the current player direction
            moveDirection = movement.normalized;

            // If the dash is active disable (or not) the player influence on the movement
            if (dashActive == true)
            {
                if (moveDuringDash)
                {
                    movement = movement * dashSpeedMultiplier;
                }
                else
                {
                    movement = dashDirection * dashSpeedMultiplier;
                }
            }

            // Moves the character with the character controller, if the player is not stunned
            if (Mathf.Approximately(stunTimer, 0f))
            {
                if (activeRelicEffect == RelicController.Effects.Speed && dashActive == false)
                {
                    characterController.Move(movement * (speed * Time.deltaTime * relicEffectModifier));
                }
                else
                {
                    characterController.Move(movement * (speed * Time.deltaTime));
                }
            }
        }

        /// <summary>
        /// Gives a player a temporary boost in speed accourding to input and resource availability
        /// </summary>
        private void Dash()
        {
            if (dashReady == true && !Mathf.Approximately(Input.GetAxis("Fire1"), 0f) &&
                Mathf.Approximately(stunTimer, 0f))
            {
                StartCoroutine(ResolveDash(dashDuration, dashCooldown));
            }
        }

        /// <summary>
        /// Ticks down the stun timer
        /// </summary>
        private void TickStun()
        {
            stunTimer = Mathf.Max(0f, stunTimer - Time.deltaTime);
        }

        /// <summary>
        /// Handles collisions between players
        /// </summary>
        private void HandlePlayerCollision(MotorController otherPlayer)
        {
            Debug.Log("Taking place!");

            // Stuns player if other player has stun relic and is in a dash
            if (otherPlayer.DashActive && otherPlayer.ActiveRelicEffect == RelicController.Effects.Stun)
            {
                Stun(relicEffectModifier);
            }

            // Stuns other player if player is in a dash and has the stun relic
            if (activeRelicEffect == RelicController.Effects.Stun && dashActive)
            {
                otherPlayer.Stun(relicEffectModifier);
            }

            // Robs a relic if player is in dash and other player has a relic
            if (dashActive && otherPlayer.Relic != null)
            {
                RobRelic(otherPlayer);
            }
        }

        #endregion

        #region Coroutines

        /// <summary>
        /// Temporarily increases the speed of the player
        /// </summary>
        /// <param name="duration">How long the dash should last for</param>
        /// <param name="cooldown">How long the cooldown of the dash is</param>
        /// <returns></returns>
        private IEnumerator ResolveDash(float duration, float cooldown)
        {
            dashReady = false;
            dashActive = true;

            if (activeRelicEffect == RelicController.Effects.Shield)
            {
            }

            if (!moveDuringDash)
            {
                dashDirection = moveDirection;
            }

            yield return new WaitForSeconds(duration);

            dashActive = false;

            if (activeRelicEffect == RelicController.Effects.Dash)
            {
                yield return new WaitForSeconds(Mathf.Max(relicEffectModifier - duration, duration));
            }
            else
            {
                yield return new WaitForSeconds(cooldown - duration);
            }

            dashReady = true;
        }

        /// <summary>
        /// Runs a tick with a relic effect periodically
        /// </summary>
        /// <returns></returns>
        private IEnumerator TickRelicEffect()
        {
            WaitForSeconds interval = new WaitForSeconds(relicTickInterval);

            while (true)
            {
                switch (activeRelicEffect)
                {
                    default:
                        yield return null;
                        break;
                    case RelicController.Effects.Points:
                        //GameManager.Instance.AddPoints(playerIndex, relicEffectModifier);
                        break;
                    case RelicController.Effects.Trail:
                        if (dashActive == true)
                        {
                            Instantiate(fireTrail, transform.position, Quaternion.identity);
                        }

                        break;
                }

                yield return interval;
            }
        }

        #endregion
    }
}