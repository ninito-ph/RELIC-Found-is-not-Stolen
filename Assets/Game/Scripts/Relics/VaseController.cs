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
        [Header("Contained Item")]
        [SerializeField] private GameObject containedItem;
        [Space]
        [Header("Fluff")]
        [SerializeField] private GameObject breakEffect;
        #endregion

        #region MonoBehavior implementation
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCollisionEnter(Collision other)
        {
            // Checks if colliding gameObject is a player
            if (other.gameObject.CompareTag("Player") == true)
            {
                // Check if colliding player is in a dash
                if (other.gameObject.GetComponent<MotorController>().DashActive == true)
                {
                    Instantiate(containedItem, transform.position, Quaternion.identity);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);

                    Destroy(gameObject);
                }

            }
        }
        #endregion
    }
}
