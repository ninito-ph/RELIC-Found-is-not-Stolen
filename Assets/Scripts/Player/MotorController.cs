using System;
using System.Collections;
using UnityEngine;

namespace RELIC
{
    public class MotorController : MonoBehaviour
    {
        #region Field Declarations
        private CharacterController characterController;
        private Vector3 moveDirection = Vector3.zero;
        private Vector3 dashDirection = Vector3.zero;
        private bool dashReady = true;
        private bool dashActive = false;
        private float stunTimer = 0f;

        [Header("Movement Properties")]
        [Tooltip("The distance a player can move normally.")]
        [SerializeField] private float speed = 1f;
        [Tooltip("The distance multiplier a player gets when dashing.")]
        [SerializeField] private float dashSpeedMultiplier = 3f;
        [Tooltip("The dash's duration.")]
        [SerializeField] private float dashDuration = 1f;
        [Tooltip("The cooldown after using a dash.")]
        [SerializeField] private float dashCooldown = 5f;
        [Tooltip("Should direction control during dashing be enabled?")]
        [SerializeField] private bool moveDuringDash = false;

        public float StunTimer { get => stunTimer; set => stunTimer = value; }
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
        #endregion

        #region Custom Methods
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
                characterController.Move(movement * speed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Gives a player a temporary boost in speed accourding to input and resource availability
        /// </summary>
        private void Dash()
        {
            if (dashReady == true && !Mathf.Approximately(Input.GetAxis("Fire1"), 0f) && Mathf.Approximately(stunTimer, 0f))
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

            yield return new WaitForSeconds(cooldown - duration);

            dashReady = true;
        }
        #endregion
    }
}
