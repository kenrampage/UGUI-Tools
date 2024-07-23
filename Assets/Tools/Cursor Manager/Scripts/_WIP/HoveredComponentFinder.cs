using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Tools.UGUI.CursorManager._WIP
{
    public class HoveredComponentFinder<T> : MonoBehaviour where T : Component
    {
        public GameObject hoveredObject;

        public T foundComponent;
        public bool IsComponentFound => foundComponent != null;

        public int pointerIndex = 0;

        void Update()
        {
            UpdateHoveredObject();
            UpdateFoundComponent();
        }

        private void UpdateHoveredObject()
        {
            if (EventSystem.current == null)
                return;


            var inputModule = EventSystem.current.currentInputModule as InputSystemUIInputModule;
            if (inputModule == null)
                return;

            RaycastResult raycastResult = inputModule.GetLastRaycastResult(pointerIndex);
            hoveredObject = raycastResult.gameObject;

            if (hoveredObject != null)
            {
                //Debug.Log("Currently hovered object: " + hoveredObject.name);
            }
        }

        private void UpdateFoundComponent()
        {
            if (hoveredObject == null)
            {
                foundComponent = null;
                return;
            }

            // Find the component of type T in the hierarchy
            foundComponent = FindComponentInParents<T>(hoveredObject);

            if (foundComponent != null)
            {
                //Debug.Log("Found component: " + foundComponent.name);
            }
        }

        private U FindComponentInParents<U>(GameObject child) where U : Component
        {
            Transform t = child.transform;
            while (t != null)
            {
                U component = t.GetComponent<U>();
                if (component != null)
                    return component;
                t = t.parent;
            }
            return null;
        }
    }
}