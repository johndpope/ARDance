using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class HumanSegmentationEffectBase : MonoBehaviour
{

    [SerializeField] protected AROcclusionManager _arOcclusionManager;
    [SerializeField] protected bool _useStencilForSegmentation = true;
    [SerializeField] protected bool _useDepthForSegmentation;

    protected bool _isSegementationActive = true;
    protected UVAdjuster _uvAdjuster;
    protected List<Material> _humanSegmentMats;
    protected List<Material> _cameraFeedMats;

    private UniversalAdditionalCameraData _universalAdditionalCameraData;
    private readonly int k_DisplayTransformId = Shader.PropertyToID("_UnityDisplayTransform");

    protected virtual void Awake()
    {
        _uvAdjuster = new UVAdjuster();
    }

    protected virtual void Update()
    {
        if (_isSegementationActive)
        {
            if (_useStencilForSegmentation)
            {
                var humanStencil = _arOcclusionManager.humanStencilTexture;
                if (humanStencil)
                {
                    foreach (var material in _humanSegmentMats)
                    {
                        _uvAdjuster.SetMaterialProperty(material, humanStencil, UVAdjuster.TextureType.Stencil);
                    }
                }
            }
        
            if (_useDepthForSegmentation)
            {
                var humanDepth = _arOcclusionManager.humanDepthTexture;
                if (humanDepth)
                {
                    foreach (var material in _humanSegmentMats)
                    {
                        _uvAdjuster.SetMaterialProperty(material, humanDepth, UVAdjuster.TextureType.Depth);
                    }
                }
            }
        }
    }
    
    protected void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        foreach (var material in _cameraFeedMats)
        {
            var count = eventArgs.textures.Count;
            for (int i = 0; i < count; ++i)
            {
                material.SetTexture(eventArgs.propertyNameIds[i], eventArgs.textures[i]);
            }

            if (eventArgs.displayMatrix.HasValue)
            {
                material.SetMatrix(k_DisplayTransformId, eventArgs.displayMatrix.Value);
            }
        }
    }
    
    protected void ChangeCameraRenderer(int index)
    {
        if (_universalAdditionalCameraData == null)
        {
            _universalAdditionalCameraData = GetComponent<UniversalAdditionalCameraData>();
        }
        _universalAdditionalCameraData.SetRenderer(index);
    }

    #region Debug
    [SerializeField] protected Text _debug;
    #endregion
}
