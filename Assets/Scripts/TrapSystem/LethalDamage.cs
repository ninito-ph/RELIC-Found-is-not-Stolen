using UnityEngine;
using UnityEngine.Events;

namespace RELIC
{
    public class LethalDamage : MonoBehaviour
    {
        #region Unity Methods
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<MotorController>().Die();
            }
        }
        #endregion
    }
}