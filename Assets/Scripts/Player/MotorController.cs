using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RELIC
{
    public class MotorController : MonoBehaviour
    {
        #region Field Declarations
        private CharacterController characterController;
        private InputAction motorControllerActions;
        private Vector3 moveDirection = Vector3.zero;
        private Vector3 dashDirection = Vector3.zero;
        private bool dashReady = true;
        private bool dashActive = false;
        private float stunTimer = 0f;

        public bool DashActive { get => dashActive; set => dashActive = value; }
        public float StunTimer { get => stunTimer; set => stunTimer = value; }

        [Header("Movement Properties")]
        [Tooltip("The normal movement's distance.")]
        [SerializeField] private float speed = 1f;
        [Tooltip("The dash's distance multiplier.")]
        [SerializeField] private float dashSpeedMultiplier = 3f;
        [Tooltip("The dash's duration.")]
        [SerializeField] private float dashDuration = 1f;
        [Tooltip("The dash's cooldown.")]
        [SerializeField] private float dashCooldown = 5f;
        [Tooltip("Should direction control during dashing be enabled?")]
        [SerializeField] private bool moveDuringDash = false;

        [Header("Player Properties")]
        [Tooltip("The player's character model.")]
        [SerializeField] private Transform playerModel;
        #endregion

        #region Unity Methods
        void Start()
        {
            characterController = GetComponent<CharacterController>();
            motorControllerActions = GetComponent<PlayerInput>().actions.FindAction("Move");
        }

        void Update()
        {
            MovePlayer();
            LookTowardsMovementDirection();
            TickStun();
        }
        #endregion

        #region Custom Methods
        /// <summary>
        /// Moves the player according to his input
        /// </summary>
        public void MovePlayer()
        {
            // Defines the current input
            Vector2 input = motorControllerActions.ReadValue<Vector2>();
            Vector3 movement = new Vector3(input.x, 0f, input.y);

            // Defines the current player direction
            if (movement != Vector3.zero)
            {
                moveDirection = movement.normalized;
            }

            // If the dash is active disable (or not) the player influence on the movement
            if (dashActive)
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
        /// Rotates character so that it looks towards the movement direction
        /// </summary>
        void LookTowardsMovementDirection() {
            playerModel.LookAt(transform.position + moveDirection, Vector3.up);
        }

        /// <summary>
        /// Gives a player a temporary boost in speed according to input and resource availability
        /// </summary>
        /// <param name="callbackContext">The callback context in which the "Dash" button was pressed</param>
        public void Dash(InputAction.CallbackContext callbackContext)
        {
            if (dashReady && Mathf.Approximately(stunTimer, 0f))
            {
                StartCoroutine(ResolveDash());
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

            yield return new WaitForSeconds(dashCooldown - dashDuration);

            dashReady = true;
        }
        #endregion
    }
}
