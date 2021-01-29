using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RELIC
{
    /// <summary>
    /// A class that controls a vase with a relic or idol inside
    /// </summary>
    public class VaseController : MonoBehaviour
    {
        #region Private Fields

        [Header("Contained Item")] [SerializeField]
        private GameObject containedItem;

        [Space] [Header("Fluff")] [SerializeField]
        private GameObject breakEffect;

        [SerializeField] private AudioClip breakSound;

        #endregion

        #region MonoBehavior implementation

        private void OnDestroy()
        {
            if (containedItem != null)
            {
                Instantiate(containedItem, transform.position, Quaternion.identity);
                Instantiate(breakEffect, transform.position, Quaternion.identity);
                AudioSource.PlayClipAtPoint(breakSound, transform.position);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Collision");

            // Checks if colliding gameObject is a player
            if (other.gameObject.CompareTag("Player") == true)
            {
                Debug.Log("Contact");

                // Check if colliding player is in a dash
                if (other.gameObject.GetComponent<MotorController>().DashActive == true)
                {
                    Destroy(gameObject);
                }
            }
        }

        #endregion
    }
}