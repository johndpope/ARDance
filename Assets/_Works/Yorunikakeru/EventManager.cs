using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Yorunikakeru
{
    public class EventManager : MonoBehaviour
    {
        [SerializeField] private Renderer _centerHuman;
        [SerializeField] private Renderer _rightHuman;
        [SerializeField] private Renderer _leftHuman;
        [SerializeField] private Renderer _originHuman;
        [SerializeField] private GameObject _scatterVfx;
        [SerializeField] private VfxTestIntegrater _vfxIntegrater;

        private Teleport _teleport;
        private int _teleportIndex;
        private Ripple _ripple;

        private void Awake()
        {
            _teleport = GetComponent<Teleport>();
            _teleport.Init(_centerHuman, _rightHuman, _leftHuman, _originHuman);
            _ripple = GetComponent<Ripple>();
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
                _ripple.PutPulse(new Vector2(
                    -_centerHuman.transform.position.x / 200f + 0.5f,
                    -_centerHuman.transform.position.z / 200f + 0.5f
                    ));
//                _ripple.PutPulse(new Vector2(0.7f, 0.7f));
            }
        }

        private void Reset()
        {
            _teleportIndex = 0;
            _teleport.Reset();
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
            _teleport.FirstTeleport(
                () => 
                    _ripple.PutPulse(new Vector2(
                        -_centerHuman.transform.position.x / 200f + 0.5f,
                        -_centerHuman.transform.position.z / 200f + 0.5f)),
                () => 
                    _ripple.PutPulse(new Vector2(
                        -_rightHuman.transform.position.x / 200f + 0.5f, 
                        -_rightHuman.transform.position.z / 200f + 0.5f))
            );
        }

        private void SecondTeleport()
        {
            _teleport.SecondTeleport(
                () => 
                    _ripple.PutPulse(new Vector2(
                        -_rightHuman.transform.position.x / 200f + 0.5f,
                        -_rightHuman.transform.position.z / 200f + 0.5f)),
                () => 
                    _ripple.PutPulse(new Vector2(
                        -_leftHuman.transform.position.x / 200f + 0.5f, 
                        -_leftHuman.transform.position.z / 200f + 0.5f)));
        }
        
        private void ThirdTeleport()
        {
            _teleport.ThirdTeleport(
                () => 
                    _ripple.PutPulse(new Vector2(
                        -_leftHuman.transform.position.x / 200f + 0.5f,
                        -_leftHuman.transform.position.z / 200f + 0.5f)),
                () => 
                    _ripple.PutPulse(new Vector2(
                        -_originHuman.transform.position.x / 200f + 0.5f, 
                        -_originHuman.transform.position.z / 200f + 0.5f)));
        }

        public void ScatterEffect()
        {
            StartCoroutine(Scatter());
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