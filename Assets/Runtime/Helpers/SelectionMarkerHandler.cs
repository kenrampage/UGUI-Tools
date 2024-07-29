using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Tools.UGUI.Helpers
{
    /// <summary>
    /// Handles the positioning of a marker object based on the currently selected UI element.
    /// </summary>
    [AddComponentMenu("Tools/UGUI/Helpers/Selection Marker Handler")]
    public class SelectionMarkerHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform markerObject;
        [SerializeField] private bool isPositioningActive = true;

        public UnityEvent OnSelect;
        public UnityEvent OnDeselect;

        private GameObject lastSelectedObject;

        private void Update()
        {
            HandleSelectionChanges();
            UpdateMarkerPosition();
        }

        /// <summary>
        /// Checks for changes in the selected object and invokes appropriate events.
        /// </summary>
        private void HandleSelectionChanges()
        {
            if (EventSystem.current == null) return;

            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
            
            if (currentSelected != lastSelectedObject)
            {
                if (currentSelected != null)
                {
                    OnSelect.Invoke();
                }
                else
                {
                    OnDeselect.Invoke();
                }
                lastSelectedObject = currentSelected;
            }
        }

        /// <summary>
        /// Updates the marker's position to match the currently selected object.
        /// </summary>
        private void UpdateMarkerPosition()
        {
            if (isPositioningActive && EventSystem.current != null)
            {
                GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
                if (currentSelected != null)
                {
                    SetPositionToScreenSpace(currentSelected);
                }
            }
        }

        /// <summary>
        /// Sets the marker's position to match the screen space position of the target object.
        /// </summary>
        /// <param name="target">The target GameObject to match position with.</param>
        private void SetPositionToScreenSpace(GameObject target)
        {
            RectTransform targetRect = target.GetComponent<RectTransform>();
            if (targetRect == null) return;

            Canvas targetCanvas = targetRect.GetComponentInParent<Canvas>();
            if (targetCanvas == null) return;

            Vector2 screenPosition;

            if (targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                screenPosition = targetRect.position;
            }
            else
            {
                Camera worldCamera = targetCanvas.worldCamera ?? Camera.main;
                screenPosition = RectTransformUtility.WorldToScreenPoint(worldCamera, targetRect.position);
            }

            markerObject.position = screenPosition;
        }

        /// <summary>
        /// Enables or disables the positioning functionality.
        /// </summary>
        /// <param name="active">Whether positioning should be active.</param>
        public void SetPositioningActive(bool active)
        {
            isPositioningActive = active;
        }
    }
}
