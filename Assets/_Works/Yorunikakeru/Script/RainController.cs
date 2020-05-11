using System;
using UnityEngine;
using UnityEngine.VFX;
using DG.Tweening;

namespace Yorunikakeru
{
    public class RainController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _rainParticle;
        [SerializeField] private VisualEffect _rainVfx;
        
        private ParticleSystem.EmissionModule _emissionModule;

        private const int INITIAL_PS_COUNT = 5;
        private const int END_PS_COUNT = 40;
        private const int INITIAL_VFX_COUNT = 50;
        private const int END_VFX_COUNT = 1000;
        private const float INCREASE_DURATION = 6f;
        
        private readonly int PropertyID_ParticleCount = Shader.PropertyToID("ParticleCount");

        private void Awake()
        {
            _emissionModule = _rainParticle.emission;
        }
        
        public void Reset()
        {
            _emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(INITIAL_PS_COUNT);
            _rainVfx.SetInt(PropertyID_ParticleCount, INITIAL_VFX_COUNT);
            _rainParticle.gameObject.SetActive(false);
            _rainVfx.gameObject.SetActive(false);
        }
        
        public void StartRain()
        {
            IncreasePSRainCount();
            DOVirtual.DelayedCall(2f, () => IncreaseVfxRainCount());
        }

        public void StopRain()
        {
            _rainParticle.gameObject.SetActive(false);
            _rainVfx.gameObject.SetActive(false);
        }

        private void IncreasePSRainCount()
        {
            _rainParticle.gameObject.SetActive(true);
            var value = INITIAL_PS_COUNT;
            DOTween.To(() => value, num => value = num, END_PS_COUNT, INCREASE_DURATION)
                .SetEase(Ease.InSine)
                .OnUpdate(() => _emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(value));
        }
        
        private void IncreaseVfxRainCount()
        {
            _rainVfx.gameObject.SetActive(true);
            var value = INITIAL_VFX_COUNT;
            DOTween.To(() => value, num => value = num, END_VFX_COUNT, INCREASE_DURATION)
                .SetEase(Ease.InSine)
                .OnUpdate(() => _rainVfx.SetInt(PropertyID_ParticleCount, value));
        }
    }
}
