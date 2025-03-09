using UnityEngine;
using Obvious.Soap;

namespace KenRampage.Addons.SOAP.Bindings
{
    /// <summary>
    /// Captures an object's rotation (local or world space) and stores it in a Vector3Variable.
    /// Updates can occur on Awake and/or every frame based on configuration.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Bindings/Rotation To Vector3 Variable")]
    public class RotationToVector3Variable : MonoBehaviour
    {
        [SerializeField] private Vector3Variable _variable;

        [Header("Settings")]
        [SerializeField] private bool _useLocalRotation;
        [SerializeField] private bool _updateEveryFrame;

        private void Awake()
        {
            SetVariable();
        }

        private void Update()
        {
            if (_updateEveryFrame)
            {
                SetVariable();
            }
        }
        
        private void SetVariable()
        {
            if (_useLocalRotation)
            {
                _variable.Value = transform.localEulerAngles;
            }
            else
            {
                _variable.Value = transform.eulerAngles;
            }
        }
    }
}