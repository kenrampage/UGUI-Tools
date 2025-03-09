using UnityEngine;
using UnityEngine.InputSystem;

namespace KenRampage.Unity.Input
{
    /// <summary>
    /// Manages switching between different Input Action Maps, maintaining a history of previous maps
    /// and providing functionality to enable/disable specific action maps.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Unity/Input/Input Action Map Switcher")]
    public class InputActionMapSwitcher : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputActionAsset;
        private string _currentActionMap;
        private string _previousActionMap;

        public void SwitchActionMap(string actionMapName)
        {
            DisableAllActionMaps();
            EnableActionMap(actionMapName);
        }

        public void SwitchToPreviousActionMap()
        {
            if (_previousActionMap != null)
            {
                SwitchActionMap(_previousActionMap);
            }
        }

        public void DisableAllActionMaps()
        {
            foreach (var map in _inputActionAsset.actionMaps)
            {
                map.Disable();
            }
        }

        private void EnableActionMap(string actionMapName)
        {
            var actionMap = _inputActionAsset.FindActionMap(actionMapName);
            if (actionMap != null)
            {
                _previousActionMap = _currentActionMap;
                _currentActionMap = actionMapName;
                actionMap.Enable();
            }
            else
            {
                Debug.LogWarning($"Action map '{actionMapName}' not found.");
            }
        }
    }
}