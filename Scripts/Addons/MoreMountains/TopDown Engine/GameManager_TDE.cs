using MoreMountains.TopDownEngine;
using UnityEngine;

namespace KenRampage.Addons.MoreMountains.TopDownEngine
{
    /// <summary>
    /// Custom version of GameManager that removes the DontDestroyOnLoad behavior from the original implementation.
    /// This allows the game manager to be destroyed between scenes when needed while maintaining all other functionality.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Addons/More Mountains/TopDown Engine/Game Manager")]
    public class GameManager_TDE : GameManager
    {
        protected override void InitializeSingleton()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (AutomaticallyUnparentOnAwake)
            {
                this.transform.SetParent(null);
            }

            if (_instance == null)
            {
                _instance = this as GameManager_TDE;
                _enabled = true;
            }
            else
            {
                if(this != _instance)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}