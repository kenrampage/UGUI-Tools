using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class HoveredComponentFinder<T> : MonoBehaviour where T : Component
{
    // Public field to store the hovered GameObject
    public GameObject hoveredObject;

    // Public field to store the found component of type T
    public T foundComponent;

    // Index value to control the pointer index, set in the inspector
    public int pointerIndex = 0;

    void Update()
    {
        // Update the hovered object
        UpdateHoveredObject();

        // Update the found component of type T
        UpdateFoundComponent();
    }

    private void UpdateHoveredObject()
    {
        // Check if the EventSystem is available
        if (EventSystem.current == null)
            return;

        // Get the current input module and ensure it's of type InputSystemUIInputModule
        var inputModule = EventSystem.current.currentInputModule as InputSystemUIInputModule;
        if (inputModule == null)
            return;

        // Get the last raycast result for the specified pointer index
        RaycastResult raycastResult = inputModule.GetLastRaycastResult(pointerIndex);

        // Save the hovered GameObject to the public field
        hoveredObject = raycastResult.gameObject;

        if (hoveredObject != null)
        {
            Debug.Log("Currently hovered object: " + hoveredObject.name);
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
            Debug.Log("Found component: " + foundComponent.name);
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
