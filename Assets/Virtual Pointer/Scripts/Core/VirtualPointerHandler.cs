using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

namespace Tools.UGUI.VirtualPointer
{
    [RequireComponent(typeof(RectTransform))]
    public class VirtualPointerHandler : MonoBehaviour
    {
        private VirtualPointer _virtualPointer;
        private RectTransform _rectTransform;

        [Header("Settings")]
        [SerializeField] private bool _manageHardwareCursorVisibility;

        #region Unity Methods
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            RegisterVirtualPointer();
            StartCoroutine(CreateVirtualPointerAtEndOfFrame());
        }
        private void OnEnable()
        {
            HideCursor();
        }

        private void OnDisable()
        {
            ShowCursor();
        }

        private void OnDestroy()
        {
            RemoveVirtualPointer();
        }

        private void Update()
        {
            UpdateVirtualPointerPosition();

            HideCursor();
        }
        #endregion

        #region Private Methods

        private IEnumerator CreateVirtualPointerAtEndOfFrame()
        {
            // Wait until all other updates are done
            yield return new WaitForEndOfFrame();
            CreateVirtualPointer();
        }

        private void CreateVirtualPointer()
        {
            if (InputSystem.GetDevice<VirtualPointer>() == null)
            {
                _virtualPointer = InputSystem.AddDevice<VirtualPointer>();
            }
            else
            {
                _virtualPointer = InputSystem.GetDevice<VirtualPointer>();
            }
        }

        private void RemoveVirtualPointer()
        {
            if (_virtualPointer != null && InputSystem.GetDevice<VirtualPointer>() == _virtualPointer)
            {
                InputSystem.RemoveDevice(_virtualPointer);
                _virtualPointer = null;
            }
        }

        private void UpdateVirtualPointerPosition()
        {
            if (_virtualPointer != null)
            {
                Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, _rectTransform.position);
                InputSystem.QueueStateEvent(_virtualPointer, new VirtualPointerInputStateTypeInfo { Position = screenPosition });
            }
        }

        private static void RegisterVirtualPointer()
        {
            InputSystem.RegisterLayout<VirtualPointer>(
                matches: new InputDeviceMatcher().WithInterface("VirtualPointer"));
        }

        private void ShowCursor()
        {
            if (_manageHardwareCursorVisibility)
            {
                Cursor.visible = true;
            }
        }

        private void HideCursor()
        {
            if (_manageHardwareCursorVisibility)
            {
                Cursor.visible = false;
            }
        }

        #endregion
    }
}