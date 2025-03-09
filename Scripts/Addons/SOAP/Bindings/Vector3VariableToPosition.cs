using UnityEngine;
using Obvious.Soap;

namespace KenRampage.Addons.SOAP.Bindings
{
    /// <summary>
    /// Sets an object's position based on a Vector3Variable value. Supports world space, local space, 
    /// and anchored positions for UI elements. Includes axis constraints and various update triggers.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Bindings/Vector3 Variable To Position")]
    public class Vector3VariableToPosition : MonoBehaviour
    {
        public enum PositionMode
        {
            WorldSpace,
            LocalSpace,
            AnchoredPosition
        }

        [SerializeField] private Vector3Variable _variable;

        [Header("Settings")]
        [SerializeField] private GameObject _targetObject;
        [SerializeField] private bool _updateOnValueChanged = false;
        [SerializeField] private bool _updateEveryFrame = false;
        [SerializeField] private bool _updateOnStart = false;
        [SerializeField] private bool _updateOnEnable = true;
        [SerializeField] private PositionMode _positionMode;
        [SerializeField] private bool _ignoreX = false;
        [SerializeField] private bool _ignoreY = false;
        [SerializeField] private bool _ignoreZ = false;

        private bool _useRigidBody = false;

        private void Update()
        {
            if (_updateEveryFrame)
            {
                SetPosition();
            }
        }

        private void Start()
        {
            if (_updateOnStart)
            {
                SetPosition();
            }
        }

        private void OnEnable()
        {
            _variable.OnValueChanged += HandleValueChanged;

            if (_updateOnEnable)
            {
                SetPosition();
            }
        }

        private void OnDisable()
        {
            _variable.OnValueChanged -= HandleValueChanged;
        }

        private void HandleValueChanged(Vector3 value)
        {
            if (_updateOnValueChanged)
            {
                SetPosition();
            }
        }

        [ContextMenu("Set Position")]
        public void SetPosition()
        {
            if (_targetObject == null)
            {
                return;
            }

            Vector3 newPosition = CalcNewPosition();
            Rigidbody rb = GetComponent<Rigidbody>();

            switch (_positionMode)
            {
                case PositionMode.WorldSpace:
                    if (_useRigidBody && rb != null)
                    {
                        rb.MovePosition(newPosition);
                    }
                    else
                    {
                        _targetObject.transform.position = newPosition;
                    }
                    break;

                case PositionMode.LocalSpace:
                    if (_useRigidBody && rb != null)
                    {
                        rb.MovePosition(newPosition);
                    }
                    else
                    {
                        _targetObject.transform.localPosition = newPosition;
                    }
                    break;

                case PositionMode.AnchoredPosition:
                    RectTransform rectTransform = _targetObject.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = newPosition;
                    break;
            }

            Physics.SyncTransforms();
        }

        private Vector3 CalcNewPosition()
        {
            Vector3 newPosition = new Vector3();

            if (_ignoreX)
            {
                newPosition.x = _targetObject.transform.position.x;
            }
            else
            {
                newPosition.x = _variable.Value.x;
            }

            if (_ignoreY)
            {
                newPosition.y = _targetObject.transform.position.y;
            }
            else
            {
                newPosition.y = _variable.Value.y;
            }

            if (_ignoreZ)
            {
                newPosition.z = _targetObject.transform.position.z;
            }
            else
            {
                newPosition.z = _variable.Value.z;
            }

            return newPosition;
        }

        public void SetTargetObject(GameObject targetObject)
        {
            _targetObject = targetObject;
        }

        public void ClearTargetObject()
        {
            _targetObject = null;
        }
    }
}