using System.Collections;
using UnityEngine;

namespace Yorunikakeru
{
    public class Ripple : MonoBehaviour
    {
        [SerializeField] private Material _rippleCulcMat;
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
                _buffer[i] = new RenderTexture(_heightMap.width, _heightMap.height, _heightMap.depth, _heightMap.format);
                _buffer[i].wrapMode = TextureWrapMode.Clamp;
                _buffer[i].Create();
            }

            _rippleCulcMat.SetFloat(PropertyID_TextureWidth, _heightMap.width);
            _rippleCulcMat.SetFloat(PropertyID_TextureHeight, _heightMap.height);
            _rippleCulcMat.SetVector(PropertyID_PulseUV, new Vector2(-10, -10));
        }

        public void PutPulse(Vector2 pulse)
        {
            StartCoroutine(HandlePulse(pulse));
        }

        private IEnumerator HandlePulse(Vector2 pulse)
        {
            _rippleCulcMat.SetVector(PropertyID_PulseUV, pulse);
            yield return null;
            _rippleCulcMat.SetVector(PropertyID_PulseUV, new Vector2(-10, -10));
        }

        private void Update()
        {
            int prevIdx1 = (_currentTargetIdx - 1 + 3) % 3;
            int prevIdx2 = (_currentTargetIdx - 2 + 3) % 3;
            _rippleCulcMat.SetTexture(PropertyID_Prev_1, _buffer[prevIdx1]);
            _rippleCulcMat.SetTexture(PropertyID_Prev_2, _buffer[prevIdx2]);

            Graphics.Blit(_buffer[prevIdx1], _buffer[_currentTargetIdx], _rippleCulcMat);
            targetRenderer.material.SetTexture(PropertyID_HeightMap, _buffer[_currentTargetIdx]);
            _currentTargetIdx = (_currentTargetIdx + 1) % 3;
        }
    }
}
