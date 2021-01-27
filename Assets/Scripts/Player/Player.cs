using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Field Declarations
    private CharacterController characterController;
    private Vector3 playerDirection = Vector3.zero;
    private bool dashReady = true;

    [Header("Movement Properties")]
    [Tooltip("The distance a player can move normally.")]
    [SerializeField] private float movementDistance = 1f;
    [Tooltip("The distance a player can move when dashing.")]
    [SerializeField] private float dashDistance = 3f;
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
    void MovePlayer() {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        characterController.Move(movement * movementDistance * Time.deltaTime);
        if(movement != Vector3.zero) {
            playerDirection = movement.normalized;
        }
    }

    void Dash() {
        if(dashReady && Convert.ToBoolean(Input.GetAxis("Fire1"))) {
            characterController.Move(playerDirection * dashDistance * Time.deltaTime);
            StartCoroutine(WaitForDashCooldown(dashCooldown));
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator WaitForDashCooldown(float delay) {
        dashReady = false;
        yield return new WaitForSeconds(delay);
        dashReady = true;
    }
    #endregion
}
