using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tools.UGUI.CursorManager
{
    public class SelectableHoverHandler : MonoBehaviour
    {
        [SerializeField] private HoveredComponentFinder_Selectable hoveredFinder;
        [SerializeField] private bool selectOnHover = true;
        private EventSystem eventSystem;
        private Selectable lastHoveredSelectable;

        private void Start()
        {
            eventSystem = EventSystem.current;
        }

        private void Update()
        {
            if (!selectOnHover) return;

            if (hoveredFinder.IsComponentFound)
            {
                Selectable currentSelectable = hoveredFinder.foundComponent;
                if (currentSelectable != lastHoveredSelectable)
                {
                    SelectNewSelectable(currentSelectable);
                }
            }
            else if (lastHoveredSelectable != null)
            {
                DeselectCurrentSelectable();
            }
        }

        private void SelectNewSelectable(Selectable selectable)
        {
            eventSystem.SetSelectedGameObject(selectable.gameObject);
            lastHoveredSelectable = selectable;
        }

        private void DeselectCurrentSelectable()
        {
            eventSystem.SetSelectedGameObject(null);
            lastHoveredSelectable = null;
        }

        public void SetSelectOnHover(bool value)
        {
            selectOnHover = value;
            if (!selectOnHover)
            {
                DeselectCurrentSelectable();
            }
        }
    }
}
