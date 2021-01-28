using System;
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
        private Vector3 playerDirection = Vector3.zero;
        private Vector3 dashDirection = Vector3.zero;
        private bool dashReady = true;
        private bool dashActive = false;

        [Header("Movement Properties")]
        [Tooltip("The normal movement's distance.")]
        [SerializeField] private float movementDistance = 1f;
        [Tooltip("The dash's distance multiplier.")]
        [SerializeField] private float dashDistanceMultiplier = 3f;
        [Tooltip("The dash's duration.")]
        [SerializeField] private float dashDuration = 1f;
        [Tooltip("The dash's cooldown.")]
        [SerializeField] private float dashCooldown = 5f;
        [Tooltip("Should direction control during dashing be enabled?")]
        [SerializeField] private bool canChangeDirectionDuringDash = false;
        #endregion

        #region Unity Methods
        void Start()
        {
            characterController = GetComponent<CharacterController>();
            motorControllerActions = GetComponent<PlayerInput>().actions.FindActionMap(name).FindAction("Move");
        }

        void Update()
        {
            Move();
            LookTowardsMovementDirection();
        }
        #endregion

        #region Custom Methods
        public void Move()
        {
            Vector2 input = motorControllerActions.ReadValue<Vector2>();
            Vector3 movement = new Vector3(input.x, 0f, input.y);

            if (movement != Vector3.zero)
            {
                playerDirection = movement.normalized;
            }

            if (dashActive)
            {
                if (canChangeDirectionDuringDash)
                {
                    movement = movement * dashDistanceMultiplier;
                }
                else
                {
                    movement = dashDirection * dashDistanceMultiplier;
                }
            }

            characterController.Move(movement * movementDistance * Time.deltaTime);
        }

        void LookTowardsMovementDirection() {
            transform.LookAt(transform.position + playerDirection, Vector3.up);
        }

        public void Dash(InputAction.CallbackContext callbackContext)
        {
            if (dashReady && playerDirection != Vector3.zero)
            {
                StartCoroutine(ResolveDash(dashDuration, dashCooldown));
            }
        }
        #endregion

        #region Coroutines
        private IEnumerator ResolveDash(float duration, float cooldown)
        {
            dashReady = false;
            dashActive = true;
            if (!canChangeDirectionDuringDash)
            {
                dashDirection = playerDirection;
            }

            yield return new WaitForSeconds(duration);

            dashActive = false;

            yield return new WaitForSeconds(cooldown - duration);

            dashReady = true;
        }
        #endregion
    }
}
