using UnityEngine;
using UnityEngine.Events;
using KenRampage.Unity.UI;

namespace KenRampage.Utilities.UI
{
    /// <summary>
    /// Handles the positioning of a marker object based on the currently hovered UI element or a default target.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Utilities/UI/Hover Marker Handler")]
    public class HoverMarkerHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform markerObject;
        [SerializeField] private HoveredComponentFinder_Selectable hoveredFinder;
        [SerializeField] private Transform defaultTargetTransform;
        [SerializeField] private bool isPositioningActive = true;

        public UnityEvent OnHover;
        public UnityEvent OnUnhover;

        private GameObject lastHoveredObject;
        private Vector2 defaultPosition;

        private void Awake()
        {
            defaultPosition = markerObject.anchoredPosition;
        }

        private void Update()
        {
            HandleHoverChanges();
            UpdateMarkerPosition();
        }

        private void HandleHoverChanges()
        {
            GameObject currentHovered = hoveredFinder.IsComponentFound ? hoveredFinder.FoundComponent.gameObject : null;

            if (currentHovered != lastHoveredObject)
            {
                if (currentHovered != null)
                    OnHover.Invoke();
                else
                    OnUnhover.Invoke();
                
                lastHoveredObject = currentHovered;
            }
        }

        private void UpdateMarkerPosition()
        {
            if (!isPositioningActive) return;

            if (hoveredFinder.IsComponentFound)
                SetPositionToScreenSpace(hoveredFinder.FoundComponent.gameObject);
            else if (defaultTargetTransform != null)
                SetPositionToScreenSpace(defaultTargetTransform.gameObject);
            else
                markerObject.anchoredPosition = defaultPosition;
        }

        private void SetPositionToScreenSpace(GameObject target)
        {
            if (target == null) return;

            Canvas targetCanvas = target.GetComponentInParent<Canvas>();
            if (targetCanvas == null) return;

            Vector2 screenPosition = targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? GetScreenPositionOverlay(target)
                : GetScreenPositionWorldSpace(target, targetCanvas);

            markerObject.position = screenPosition;
        }

        private Vector2 GetScreenPositionOverlay(GameObject target)
        {
            RectTransform targetRect = target.GetComponent<RectTransform>();
            return targetRect != null ? targetRect.position : (Vector2)target.transform.position;
        }

        private Vector2 GetScreenPositionWorldSpace(GameObject target, Canvas targetCanvas)
        {
            UnityEngine.Camera worldCamera = targetCanvas.worldCamera ?? UnityEngine.Camera.main;
            return RectTransformUtility.WorldToScreenPoint(worldCamera, target.transform.position);
        }

        /// <summary>
        /// Enables or disables the positioning functionality.
        /// </summary>
        public void SetPositioningActive(bool active)
        {
            isPositioningActive = active;
        }

        /// <summary>
        /// Sets the default target transform for the marker to follow when nothing is hovered.
        /// </summary>
        public void SetDefaultTargetTransform(Transform target)
        {
            defaultTargetTransform = target;
        }
    }
}
