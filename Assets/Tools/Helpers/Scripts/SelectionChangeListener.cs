using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Tools.UGUI.Helpers
{
    /// <summary>
    /// Listens for changes in the selected GameObject and invokes events accordingly.
    /// </summary>
    [AddComponentMenu("Tools/UGUI/Helpers/Selection Change Listener")]
    public class SelectionChangeListener : MonoBehaviour
    {
        /// <summary>
        /// Invoked when the selected GameObject changes. Passes the newly selected GameObject.
        /// </summary>
        public UnityEvent<GameObject> OnSelectionChanged;

        /// <summary>
        /// Invoked when the selection state changes. Passes true if an object is selected, false otherwise.
        /// </summary>
        public UnityEvent<bool> OnSelectionStateChanged;

        private GameObject lastSelectedObject;
        private EventSystem lastEventSystem;

        private void Update()
        {
            CheckForSelectionChanges();
        }

        private void CheckForSelectionChanges()
        {
            if (EventSystem.current == null) return;

            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
            bool selectionChanged = currentSelected != lastSelectedObject || EventSystem.current != lastEventSystem;

            if (selectionChanged)
            {
                OnSelectionChanged.Invoke(currentSelected);
                OnSelectionStateChanged.Invoke(currentSelected != null);

                lastSelectedObject = currentSelected;
                lastEventSystem = EventSystem.current;
            }
        }
    }
}
