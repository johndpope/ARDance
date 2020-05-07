using System;
using DG.Tweening;
using UnityEngine;

namespace Yorunikakeru
{
    public class EventManager : MonoBehaviour
    {
        [SerializeField] private GameObject _centerHuman;
        [SerializeField] private GameObject _rightHuman;
        [SerializeField] private GameObject _leftHuman;
        [SerializeField] private GameObject _originHuman;

        [SerializeField] private GameObject _scatterVfx;
        
        [SerializeField] private Material[] _firstTeleportMats = new Material[2];
        [SerializeField] private Material[] _secondTeleportMats = new Material[2];
        [SerializeField] private Material[] _thirdTeleportMats = new Material[2];

        private readonly int PropertyID_DistortionPower = Shader.PropertyToID("_DistortionPower");
        private int _teleportIndex;

        private void Start()
        {
            Reset();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Reset();
            }
        
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                FirstTeleport();
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SecondTeleport();
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ThirdTeleport();
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ScatterEffect();
            }
        }

        private void Reset()
        {
            _teleportIndex = 0;
            _centerHuman.SetActive(true); 
            _rightHuman.SetActive(false);
            _leftHuman.SetActive(false);
            _originHuman.SetActive(false);
            _firstTeleportMats[0].SetFloat(PropertyID_DistortionPower, 0f);
            _firstTeleportMats[1].SetFloat(PropertyID_DistortionPower, 0.45f);
            _secondTeleportMats[1].SetFloat(PropertyID_DistortionPower, -0.4f);
            _thirdTeleportMats[1].SetFloat(PropertyID_DistortionPower, 0.3f);
            _scatterVfx.SetActive(false);
        }

        public void Teleport()
        {
            if (_teleportIndex == 0)
            {
                FirstTeleport();
            }
            else if (_teleportIndex == 1)
            {
                SecondTeleport();
            }
            else
            {
                ThirdTeleport();
            }
            _teleportIndex++;
        }
        
        private void FirstTeleport()
        {
            Move2Single(_firstTeleportMats[0], 0, -0.4f, 0.1f, Ease.InSine).OnComplete(() => 
            { 
                _centerHuman.SetActive(false); 
                _rightHuman.SetActive(true);
                Move2Single(_firstTeleportMats[1], 0.45f, 0f, 0.1f, Ease.OutSine);
            });
        }
        
        private void SecondTeleport()
        {
            Move2Single(_secondTeleportMats[0], 0, 0.45f, 0.1f, Ease.InSine).OnComplete(() => 
            { 
                _rightHuman.SetActive(false); 
                _leftHuman.SetActive(true);
                Move2Single(_secondTeleportMats[1], -0.4f, 0f, 0.1f, Ease.OutSine);
            });
        }
        
        private void ThirdTeleport()
        {
            Move2Single(_thirdTeleportMats[0], 0, -0.4f, 0.1f, Ease.InSine).OnComplete(() => 
            { 
                _leftHuman.SetActive(false); 
                _originHuman.SetActive(true);
                Move2Single(_thirdTeleportMats[1], 0.3f, 0f, 0.1f, Ease.OutSine);
            });
        }
        
        private Tween Move2Single(Material material, float start, float end, float duration, Ease ease)
        {
            var value = start;
            return DOTween.To(() => value, num => value = num, end, duration)
                .SetEase(ease)
                .OnUpdate(() => material.SetFloat(PropertyID_DistortionPower, value));
        }

        public void ScatterEffect()
        {
            _scatterVfx.SetActive(true);
            DOVirtual.DelayedCall(0.05f, () => _originHuman.SetActive(false));
        }
    }
}