using UnityEngine;
using Obvious.Soap;
using KenRampage.Utilities.Camera;

namespace KenRampage.Addons.SOAP.Bindings
{
    /// <summary>
    /// Tracks an object's position (world, local, or screen space) and updates a Vector3Variable with the result.
    /// Supports camera-based coordinate transformations and canvas space handling.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Bindings/Position To Vector3 Variable")]
    [RequireComponent(typeof(CameraProvider))]
    public class PositionToVector3Variable : MonoBehaviour
    {
        [SerializeField] private Vector3Variable _variable;
        [SerializeField] private CameraProvider _cameraProvider;

        [Header("Settings")]
        [SerializeField] private GameObject _objectToTrack;
        [SerializeField] private bool _useLocalPosition;
        [SerializeField] private bool _useScreenPosition;
        [SerializeField] private bool _updateEveryFrame;
        [SerializeField] private bool _updateOnAwake = true;
        [SerializeField] private bool _updateOnEnable;

        private Camera _camera;

        private void Awake()
        {
            if (_updateOnAwake)
            {
                SetVariable();
            }
        }

        private void OnEnable()
        {
            if (_updateOnEnable)
            {
                SetVariable();
            }
        }

        private void Start()
        {
            // Obtain the camera from the CameraProvider
            if (_cameraProvider == null)
            {
                _cameraProvider = GetComponent<CameraProvider>();
            }

            _camera = _cameraProvider.GetCamera();

            if (_objectToTrack == null)
            {
                _objectToTrack = this.gameObject;
            }
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
            if (_objectToTrack != null && _camera != null)
            {
                Canvas canvas = _objectToTrack.GetComponentInParent<Canvas>();
                if (_useScreenPosition)
                {
                    if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    {
                        // The object is on an overlay canvas, use its position directly
                        _variable.Value = _objectToTrack.transform.position;
                    }
                    else
                    {
                        // The object is in world space, convert its position to screen space
                        _variable.Value = _camera.WorldToScreenPoint(_objectToTrack.transform.position);
                    }
                }
                else if (_useLocalPosition)
                {
                    _variable.Value = _objectToTrack.transform.localPosition;
                }
                else
                {
                    _variable.Value = _objectToTrack.transform.position;
                }
            }
            else
            {
                //Debug.Log("No object to track or camera is not assigned.");
            }
        }


        public void SetObjectToTrack(GameObject newObject)
        {
            _objectToTrack = newObject;
            SetVariable();
        }

        public void ClearObjectToTrack()
        {
            _objectToTrack = null;
            SetVariable();
        }
    }
}