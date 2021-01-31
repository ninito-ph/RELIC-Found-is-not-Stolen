using System;
using System.Collections;
using System.Collections.Generic;
using RELIC;
using UnityEngine;

/// <summary>
/// Controls a trail of fire left by the trail relic
/// </summary>
public class FireTrailController : MonoBehaviour
{
    #region Field Declarations

    [Header("Fire Trail Parameters")] [SerializeField]
    private float trailDuration = 4f;

    [SerializeField] private float trailStunAmount = 1.2f;
    [SerializeField] private GameObject trailExplodeEffect;

    private Coroutine lifetimeRoutine;
    private GameObject spawnedFrom;

    public GameObject SpawnedFrom
    {
        get => spawnedFrom;
        set => spawnedFrom = value;
    }

    #endregion

    #region MonoBehaviour Implementation

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(Lifetime());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject != spawnedFrom)
        {
            // Stuns player
            MotorController player = other.GetComponent<MotorController>();
            player.Stun(trailStunAmount);

            // Self-destructs
            Instantiate(trailExplodeEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    #endregion

    #region Coroutine

    /// <summary>
    /// A routine that destroys the gameObject once its lifetime passes
    /// </summary>
    /// <returns></returns>
    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(trailDuration);
        Instantiate(trailExplodeEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    #endregion
}