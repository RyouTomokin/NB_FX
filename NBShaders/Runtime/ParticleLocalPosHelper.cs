using UnityEngine;

namespace NBShader
{
    /// <summary>
    /// 粒子系统局部坐标辅助脚本
    /// 用于在粒子系统中使用局部坐标作为UV，确保旋转和缩放时贴图不会被拉伸
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleLocalPosHelper : MonoBehaviour
    {
        private ParticleSystem particleSystem;
        private ParticleSystemRenderer particleRenderer;
        private MaterialPropertyBlock materialPropertyBlock;
        
        // Shader属性ID（缓存以提高性能）
        private static readonly int ParticleWorldToLocalMatrix0Id = Shader.PropertyToID("_ParticleWorldToLocalMatrix0");
        private static readonly int ParticleWorldToLocalMatrix1Id = Shader.PropertyToID("_ParticleWorldToLocalMatrix1");
        private static readonly int ParticleWorldToLocalMatrix2Id = Shader.PropertyToID("_ParticleWorldToLocalMatrix2");
        private static readonly int ParticleWorldToLocalMatrix3Id = Shader.PropertyToID("_ParticleWorldToLocalMatrix3");
        
        private void Awake()
        {
            Initialize();
        }
        
        private void OnEnable()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            if (particleSystem == null)
            {
                particleSystem = GetComponent<ParticleSystem>();
            }
            
            if (particleRenderer == null)
            {
                particleRenderer = GetComponent<ParticleSystemRenderer>();
            }
            
            if (materialPropertyBlock == null)
            {
                materialPropertyBlock = new MaterialPropertyBlock();
            }
        }
        
        private void LateUpdate()
        {
            UpdateWorldToLocalMatrix();
        }
        
        /// <summary>
        /// 更新WorldToLocal矩阵到Shader
        /// </summary>
        private void UpdateWorldToLocalMatrix()
        {
            if (particleRenderer == null || particleSystem == null)
            {
                return;
            }
            
            // 获取粒子系统的WorldToLocal矩阵
            Matrix4x4 worldToLocal = transform.worldToLocalMatrix;
            
            // 设置到MaterialPropertyBlock（粒子系统通常使用PropertyBlock）
            // 注意：使用SetVector逐行传递，避免Unity的SetMatrix自动转置问题
            particleRenderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetVector(ParticleWorldToLocalMatrix0Id, worldToLocal.GetRow(0));
            materialPropertyBlock.SetVector(ParticleWorldToLocalMatrix1Id, worldToLocal.GetRow(1));
            materialPropertyBlock.SetVector(ParticleWorldToLocalMatrix2Id, worldToLocal.GetRow(2));
            materialPropertyBlock.SetVector(ParticleWorldToLocalMatrix3Id, worldToLocal.GetRow(3));
            particleRenderer.SetPropertyBlock(materialPropertyBlock);
            
            // 同时更新材质本身（如果材质不是实例化的）
            Material[] materials = particleRenderer.sharedMaterials;
            if (materials != null && materials.Length > 0)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] != null)
                    {
                        materials[i].SetVector(ParticleWorldToLocalMatrix0Id, worldToLocal.GetRow(0));
                        materials[i].SetVector(ParticleWorldToLocalMatrix1Id, worldToLocal.GetRow(1));
                        materials[i].SetVector(ParticleWorldToLocalMatrix2Id, worldToLocal.GetRow(2));
                        materials[i].SetVector(ParticleWorldToLocalMatrix3Id, worldToLocal.GetRow(3));
                    }
                }
            }
        }
        
        /// <summary>
        /// 手动更新矩阵（可以在外部调用，例如在动画或脚本中）
        /// </summary>
        public void RefreshMatrix()
        {
            UpdateWorldToLocalMatrix();
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            // 在编辑器中，当组件被修改时也更新
            if (Application.isPlaying)
            {
                UpdateWorldToLocalMatrix();
            }
        }
#endif
    }
}

