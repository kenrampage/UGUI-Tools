using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KenRampage.Unity.UI
{
    /// <summary>
    /// Handles hover behavior for Selectable UI elements.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Unity/UI/Selectable Hover Handler")]
    public class SelectableHoverHandler : MonoBehaviour
    {
        [SerializeField] private HoveredComponentFinder_Selectable hoveredFinder;
        [SerializeField] private bool selectOnHover = true;
        [SerializeField] private bool deselectOnUnhover = true;

        private Selectable lastHoveredSelectable;

        private void Update()
        {
            HandleHoverBehavior();
        }

        private void HandleHoverBehavior()
        {
            EventSystem eventSystem = EventSystem.current;

            if (hoveredFinder.IsComponentFound)
            {
                HandleHoveredComponent(eventSystem);
            }
            else if (lastHoveredSelectable != null && deselectOnUnhover)
            {
                DeselectCurrentSelectable(eventSystem);
            }

            lastHoveredSelectable = hoveredFinder.IsComponentFound ? hoveredFinder.FoundComponent : null;
        }

        private void HandleHoveredComponent(EventSystem eventSystem)
        {
            Selectable currentSelectable = hoveredFinder.FoundComponent;
            if (currentSelectable != lastHoveredSelectable && selectOnHover && currentSelectable.gameObject != eventSystem.currentSelectedGameObject)
            {
                SelectNewSelectable(currentSelectable, eventSystem);
            }
        }

        private void SelectNewSelectable(Selectable selectable, EventSystem eventSystem)
        {
            eventSystem.SetSelectedGameObject(selectable.gameObject);
            lastHoveredSelectable = selectable;
        }

        private void DeselectCurrentSelectable(EventSystem eventSystem)
        {
            eventSystem.SetSelectedGameObject(null);
            lastHoveredSelectable = null;
        }

        /// <summary>
        /// Sets whether to select on hover.
        /// </summary>
        public void SetSelectOnHover(bool value)
        {
            selectOnHover = value;
            if (!selectOnHover)
            {
                DeselectCurrentSelectable(EventSystem.current);
            }
        }
    }
}
