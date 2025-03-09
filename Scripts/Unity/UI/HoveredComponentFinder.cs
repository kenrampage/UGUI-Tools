using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace KenRampage.Unity.UI
{
    /// <summary>
    /// Generic class for finding and tracking components under the cursor.
    /// </summary>
    /// <typeparam name="T">The type of component to find. Must derive from Component.</typeparam>
    public class HoveredComponentFinder<T> : MonoBehaviour where T : Component
    {
        [Header("Settings")]
        [SerializeField] private int pointerIndex = 0;

        private GameObject hoveredObject;
        private T foundComponent;

        /// <summary>
        /// Gets the currently hovered GameObject.
        /// </summary>
        public GameObject HoveredObject => hoveredObject;

        /// <summary>
        /// Gets the found component of type T on the hovered object or its parents.
        /// </summary>
        public T FoundComponent => foundComponent;

        /// <summary>
        /// Indicates whether a component of type T has been found.
        /// </summary>
        public bool IsComponentFound => foundComponent != null;

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
            if (EventSystem.current == null || !(EventSystem.current.currentInputModule is InputSystemUIInputModule inputModule))
                return;

            RaycastResult raycastResult = inputModule.GetLastRaycastResult(pointerIndex);
            hoveredObject = raycastResult.gameObject;
        }

        /// <summary>
        /// Updates the found component based on the current hovered object.
        /// </summary>
        private void UpdateFoundComponent()
        {
            foundComponent = hoveredObject != null ? FindComponentInParents<T>(hoveredObject) : null;
        }

        /// <summary>
        /// Finds a component of type U in the given object or its parents.
        /// </summary>
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

        /// <summary>
        /// Sets the pointer index for raycasting.
        /// </summary>
        public void SetPointerIndex(int index)
        {
            pointerIndex = index;
        }
    }
}
