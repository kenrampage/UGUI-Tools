using UnityEngine;

namespace KenRampage.Unity.Materials
{
    /// <summary>
    /// Controls material emission properties through MaterialPropertyBlocks, providing efficient
    /// methods to toggle emission and adjust emission intensity.
    /// </summary>
    [AddComponentMenu("Ken Rampage/Unity/Materials/Material Emission Helper")]
    public class MaterialEmissionHelper : MonoBehaviour
    {
        #region Fields
        [SerializeField] private MeshRenderer _meshRenderer;
        private MaterialPropertyBlock _propertyBlock;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        #endregion

        #region Unity Messages
        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
        }
        #endregion

        #region Emission Controls
        public void SetEmission(bool enabled)
        {
            _propertyBlock.SetColor(EmissionColor, enabled ? Color.white : Color.black);
            _meshRenderer.SetPropertyBlock(_propertyBlock);
        }

        public void SetEmissionIntensity(float intensity)
        {
            _propertyBlock.SetColor(EmissionColor, Color.white * intensity);
            _meshRenderer.SetPropertyBlock(_propertyBlock);
        }
        #endregion
    }
}