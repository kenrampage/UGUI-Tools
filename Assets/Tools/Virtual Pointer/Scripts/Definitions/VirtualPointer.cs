using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

/// <summary>
/// The VirtualPointer class extends the Pointer class from the Unity Input System.
/// It represents a virtual pointer device with a customizable position control.
/// The class is decorated with the InputControlLayout attribute to specify its state type and display name.
/// </summary>
namespace Tools.UGUI.VirtualPointer
{
    [InputControlLayout(stateType = typeof(VirtualPointerInputStateTypeInfo), displayName = "Virtual Pointer")]
    public class VirtualPointer : Pointer
    {
        public Vector2Control Position { get; private set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();
            Position = GetChildControl<Vector2Control>("Position");
        }
    }
}
