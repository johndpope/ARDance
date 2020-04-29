using UnityEngine;

public class RippleWave : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private Texture _inputTex;
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private RenderTexture _heightMap;
    
    private RenderTexture[] _buffer = new RenderTexture[3];
    private int _currentTargetIdx;

    private readonly int PropertyID_TextureWidth = Shader.PropertyToID("_TextureWidth");
    private readonly int PropertyID_TextureHeight = Shader.PropertyToID("_TextureHeight");
    private readonly int PropertyID_Prev_1 = Shader.PropertyToID("_Prev_1");
    private readonly int PropertyID_Prev_2= Shader.PropertyToID("_Prev_2");
    private readonly int PropertyID_PulseUV= Shader.PropertyToID("_PulseUV");
    
    private readonly int PropertyID_HeightMap = Shader.PropertyToID("_HeightMap");

    private void Start()
    {
        for (int i = 0; i < _buffer.Length; ++i)
        {
            //_buffer[i] = new RenderTexture(1125, 2436, 0);
            _buffer[i] = new RenderTexture(_heightMap.width, _heightMap.height, _heightMap.depth, _heightMap.format);
            _buffer[i].wrapMode = TextureWrapMode.Clamp;
            _buffer[i].Create();
        }

        _material.SetFloat(PropertyID_TextureWidth, 1125);
        _material.SetFloat(PropertyID_TextureHeight, 2436);
    }

    private void Update()
    {
        
        if (Input.GetMouseButtonUp(0))
        {
            var uv = new Vector2(Input.mousePosition.x / 1125f, Input.mousePosition.y / 2436f);
            _material.SetVector(PropertyID_PulseUV, uv);
        }
        else
        {
            _material.SetVector(PropertyID_PulseUV, new Vector4(-1, -1, 0, 0));
        }
        
        int prevIdx1 = (_currentTargetIdx - 1 + 3) % 3;
        int prevIdx2 = (_currentTargetIdx - 2 + 3) % 3;
        _material.SetTexture(PropertyID_Prev_1, _buffer[prevIdx1]);
        _material.SetTexture(PropertyID_Prev_2, _buffer[prevIdx2]);

        Graphics.Blit(_buffer[prevIdx1], _buffer[_currentTargetIdx], _material);
        targetRenderer.material.SetTexture(PropertyID_HeightMap, _buffer[_currentTargetIdx]);
        _currentTargetIdx = (_currentTargetIdx + 1) % 3;
    }
}
