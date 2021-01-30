using System.Collections;
using UnityEngine;

namespace RELIC
{
    public class MotorController : MonoBehaviour
    {
        #region Field Declarations

        // Movement
        private CharacterController characterController;
        private string movementHorizontalAxis;
        private string movementVerticalAxis;
        private string dashAxis;
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

        [Header("Player Properties")] [Tooltip("The player's character model.")] [SerializeField]
        private Transform playerModel;

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

        CharacterAnimation playerAnimation; 

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            movementHorizontalAxis = name + "Horizontal";
            movementVerticalAxis = name + "Vertical";
            dashAxis = name + "Dash";
            playerAnimation = playerModel.GetComponent<CharacterAnimation>();
        }

        private void Update()
        {
            MovePlayer();
            LookTowardsMovementDirection();
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
        /// Moves the player according to his input
        /// </summary>
        public void MovePlayer()
        {
            // Defines the current input
            Vector3 movement = new Vector3(Input.GetAxis(movementHorizontalAxis), 0f,
                Input.GetAxis(movementVerticalAxis));

            // Defines the current player direction
            if (movement != Vector3.zero)
            {
                moveDirection = movement.normalized;
            }
            else
            {
                playerAnimation.AnimateIdle();
            }
            

            // If the dash is active disable (or not) the player influence on the movement
            if (dashActive)
            {
                if (moveDuringDash)
                {
                    movement *= dashSpeedMultiplier;
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
                    Vector3 finalmovement = movement * speed * relicEffectModifier * Time.deltaTime;
                    if (finalmovement.magnitude > 0.01f)
                    {
                        playerAnimation.AnimateRun();
                    }
                    else
                    {
                        playerAnimation.AnimateStop();
                    }

                    characterController.Move(finalmovement);

                }
                else
                {
                    Vector3 finalmovement = movement * speed * Time.deltaTime;
                    if(finalmovement.magnitude > 0.01f)
                    {
                        playerAnimation.AnimateRun();
                    }
                    else
                    {
                        playerAnimation.AnimateStop();
                    }
                    characterController.Move(finalmovement);
                }
            }
            else
            {
                playerAnimation.AnimateFall();
            }

            characterController.Move(Vector3.down * 0.2f);
        }

        /// <summary>
        /// Rotates character so that it looks towards the movement direction
        /// </summary>
        void LookTowardsMovementDirection()
        {
            playerModel.LookAt(transform.position + moveDirection, Vector3.up);
        }

        /// <summary>
        /// Gives a player a temporary boost in speed according to input and resource availability
        /// </summary>
        public void Dash()
        {
            if (dashReady == true && Input.GetButtonDown(dashAxis) &&
                Mathf.Approximately(stunTimer, 0f))
            {
                StartCoroutine(ResolveDash());
                playerAnimation.AnimateDash();
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
        /// <returns></returns>
        private IEnumerator ResolveDash()
        {
            dashReady = false;
            dashActive = true;

            if (!moveDuringDash)
            {
                dashDirection = moveDirection;
            }

            yield return new WaitForSeconds(dashDuration);

            dashActive = false;

            if (activeRelicEffect == RelicController.Effects.Dash)
            {
                yield return new WaitForSeconds(Mathf.Max(relicEffectModifier - dashDuration, dashDuration));
            }
            else
            {
                yield return new WaitForSeconds(dashCooldown - dashDuration);
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