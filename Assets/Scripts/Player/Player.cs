using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Field Declarations
    private CharacterController characterController;
    private Vector3 playerDirection = Vector3.zero;
    private bool dashReady = true;
    private bool dashActive = false;

    [Header("Movement Properties")]
    [Tooltip("The distance a player can move normally.")]
    [SerializeField] private float movementDistance = 1f;
    [Tooltip("The distance multiplier a player gets when dashing.")]
    [SerializeField] private float dashDistanceMultiplier = 3f;
    [Tooltip("The dash's duration.")]
    [SerializeField] private float dashDuration = 1f;
    [Tooltip("The cooldown after using a dash.")]
    [SerializeField] private float dashCooldown = 5f;
    #endregion

    #region Unity Methods
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        MovePlayer();
        Dash();
    }
    #endregion

    #region Custom Methods
    void MovePlayer()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if (movement != Vector3.zero)
        {
            playerDirection = movement.normalized;
        }

        if (dashActive)
        {
            movement = movement * dashDistanceMultiplier;
        }

        characterController.Move(movement * movementDistance * Time.deltaTime);
    }

    void Dash()
    {
        if (dashReady && Convert.ToBoolean(Input.GetAxis("Fire1")))
        {
            StartCoroutine(WaitForDashCooldown(dashDuration, dashCooldown));
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator WaitForDashCooldown(float duration, float cooldown)
    {
        dashReady = false;
        dashActive = true;

        yield return new WaitForSeconds(duration);

        dashActive = false;

        yield return new WaitForSeconds(cooldown - duration);

        dashReady = true;
    }
    #endregion
}
