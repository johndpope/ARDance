using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HumanRipple : HumanSegmentationEffectBase, IAddtionalPostProcess
{
    public Material MaterialForPostProcess => _ppMaterial;
    public bool IsEnable => true;
    
    [SerializeField] private Material _material;
    [SerializeField] private Material _ppMaterial;

    private RenderTexture[] _buffer = new RenderTexture[3];
    private int _currentTargetIdx;

    private readonly int PropertyID_TextureWidth = Shader.PropertyToID("_TextureWidth");
    private readonly int PropertyID_TextureHeight = Shader.PropertyToID("_TextureHeight");
    private readonly int PropertyID_Prev_1 = Shader.PropertyToID("_Prev_1");
    private readonly int PropertyID_Prev_2 = Shader.PropertyToID("_Prev_2");
    private readonly int PropertyID_Speed = Shader.PropertyToID("_Speed");
    private readonly int PropertyID_Attenuation = Shader.PropertyToID("_Attenuation");
    private readonly int PropertyID_RippleTex = Shader.PropertyToID("_RippleTex");

    private void Start()
    {
        for (int i = 0; i < _buffer.Length; ++i)
        {
            _buffer[i] = new RenderTexture(Screen.width, Screen.height, 0);
            _buffer[i].wrapMode = TextureWrapMode.Clamp;
            _buffer[i].Create();
        }

        _material.SetFloat(PropertyID_TextureWidth, Screen.width);
        _material.SetFloat(PropertyID_TextureHeight, Screen.height);
        
        _humanSegmentMats = new List<Material>
        {
            _material, _ppMaterial
        };
    }

    protected override void Update()
    {
        base.Update();
        
        int prevIdx1 = (_currentTargetIdx - 1 + 3) % 3;
        int prevIdx2 = (_currentTargetIdx - 2 + 3) % 3;
        _material.SetTexture(PropertyID_Prev_1, _buffer[prevIdx1]);
        _material.SetTexture(PropertyID_Prev_2, _buffer[prevIdx2]);

        Graphics.Blit(_buffer[prevIdx1], _buffer[_currentTargetIdx], _material);
        _ppMaterial.SetTexture(PropertyID_RippleTex, _buffer[_currentTargetIdx]);
        _currentTargetIdx = (_currentTargetIdx + 1) % 3;
    }

    public void ChangeSpeed(Slider slider)
    {
        _material.SetFloat(PropertyID_Speed, slider.value);
    }
    
    public void ChangeAttenuation(Slider slider)
    {
        _material.SetFloat(PropertyID_Attenuation, slider.value);
    }
}
