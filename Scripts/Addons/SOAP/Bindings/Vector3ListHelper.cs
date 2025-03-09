using Obvious.Soap;
using UnityEngine;

namespace KenRampage.Addons.SOAP.Bindings
{
    /// <summary>
    /// Provides helper functions for managing a ScriptableListVector3, including adding items, 
    /// removing the last item, and clearing the list. Accessible through the component's context menu.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Bindings/Vector3 List Helper")]
    public class Vector3ListHelper : MonoBehaviour
    {
        [SerializeField] private ScriptableListVector3 _vector3List;

        [ContextMenu("Add Current Position")]
        public void AddCurrentPosition()
        {
            _vector3List.Add(transform.position);
        }

        [ContextMenu("Remove Last Item")]
        public void RemoveLastListItem()
        {
            if(_vector3List.Count > 0 )
            {
                _vector3List.Remove(_vector3List[_vector3List.Count - 1]);
            }
        }

        [ContextMenu("Clear List")]
        public void ClearList()
        {
            for (int i = 0; 0 < _vector3List.Count; i++)
            {
                RemoveLastListItem();
            }
        }
    }
}