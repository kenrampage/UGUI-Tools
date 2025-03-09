using UnityEngine;
using System;
using System.Collections.Generic;

namespace KenRampage.Utilities.StateManagement
{
    /// <summary>
    /// Base class for switching between different GameObjects based on a provided key.
    /// Only the GameObject with the matching key will be enabled, while others will be disabled.
    /// Extend this class to implement custom switching logic by overriding OnSwitchObjects.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Utilities/State Management/Object Switcher")]
    public class ObjectSwitcher : MonoBehaviour
    {
        #region Nested Classes
        [Serializable]
        public class ObjectMapping
        {
            public string Key;
            public GameObject GameObject;
        }

        [Serializable] 
        public class Debug
        {
            public string PreviousKey;
            public string CurrentKey;
        }
        #endregion

        #region Fields
        [SerializeField] protected Debug _debug;
        [SerializeField] protected List<ObjectMapping> _objectMappings;
        #endregion

        #region Public Methods
        public void SwitchObjects(string key)
        {
            if (_objectMappings == null) return;
            if (_debug.CurrentKey == key) return;

            _debug.PreviousKey = _debug.CurrentKey;
            _debug.CurrentKey = key;

            OnSwitchObjects(key);
        }
        #endregion

        #region Protected Methods
        protected virtual void OnSwitchObjects(string key)
        {
            foreach (var mapping in _objectMappings)
            {
                if (mapping.GameObject != null)
                {
                    mapping.GameObject.SetActive(false);
                }
            }

            foreach (var mapping in _objectMappings)
            {
                if (mapping.GameObject != null && mapping.Key == key)
                {
                    mapping.GameObject.SetActive(true);
                }
            }
        }
        #endregion
    }
}