using System;
using DG.Tweening;
using UnityEngine;

namespace Yorunikakeru
{
    public class Teleport : MonoBehaviour
    {
        
        [SerializeField] private Material[] _firstTeleportMats = new Material[2];
        [SerializeField] private Material[] _secondTeleportMats = new Material[2];
        [SerializeField] private Material[] _thirdTeleportMats = new Material[2];
        
        private Renderer _centerHuman;
        private Renderer _rightHuman;
        private Renderer _leftHuman;
        private Renderer _originHuman;
        
        private const float DISTORTION_POWER_X = 0.45f;
        private const float DISTORTION_POWER_Y = 1f;
        private readonly int PropertyID_DistortionPower = Shader.PropertyToID("_DistortionPower");
        private readonly int PropertyID_ALPHA = Shader.PropertyToID("_Alpha");

        public void Init(Renderer centerHuman, Renderer rightHuman, Renderer leftHuman, Renderer originHuman)
        {
            _centerHuman = centerHuman;
            _rightHuman = rightHuman;
            _leftHuman = leftHuman;
            _originHuman = originHuman;
        }
        
        public void Reset()
        {
            _centerHuman.gameObject.SetActive(true); 
            _rightHuman.gameObject.SetActive(false);
            _leftHuman.gameObject.SetActive(false);
            _originHuman.gameObject.SetActive(false);
            _firstTeleportMats[0].SetFloat(PropertyID_DistortionPower, 0f);
            _firstTeleportMats[1].SetFloat(PropertyID_DistortionPower, DISTORTION_POWER_X);
            _secondTeleportMats[1].SetFloat(PropertyID_DistortionPower, -DISTORTION_POWER_X);
            _thirdTeleportMats[0].SetFloat(PropertyID_DistortionPower, 0f);
            _thirdTeleportMats[1].SetFloat(PropertyID_DistortionPower, -DISTORTION_POWER_Y);
            _leftHuman.material = _secondTeleportMats[1];
            
            _firstTeleportMats[0].SetFloat(PropertyID_ALPHA, 1f);
            _firstTeleportMats[1].SetFloat(PropertyID_ALPHA, 0f);
            _secondTeleportMats[1].SetFloat(PropertyID_ALPHA, 0f);
        }
        
        public void FirstTeleport(Action firstRipple, Action secondRipple)
        {
            _centerHuman.material = _firstTeleportMats[0];
            MoveSingle(_firstTeleportMats[0], 0, -DISTORTION_POWER_X, 0.1f, Ease.OutSine).OnComplete(() => 
            { 
                _centerHuman.gameObject.SetActive(false);
                firstRipple.Invoke();
            });
            SetAlpha(_firstTeleportMats[0], true, 0.1f);
            SetAlpha(_firstTeleportMats[1], false, 0.1f).SetDelay(0.08f);
            _rightHuman.gameObject.SetActive(true);
            MoveSingle(_firstTeleportMats[1], DISTORTION_POWER_X, 0f, 0.1f, Ease.OutSine)
                .SetDelay(0.08f)
                .OnComplete(() => secondRipple.Invoke());
            
        }
        
        public void SecondTeleport(Action firstRipple, Action secondRipple)
        {
            MoveSingle(_secondTeleportMats[0], 0, DISTORTION_POWER_X, 0.1f, Ease.OutSine).OnComplete(() => 
            { 
                _rightHuman.gameObject.SetActive(false);
                firstRipple.Invoke();
            });
            SetAlpha(_secondTeleportMats[0], true, 0.1f);
            SetAlpha(_secondTeleportMats[1], false, 0.1f).SetDelay(0.08f);
            _leftHuman.gameObject.SetActive(true);
            MoveSingle(_secondTeleportMats[1], -DISTORTION_POWER_X, 0f, 0.1f, Ease.OutSine)
                .SetDelay(0.08f)
                .OnComplete(() => secondRipple.Invoke());
        }
        
        
        public void ThirdTeleport(Action firstRipple, Action secondRipple)
        {
            _leftHuman.material = _thirdTeleportMats[0];
            MoveSingle(_thirdTeleportMats[0], 0, -DISTORTION_POWER_Y, 0.1f, Ease.OutSine).OnComplete(() => 
            { 
                _leftHuman.gameObject.SetActive(false); 
                _originHuman.gameObject.SetActive(true);
                firstRipple.Invoke();
                MoveSingle(_thirdTeleportMats[1], -DISTORTION_POWER_Y, 0f, 0.1f, Ease.OutSine)
                    .OnComplete(() => secondRipple.Invoke());
            });
        }
        
        private Tween MoveSingle(Material material, float start, float end, float duration, Ease ease)
        {
            var value = start;
            return DOTween.To(() => value, num => value = num, end, duration)
                .SetEase(ease)
                .OnUpdate(() => material.SetFloat(PropertyID_DistortionPower, value));
        }

        private Tween SetAlpha(Material material, bool isFadeOut, float duration)
        {
            var value = isFadeOut ? 1f : 0f;
            var end = isFadeOut ? 0f : 1f;
            var ease = isFadeOut ? Ease.InQuint : Ease.OutQuint;
            return DOTween.To(() => value, num => value = num, end, duration)
                .SetEase(ease)
                .OnUpdate(() => material.SetFloat(PropertyID_ALPHA, value));
        }
    }
}
