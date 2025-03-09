using UnityEngine;
using Obvious.Soap;

namespace KenRampage.Addons.SOAP.Bindings
{
    /// <summary>
    /// Makes an object look at a target position defined by a Vector3Variable, with optional axis constraints.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Bindings/Look At Vector3 Variable")]
    public class LookAtVector3Variable : MonoBehaviour
    {
        [SerializeField]
        private Vector3Variable targetPosition;

        [Header("Axis Constraints")]
        public bool ignoreX = false;
        public bool ignoreY = false;
        public bool ignoreZ = false;

        void Update()
        {
            Vector3 direction = targetPosition - transform.position;

            // Apply axis constraints
            if (ignoreX) direction.x = 0;
            if (ignoreY) direction.y = 0;
            if (ignoreZ) direction.z = 0;

            // Ensure direction is not zero to avoid errors with LookRotation
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = lookRotation;
            }
        }
    }
}