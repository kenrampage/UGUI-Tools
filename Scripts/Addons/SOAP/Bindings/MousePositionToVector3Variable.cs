using UnityEngine;
using Obvious.Soap;

namespace KenRampage.Addons.SOAP.Bindings
{
    /// <summary>
    /// Converts mouse screen position to world space coordinates and stores the result in a Vector3Variable.
    /// Handles camera-based coordinate transformation and Z-depth adjustments.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Bindings/Mouse Position To Vector3 Variable")]
    public class MousePositionToVector3Variable : MonoBehaviour
    {
        [SerializeField] private Vector3Variable _currentMousePosition;
        private Camera _mainCamera;

        void Start()
        {
            _mainCamera = Camera.main; // Ensure you have a main camera tagged correctly in the scene
        }

        void Update()
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = _mainCamera.transform.position.z * -1; // Set the depth to match the camera's
            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0; // Adjust z position if necessary, depending on your game setup

            _currentMousePosition.Value = worldPosition;
        }
    }
}