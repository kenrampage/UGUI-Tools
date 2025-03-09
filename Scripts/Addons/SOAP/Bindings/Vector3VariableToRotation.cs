using UnityEngine;
using Obvious.Soap;

namespace KenRampage.Addons.SOAP.Bindings
{
    /// <summary>
    /// Sets an object's rotation based on a Vector3Variable value. Supports both world and local space rotations,
    /// with optional axis constraints and frame-based updates.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Bindings/Vector3 Variable To Rotation")]
    public class Vector3VariableToRotation : MonoBehaviour
    {
        [SerializeField] private Vector3Variable _variable;

        [Header("Settings")]
        [SerializeField] private bool _updateEveryFrame = false;
        [SerializeField] private bool _useLocalRotation = false;
        [SerializeField] private bool _ignoreX = false;
        [SerializeField] private bool _ignoreY = false;
        [SerializeField] private bool _ignoreZ = false;

        private void Update()
        {
            if (_updateEveryFrame)
            {
                SetRotation();
            }
        }

        [ContextMenu("Set Rotation")]
        public void SetRotation()
        {
            if (_useLocalRotation)
            {
                transform.localEulerAngles = CalcNewRotation();
            }
            else
            {
                transform.eulerAngles = CalcNewRotation();
            }
        }

        private Vector3 CalcNewRotation()
        {
            Vector3 eulerAngles = new Vector3();

            if (_ignoreX)
            {
                eulerAngles.x = transform.rotation.x;
            }
            else
            {
                eulerAngles.x = _variable.Value.x;
            }

            if (_ignoreY)
            {
                eulerAngles.y = transform.rotation.y;
            }
            else
            {
                eulerAngles.y = _variable.Value.y;
            }

            if (_ignoreZ)
            {
                eulerAngles.z = transform.rotation.z;
            }
            else
            {
                eulerAngles.z = _variable.Value.z;
            }

            return eulerAngles;
        }
    }
}