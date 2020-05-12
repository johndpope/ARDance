using System.Collections;
using UnityEngine;

namespace Yorunikakeru
{
    public class EventManager : MonoBehaviour
    {
        [SerializeField] private Renderer _centerHuman;
        [SerializeField] private Renderer _rightHuman;
        [SerializeField] private Renderer _leftHuman;
        [SerializeField] private Renderer _originHuman;
        [SerializeField] private Renderer _floor;
        [SerializeField] private GameObject _scatterVfx;
        [SerializeField] private VfxTestIntegrater _vfxIntegrater;

        private AudioSource _audioSource;
        private Teleport _teleport;
        private int _teleportIndex;
        private Ripple _ripple;
        private IntroController _introController;
        private RainController _rainController;
        private BigTransition _bigTransition;
        private LightningPillar _lightningPillar;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _teleport = GetComponent<Teleport>();
            _teleport.Init(_centerHuman, _rightHuman, _leftHuman, _originHuman);
            _ripple = GetComponent<Ripple>();
            _introController = GetComponent<IntroController>();
            _introController.Init(_floor, _centerHuman);
            _bigTransition = GetComponent<BigTransition>();
            _bigTransition.Init(_floor, _centerHuman);
            _rainController = GetComponent<RainController>();
            _lightningPillar = GetComponent<LightningPillar>();
        }

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
            
            if (Input.GetKeyDown(KeyCode.Z))
            {
                LightUp();
            }
            
            if (Input.GetKeyDown(KeyCode.X))
            {
                LightDiffuse();
            }
        }
        
        #region Signal Event
        public void PlayAudio()
        {
            _audioSource.Play();
        }
        
        public void LightUp()
        {
            _introController.LightUp();
        }
        
        public void LightDiffuse()
        {
            _introController.LightDiffuse();
        }

        public void StartRain()
        {
            _rainController.StartRain();
        }

        public void Transit()
        {
            _rainController.StopRain();
            _bigTransition.Transit();
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
        
        public void ScatterEffect()
        {
            StartCoroutine(Scatter());
        }
        
        public void LightningShowEffect()
        {
            _lightningPillar.Show();
        }
        #endregion

        private void Reset()
        {
            _teleportIndex = 0;
            _teleport.Reset();
            _scatterVfx.SetActive(false);
            _introController.Reset();
            _rainController.Reset();
            _lightningPillar.Reset();
        }

        private void FirstTeleport()
        {
            _teleport.FirstTeleport(
                () => _ripple.Pulse(_centerHuman.transform.position, Ripple.PulseStrength.Middle),
                () => _ripple.Pulse(_rightHuman.transform.position, Ripple.PulseStrength.Middle));
        }

        private void SecondTeleport()
        {
            _teleport.SecondTeleport(
                () => _ripple.Pulse(_rightHuman.transform.position, Ripple.PulseStrength.Middle),
                () => _ripple.Pulse(_leftHuman.transform.position, Ripple.PulseStrength.Middle));
        }
        
        private void ThirdTeleport()
        {
            _teleport.ThirdTeleport(
                () => _ripple.Pulse(_leftHuman.transform.position, Ripple.PulseStrength.Middle),
                () => _ripple.Pulse(_originHuman.transform.position, Ripple.PulseStrength.Middle));
        }

        private IEnumerator Scatter()
        {
            _vfxIntegrater.IsEnable = true;
            yield return new WaitForSeconds(0.1f);
            _scatterVfx.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            _originHuman.gameObject.SetActive(false);
            _vfxIntegrater.IsEnable = false;
        }
    }
}