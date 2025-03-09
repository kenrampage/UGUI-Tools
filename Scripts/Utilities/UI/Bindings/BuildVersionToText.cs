using UnityEngine;
using TMPro;

namespace KenRampage.Utilities.UI.Bindings
{
    /// <summary>
    /// Displays the application version in a TextMeshProUGUI component.
    /// Updates the text when the component is enabled.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Utilities/UI/Bindings/Build Version To Text")]
    public class BuildVersionToText : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private TextMeshProUGUI _text;

        private void OnEnable()
        {
            if (_text != null)
            {
                string version = Application.version;
                _text.text = version;
            }
            else
            {
                Debug.LogWarning("TextMeshProUGUI reference is not set in BuildVersionToText script.");
            }
        }
    }
}