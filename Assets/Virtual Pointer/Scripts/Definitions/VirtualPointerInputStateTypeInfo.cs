using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// The VirtualPointerInputStateTypeInfo struct defines the input state for the VirtualPointer.
/// It implements the IInputStateTypeInfo interface to provide information about the input state format.
/// The struct contains a single control, position, which is a Vector2 representing the pointer's position.
/// </summary>
namespace Tools.UGUI.VirtualPointer
{
    public struct VirtualPointerInputStateTypeInfo : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('V', 'P', 'T', 'R');

        [InputControl(layout = "Vector2")]
        public Vector2 position;
    }
}
