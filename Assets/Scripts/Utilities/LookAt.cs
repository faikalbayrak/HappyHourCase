using UnityEngine;

namespace Utilities
{
    public class LookAt : MonoBehaviour
    {
        public Transform LookAtTransform;

        [Header("Disable Rotation Axes")]
        public bool disableX = false;
        public bool disableY = false;
        public bool disableZ = false;

        private void LateUpdate()
        {
            if (LookAtTransform != null)
            {
                Vector3 direction = LookAtTransform.position - transform.position;
                
                if (disableX) direction.x = 0;
                if (disableY) direction.y = 0;
                if (disableZ) direction.z = 0;
                
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = targetRotation;
                }
            }
        }
    }
}