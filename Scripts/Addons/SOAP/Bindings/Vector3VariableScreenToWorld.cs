using UnityEngine;
using Obvious.Soap;

namespace KenRampage.Addons.SOAP.Bindings
{
    /// <summary>
    /// Handles bidirectional conversion between screen space and world space coordinates using Vector3Variables.
    /// Supports automatic conversion on value changes and manual conversion through direct calls.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Bindings/Vector3 Variable Screen To World")]
    public class Vector3VariableScreenToWorld : MonoBehaviour
    {
        public enum ConversionDirection
        {
            ScreenToWorld,
            WorldToScreen
        }

        [SerializeField] private Camera _camera;
        [SerializeField] private Vector3Variable _screenSpaceVariable;
        [SerializeField] private Vector3Variable _worldSpaceVariable;
        [SerializeField] private ConversionDirection _conversionDirection;
        [SerializeField] private bool _triggerConversionOnValueChange = true;

        private void OnEnable()
        {
            if (_triggerConversionOnValueChange)
            {
                if (_conversionDirection == ConversionDirection.ScreenToWorld)
                {
                    _screenSpaceVariable.OnValueChanged += HandleValueChanged;
                }
                else if (_conversionDirection == ConversionDirection.WorldToScreen)
                {
                    _worldSpaceVariable.OnValueChanged += HandleValueChanged;
                }
            }
        }

        private void OnDisable()
        {
            if (_triggerConversionOnValueChange)
            {
                if (_conversionDirection == ConversionDirection.ScreenToWorld)
                {
                    _screenSpaceVariable.OnValueChanged -= HandleValueChanged;
                }
                else if (_conversionDirection == ConversionDirection.WorldToScreen)
                {
                    _worldSpaceVariable.OnValueChanged -= HandleValueChanged;
                }
            }
        }

        private void HandleValueChanged(Vector3 value)
        {
            Convert();
        }

        public void Convert()
        {
            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
                if (_camera == null)
                {
                    Debug.LogError("Camera is not assigned for conversion and could not be found on the current object.");
                    return;
                }
            }

            if (_screenSpaceVariable == null || _worldSpaceVariable == null)
            {
                Debug.LogError("ScreenSpaceVariable or WorldSpaceVariable is not assigned.");
                return;
            }

            if (_conversionDirection == ConversionDirection.ScreenToWorld)
            {
                ConvertScreenToWorld();
            }
            else if (_conversionDirection == ConversionDirection.WorldToScreen)
            {
                ConvertWorldToScreen();
            }
        }

        private void ConvertScreenToWorld()
        {
            Vector3 screenSpace = _screenSpaceVariable.Value;
            screenSpace.z = _camera.nearClipPlane;
            Vector3 worldSpace = _camera.ScreenToWorldPoint(screenSpace);
            _worldSpaceVariable.Value = worldSpace;
        }

        private void ConvertWorldToScreen()
        {
            Vector3 worldSpace = _worldSpaceVariable.Value;
            Vector3 screenSpace = _camera.WorldToScreenPoint(worldSpace);
            _screenSpaceVariable.Value = screenSpace;
        }
    }
}