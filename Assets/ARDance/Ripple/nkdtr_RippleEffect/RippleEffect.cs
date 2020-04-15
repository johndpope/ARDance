using UnityEngine;
using UnityEngine.UI;

namespace nkdtr
{
    public class RippleEffect : MonoBehaviour, IAddtionalPostProcess
    {
        public Material MaterialForPostProcess => _material;
        public bool IsEnable => true;

        [SerializeField] private Material _material;
        [SerializeField] private Texture _inputTex;

        private RenderTexture[] _waveBuf = new RenderTexture[3];
        private int _currentTargetIdx = 0;

        public Renderer targetRenderer;

        private void Start()
        {
            for (int i = 0; i < 3; ++i)
            {
                _waveBuf[i] = new RenderTexture(1024, 1024, 24);
                _waveBuf[i].wrapMode = TextureWrapMode.Clamp;
                _waveBuf[i].Create();
            }

            _material.SetTexture("_draw", _inputTex);
        }

        private void Update()
        {
            int prevIdx1 = (_currentTargetIdx - 1 + 3) % 3;
            int prevIdx2 = (_currentTargetIdx - 2 + 3) % 3;
            _material.SetTexture("_prev_1", _waveBuf[prevIdx1]);
            _material.SetTexture("_prev_2", _waveBuf[prevIdx2]);

            Graphics.Blit(_waveBuf[prevIdx1], _waveBuf[_currentTargetIdx], _material);

            targetRenderer.material.mainTexture = _waveBuf[_currentTargetIdx];

            _currentTargetIdx = (_currentTargetIdx + 1) % 3;
        }
    }
}
