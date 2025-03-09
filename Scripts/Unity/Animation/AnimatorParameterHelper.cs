using UnityEngine;

namespace KenRampage.Unity.Animation
{
    /// <summary>
    /// Helper component to expose Animator parameter controls to the Unity Inspector and Events system.
    /// Configure the parameter name in the inspector, then use the provided methods to control that parameter.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Unity/Animation/Animator Parameter Helper")]
    public class AnimatorParameterHelper : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Animator animator;
        [SerializeField] private string parameterName;
        #endregion

        #region Parameter Controls
        public void SetBool(bool value)
        {
            animator.SetBool(parameterName, value);
        }

        public void SetTrigger()
        {
            animator.SetTrigger(parameterName);
        }

        public void SetFloat(float value)
        {
            animator.SetFloat(parameterName, value);
        }

        public void SetInteger(int value)
        {
            animator.SetInteger(parameterName, value);
        }
        #endregion
    }
}