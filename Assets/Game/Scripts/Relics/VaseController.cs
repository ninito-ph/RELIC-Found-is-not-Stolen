using System;
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
        private GameObject spawnEffect;

        [SerializeField] private GameObject breakEffect;

        public GameObject ContainedItem
        {
            get => containedItem;
            set => containedItem = value;
        }

        #endregion

        #region MonoBehavior implementation

        private void Start()
        {
            Instantiate(spawnEffect, transform.position, Quaternion.identity);
        }

        private void OnDestroy()
        {
            if (containedItem != null)
            {
                Instantiate(containedItem, transform.position, Quaternion.identity);
            }

            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Checks if colliding gameObject is a player
            if (other.gameObject.CompareTag("Player") == true)
            {
                // Check if colliding player is in a dash
                if (other.gameObject.GetComponent<MotorController>().DashActive == true)
                {
                    Destroy(gameObject);
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (containedItem != null)
            {
                Gizmos.color = new Color(0f, 1f, 0.33f, 0.4f);
                Gizmos.DrawSphere(transform.position + (Vector3.up * 0.5f), 0.66f);
            }
        }
#endif

        #endregion
    }
}