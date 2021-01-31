using UnityEngine;
using UnityEngine.Events;

namespace RELIC
{
    public class LethalDamage : MonoBehaviour
    {
        #region Unity Methods
        void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                GameManager.gameManager.SpawnPlayerAfterDeath(collider.gameObject);
            }
        }
        #endregion
    }
}