using System;
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

        [Header("Active Relic Effects")]
        private GameObject fireTrail;

        [Header("Fluff")] [SerializeField] private AudioClip stunAudio;
        [SerializeField] private AudioClip dashAudio;
        [SerializeField] private AudioClip stealAudio;

        public int PlayerIndex { get; }

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

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                MotorController otherPlayer = other.gameObject.GetComponent<MotorController>();

                // Stuns player if other player has stun relic and is in a dash
                if (otherPlayer.DashActive == true && otherPlayer.ActiveRelicEffect == RelicController.Effects.Stun &&
                    dashActive == false)
                {
                    stunTimer = otherPlayer.RelicEffectModifier;
                }

                // Stuns other player if player is in a dash and has the stun relic
                if (activeRelicEffect == RelicController.Effects.Stun && otherPlayer.DashActive == false &&
                    dashActive == true)
                {
                    otherPlayer.StunTimer = relicEffectModifier;
                }

                // Robs a relic if player is in dash and other player has a relic
                if (dashActive == true && otherPlayer.Relic != null)
                {
                    RobRelic(otherPlayer);
                }
            }
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// Ticks the active relic effects
        /// </summary>
        private void TickActiveRelicEffects()
        {
            // Performs relic action based on effect type
            switch (activeRelicEffect)
            {
                
                case RelicController.Effects.Shield:
                    
                    break;
                
                case RelicController.Effects.Trail:
                    if (activeRelicEffect == RelicController.Effects.Trail)
                    {
                        Instantiate(fireTrail, transform.position, Quaternion.identity);
                    }
                    break;
                
                case RelicController.Effects.Points:
                    if (activeRelicEffect == RelicController.Effects.Points)
                    {
                    }
                    break;
                
                // Do nothing
                default:
                    break;
            }
        }
        
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

        #endregion
    }
}