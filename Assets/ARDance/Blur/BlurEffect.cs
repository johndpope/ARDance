using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurEffect : MonoBehaviour
{
    [SerializeField] private Texture _texture;
    [SerializeField] private Shader _shader;
    [SerializeField, Range(1f, 10f)] private float _offset;
    [SerializeField, Range(10f, 1000f)] private float _blur;

    private Material _material;
    private Renderer _renderer;
    private RenderTexture _rt1;
    private RenderTexture _rt2;
    private float[] _weights = new float[10];
    private bool _isInitialized;
    
    private void Awake()
    {
        Initialize();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        UpdateWeights();
        Blur();
    }
    
    private void Initialize()
    {
        if (_isInitialized) return;
        _material = new Material(_shader);
        _material.hideFlags = HideFlags.HideAndDontSave;

        // Down scale
        _rt1 = new RenderTexture(_texture.width / 2, _texture.height / 2, 0, RenderTextureFormat.ARGB32);
        _rt2 = new RenderTexture(_texture.width / 2, _texture.height / 2, 0, RenderTextureFormat.ARGB32);

        _renderer = GetComponent<Renderer>();
        UpdateWeights();
        _isInitialized = true;
    }
    
    public void Blur()
    {
        if (!_isInitialized) Initialize();

        Graphics.Blit(_texture, _rt1);
        _material.SetFloatArray("_Weights", _weights);

        float x = _offset / _rt1.width;
        float y = _offset / _rt1.height;

        // for horizontal blur.
        _material.SetVector("_Offsets", new Vector4(x, 0, 0, 0));

        Graphics.Blit(_rt1, _rt2, _material);

        // for vertical blur.
        _material.SetVector("_Offsets", new Vector4(0, y, 0, 0));

        Graphics.Blit(_rt2, _rt1, _material);

        _renderer.material.mainTexture = _rt1;
    }
    
    private void UpdateWeights()
    {
        float total = 0;
        float d = _blur * _blur * 0.001f;

        for (int i = 0; i < _weights.Length; i++)
        {
            // Offset position per x.
            float x = i * 2f;
            float w = Mathf.Exp(-0.5f * (x * x) / d);
            _weights[i] = w;

            if (i > 0)
            {
                w *= 2.0f;
            }

            total += w;
        }

        for (int i = 0; i < _weights.Length; i++)
        {
            _weights[i] /= total;
        }
    }
}
