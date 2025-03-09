using UnityEngine;
using Obvious.Soap;
using System.Collections.Generic;

namespace KenRampage.Addons.SOAP.Bindings
{
    /// <summary>
    /// Manages a collection of GameObjects whose positions are driven by a ScriptableListVector3.
    /// Handles object instantiation, removal, and position updates based on the Vector3 list's contents.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/SOAP/Bindings/Vector3 List To GameObject Positions")]
    public class Vector3ListToGameObjectPositions : MonoBehaviour
    {
        [SerializeField] private ScriptableListVector3 _vector3List;

        [Space(10)]
        [SerializeField] private bool _updateEveryFrame;
        [Space(10)]
        [SerializeField] private GameObject _sourceObject;
        [SerializeField] private List<GameObject> _instantiatedObjects;

        private void Update()
        {
            if(_updateEveryFrame)
            {
                UpdatePositions();
            }
        }

        private void OnEnable()
        {
            _vector3List.OnItemAdded += AddObject;
            _vector3List.OnItemRemoved += RemoveObject;
        }

        private void OnDisable()
        {
            _vector3List.OnItemAdded -= AddObject;
            _vector3List.OnItemRemoved -= RemoveObject;
        }

        public void ClearInstantiatedObjects()
        {
            foreach(GameObject go in _instantiatedObjects)
            {
                Destroy(go);
            }

            _instantiatedObjects.Clear();
        }

        public void AddObject(Vector3 position)
        {
            GameObject newObject = Instantiate(_sourceObject);
            newObject.transform.parent = transform;
            newObject.transform.position = position;

            _instantiatedObjects.Add(newObject);
        }

        public void RemoveObject(Vector3 position)
        {
            if (_instantiatedObjects.Count > 0)
            {
                GameObject objectToRemove = _instantiatedObjects[_instantiatedObjects.Count - 1];
                _instantiatedObjects.Remove(objectToRemove);
                Destroy(objectToRemove);
            }
        }

        private void UpdatePositions()
        {
            if (_vector3List.Count != _instantiatedObjects.Count) return;

            for (int i = 0; i < _vector3List.Count; i++)
            {
                _instantiatedObjects[i].transform.position = _vector3List[i];
            }
        }
    }
}