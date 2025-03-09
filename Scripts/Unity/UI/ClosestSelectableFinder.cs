using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

namespace KenRampage.Unity.UI
{
    /// <summary>
    /// Finds and selects the closest interactable UI element when no object is currently selected.
    /// Useful for maintaining UI navigation state and gamepad/keyboard input functionality.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Unity/Event System/Closest Selectable Finder")]
    public class ClosestSelectableFinder : MonoBehaviour
    {
        private RectTransform thisRectTransform;

        private void Awake()
        {
            thisRectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                FindClosestSelectable();
            }
        }

        private Selectable FindClosestSelectable()
        {
            return Object.FindObjectsByType<Selectable>(FindObjectsSortMode.None)
                .Where(s => s.IsInteractable() && s.isActiveAndEnabled)
                .OrderBy(s => Vector2.Distance(GetScreenPosition(thisRectTransform), GetScreenPosition(s.transform as RectTransform)))
                .FirstOrDefault();
        }

        public void SelectClosestSelectable()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                Selectable closest = FindClosestSelectable();
                if (closest != null)
                {
                    EventSystem.current.SetSelectedGameObject(closest.gameObject);
                }
            }
        }

        private Vector2 GetScreenPosition(RectTransform rectTransform)
        {
            if (rectTransform == null) return Vector2.zero;

            Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
            if (canvas == null) return Vector2.zero;

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return rectTransform.position;
            }
            else
            {
                Camera worldCamera = canvas.worldCamera ?? Camera.main;
                return RectTransformUtility.WorldToScreenPoint(worldCamera, rectTransform.position);
            }
        }
    }
}