using UnityEngine;
using MoreMountains.TopDownEngine;

namespace KenRampage.Addons.MoreMountains.TopDownEngine
{
    /// <summary>
    /// Extension component for TopDownEngine's WeaponAim that provides easy control over aim modes through public functions.
    /// Allows runtime switching between different aim control types (Mouse, Off, Primary Movement, Secondary Movement)
    /// through both the inspector and context menu.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/More Mountains/TopDown Engine/Weapon Aim Controller")]
    public class WeaponAimController : MonoBehaviour
    {
        #region Private Fields
        [SerializeField] private WeaponAim _weaponAim;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (_weaponAim == null)
            {
                _weaponAim = GetComponent<WeaponAim>();
            }
        }
        #endregion

        #region Public Functions
        [ContextMenu("Set Aim Control: Mouse")]
        public void SetAimControlMouse()
        {
            SetAimControl(WeaponAim.AimControls.Mouse);
        }

        [ContextMenu("Set Aim Control: Off")]
        public void SetAimControlOff()
        {
            SetAimControl(WeaponAim.AimControls.Off);
        }

        [ContextMenu("Set Aim Control: Primary Movement")]
        public void SetAimControlPrimaryMovement()
        {
            SetAimControl(WeaponAim.AimControls.PrimaryMovement);
        }

        [ContextMenu("Set Aim Control: Secondary Movement")]
        public void SetAimControlSecondaryMovement() 
        {
            SetAimControl(WeaponAim.AimControls.SecondaryMovement);
        }
        #endregion

        #region Private Functions
        private void SetAimControl(WeaponAim.AimControls newControl)
        {
            if (_weaponAim != null)
            {
                _weaponAim.AimControl = newControl;
            }
        }
        #endregion
    }
}