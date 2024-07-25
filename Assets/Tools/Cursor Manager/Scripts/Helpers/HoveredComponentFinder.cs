using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Tools.UGUI.CursorManager
{
    /// <summary>
    /// Generic class for finding and tracking components under the cursor.
    /// </summary>
    /// <typeparam name="T">The type of component to find. Must derive from Component.</typeparam>
    public class HoveredComponentFinder<T> : MonoBehaviour where T : Component
    {
        /// <summary>
        /// The currently hovered GameObject.
        /// </summary>
        public GameObject hoveredObject;

        /// <summary>
        /// The found component of type T on the hovered object or its parents.
        /// </summary>
        public T foundComponent;

        /// <summary>
        /// Indicates whether a component of type T has been found.
        /// </summary>
        public bool IsComponentFound => foundComponent != null;

        /// <summary>
        /// The index of the pointer to use for raycasting.
        /// </summary>
        public int pointerIndex = 0;

        private void Update()
        {
            UpdateHoveredObject();
            UpdateFoundComponent();
        }

        /// <summary>
        /// Updates the hovered object using the current EventSystem and InputModule.
        /// </summary>
        private void UpdateHoveredObject()
        {
            if (EventSystem.current == null)
                return;

            var inputModule = EventSystem.current.currentInputModule as InputSystemUIInputModule;
            if (inputModule == null)
                return;

            RaycastResult raycastResult = inputModule.GetLastRaycastResult(pointerIndex);
            hoveredObject = raycastResult.gameObject;
        }

        /// <summary>
        /// Updates the found component based on the current hovered object.
        /// </summary>
        private void UpdateFoundComponent()
        {
            foundComponent = (hoveredObject != null) ? FindComponentInParents<T>(hoveredObject) : null;
        }

        /// <summary>
        /// Finds a component of type U in the given object or its parents.
        /// </summary>
        /// <typeparam name="U">The type of component to find.</typeparam>
        /// <param name="child">The starting GameObject to search from.</param>
        /// <returns>The found component, or null if not found.</returns>
        private U FindComponentInParents<U>(GameObject child) where U : Component
        {
            Transform t = child.transform;
            while (t != null)
            {
                if (t.TryGetComponent(out U component))
                    return component;
                t = t.parent;
            }
            return null;
        }
    }
}
