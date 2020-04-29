using UnityEngine;

public class Ripple : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private Texture _inputTex;
    [SerializeField] private Renderer targetRenderer;
    
    private RenderTexture[] _buffer = new RenderTexture[3];
    private int _currentTargetIdx;

    private readonly int PropertyID_TextureWidth = Shader.PropertyToID("_TextureWidth");
    private readonly int PropertyID_TextureHeight = Shader.PropertyToID("_TextureHeight");
    private readonly int PropertyID_TriggerTexture = Shader.PropertyToID("_TriggerTexture");
    private readonly int PropertyID_Prev_1 = Shader.PropertyToID("_Prev_1");
    private readonly int PropertyID_Prev_2= Shader.PropertyToID("_Prev_2");
    
    private readonly int PropertyID_RippleTex = Shader.PropertyToID("_RippleTex");

    private void Start()
    {
        for (int i = 0; i < _buffer.Length; ++i)
        {
            _buffer[i] = new RenderTexture(1125, 2436, 0);
            _buffer[i].wrapMode = TextureWrapMode.Clamp;
            _buffer[i].Create();
        }

        _material.SetFloat(PropertyID_TextureWidth, 1125);
        _material.SetFloat(PropertyID_TextureHeight, 2436);
        _material.SetTexture(PropertyID_TriggerTexture, _inputTex);
    }

    private void Update()
    {
        int prevIdx1 = (_currentTargetIdx - 1 + 3) % 3;
        int prevIdx2 = (_currentTargetIdx - 2 + 3) % 3;
        _material.SetTexture(PropertyID_Prev_1, _buffer[prevIdx1]);
        _material.SetTexture(PropertyID_Prev_2, _buffer[prevIdx2]);

        Graphics.Blit(_buffer[prevIdx1], _buffer[_currentTargetIdx], _material);
        targetRenderer.material.SetTexture(PropertyID_RippleTex, _buffer[_currentTargetIdx]);
        _currentTargetIdx = (_currentTargetIdx + 1) % 3;
    }
}
