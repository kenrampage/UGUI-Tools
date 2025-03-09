using UnityEngine;

namespace KenRampage.Utilities.InspectorHelpers
{
    /// <summary>
    /// Simple utility component that prints string messages to the Unity console.
    /// Useful for debugging and event response visualization through the Unity Inspector.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Utilities/Inspector Helpers/String Printer")]
    public class StringPrinter : MonoBehaviour
    {
        public void Print(string message)
        {
            Debug.Log(message);
        }
    }
}