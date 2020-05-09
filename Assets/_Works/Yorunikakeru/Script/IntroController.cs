using UnityEngine;
using DG.Tweening;

namespace Yorunikakeru
{
    public class IntroController : MonoBehaviour
    {
        [SerializeField] private GameObject _directionLight;
        [SerializeField] private Light _spotLight;
        [SerializeField] private GameObject _volumeLight;
        [SerializeField] private Material[] _floorMats;
        [SerializeField] private Material _humanLitMaterial;
        [SerializeField] private Material _volumeLightMat;

        private Renderer _floor;
        private Renderer _humanRen;

        private const float INITIAL_VOLUMELIGHT_WIDTH = 0.086f;
        private const float END_VOLUMELIGHT_WIDTH = 0.7f;
        private const float INITIAL_FlOORLIGHT_RADIUS = 1.53f;
        private const float END_FlOORLIGHT_RADIUS = 10f;
        private const float INITIAL_SPOTLIGHT_INTENSITY = 16.4f;
        private const float END_SPOTLIGHT_INTENSITY = 3f;
        private const float DIFFUSION_DURATION = 3f;

        private readonly int PropertyID_ConeWidth = Shader.PropertyToID("_ConeWidth");
        private readonly int PropertyID_Radius = Shader.PropertyToID("_Radius");
        private readonly int PropertyID_ColorPower = Shader.PropertyToID("_ColorPower");
        
        public void Init(Renderer floor, Renderer humanRen)
        {
            _floor = floor;
            _humanRen = humanRen;
        }

        public void Reset()
        {
            _humanRen.material = _humanLitMaterial;
            _spotLight.intensity = INITIAL_SPOTLIGHT_INTENSITY;
            _directionLight.SetActive(true);
            _spotLight.gameObject.SetActive(false);
            _volumeLight.SetActive(false);
            _floor.material = _floorMats[0];
            _volumeLightMat.SetFloat(PropertyID_ConeWidth, INITIAL_VOLUMELIGHT_WIDTH);
            _floorMats[1].SetFloat(PropertyID_Radius, INITIAL_FlOORLIGHT_RADIUS);
            _floorMats[1].SetFloat(PropertyID_ColorPower, 1f);
        }

        public void LightUp()
        {
            _directionLight.SetActive(false);
            _spotLight.gameObject.SetActive(true);
            _volumeLight.SetActive(true);
            _floor.material = _floorMats[1];
        }

        public void LightDiffuse()
        {
            DiffuseVolumeLight();
            DiffuseFloorLight();
            WeakenSpotLight();
        }

        private void DiffuseVolumeLight()
        {
            var value = INITIAL_VOLUMELIGHT_WIDTH;
            DOTween.To(() => value, num => value = num, END_VOLUMELIGHT_WIDTH, DIFFUSION_DURATION)
                .SetEase(Ease.Linear)
                .OnUpdate(() => _volumeLightMat.SetFloat(PropertyID_ConeWidth, value))
                .OnComplete(() => _volumeLight.SetActive(false));
        }
        
        private void DiffuseFloorLight()
        {
            var value1 = INITIAL_FlOORLIGHT_RADIUS;
            DOTween.To(() => value1, num => value1 = num, END_FlOORLIGHT_RADIUS * 3, DIFFUSION_DURATION * 3)
                .SetEase(Ease.Linear)
                .OnUpdate(() => _floor.material.SetFloat(PropertyID_Radius, value1));

            var value2 = 1f;
            DOTween.To(() => value2, num => value2 = num, 0f, DIFFUSION_DURATION)
                .SetEase(Ease.Linear)
                .OnUpdate(() => _floor.material.SetFloat(PropertyID_ColorPower, value2));
        }
        
        private void WeakenSpotLight()
        {
            var value = INITIAL_SPOTLIGHT_INTENSITY;
            DOTween.To(() => value, num => value = num, END_SPOTLIGHT_INTENSITY, DIFFUSION_DURATION)
                .SetEase(Ease.Linear)
                .OnUpdate(() => _spotLight.intensity = value);
        }
    }
}
