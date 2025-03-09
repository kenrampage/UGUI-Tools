using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KenRampage.Addons.MoreMountains.TopDownEngine
{
    /// <summary>
    /// Custom implementation of the TopDown Engine's InputSystemManager that uses InputActionReferences instead of a single InputActionAsset.
    /// This allows for more flexible input binding through the Unity Inspector and better integration with existing input action assets.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/More Mountains/TopDown Engine/Input System Manager")]
    public class InputSystemManager_TDE : InputSystemManager
    {
        #region Input Action References
        [System.Serializable]
        public class InputActionReferences
        {
            public InputActionReference primaryMovement;
            public InputActionReference secondaryMovement;
            public InputActionReference cameraRotation;
            public InputActionReference jump;
            public InputActionReference run;
            public InputActionReference dash;
            public InputActionReference crouch;
            public InputActionReference shoot;
            public InputActionReference secondaryShoot;
            public InputActionReference interact;
            public InputActionReference reload;
            public InputActionReference pause;
            public InputActionReference switchWeapon;
            public InputActionReference switchCharacter;
            public InputActionReference timeControl;
        }

        public new InputActionAsset InputActions;
        public InputActionReferences ActionReferences;
        #endregion

        #region Unity Lifecycle
        protected override void OnEnable()
        {
            // Intentionally left empty to prevent automatic InputActions enabling
        }

        protected override void OnDisable()
        {
            // Intentionally left empty to prevent automatic InputActions disabling
        }

        protected override void Update()
        {
            // Intentionally empty to prevent base input action management
        }
        #endregion

        #region Initialization
        protected override void Initialization()
        {
            base.Initialization();

            ActionReferences.primaryMovement.action.performed += context =>
            {
                if (_targetCamera == null)
                {
                    SetCamera(Camera.main, true);
                }
                
                _primaryMovement = ApplyCameraRotation(context.ReadValue<Vector2>());
            };
            ActionReferences.secondaryMovement.action.performed += context => _secondaryMovement = ApplyCameraRotation(context.ReadValue<Vector2>());
            ActionReferences.cameraRotation.action.performed += context => _cameraRotationInput = context.ReadValue<float>();

            ActionReferences.jump.action.performed += context => { BindButton(context, JumpButton); };
            ActionReferences.run.action.performed += context => { BindButton(context, RunButton); };
            ActionReferences.dash.action.performed += context => { BindButton(context, DashButton); };
            ActionReferences.crouch.action.performed += context => { BindButton(context, CrouchButton); };
            ActionReferences.shoot.action.performed += context => { BindButton(context, ShootButton); };
            ActionReferences.secondaryShoot.action.performed += context => { BindButton(context, SecondaryShootButton); };
            ActionReferences.interact.action.performed += context => { BindButton(context, InteractButton); };
            ActionReferences.reload.action.performed += context => { BindButton(context, ReloadButton); };
            ActionReferences.pause.action.performed += context => { BindButton(context, PauseButton); };
            ActionReferences.switchWeapon.action.performed += context => { BindButton(context, SwitchWeaponButton); };
            ActionReferences.switchCharacter.action.performed += context => { BindButton(context, SwitchCharacterButton); };
            ActionReferences.timeControl.action.performed += context => { BindButton(context, TimeControlButton); };
        }
        #endregion
    }
}