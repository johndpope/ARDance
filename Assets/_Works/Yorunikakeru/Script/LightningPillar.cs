using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.VFX;

namespace Yorunikakeru
{
    public class LightningPillar : MonoBehaviour
    {
        [SerializeField] private Transform _pillar;
        [SerializeField] private VisualEffect _strip;
        [SerializeField] private GameObject _centerHuman;
        [SerializeField] private Light _spotLight1;
        [SerializeField] private Light _spotLight2;

        private Ripple _ripple;
        
        private const float INITIAL_PILLAR_POSY = 16f;
        private const float END_PILLAR_POSY = -4f;
        private const float INITIAL_PILLAR_SCALEX = 3f;
        private const float END_PILLAR_SCALEX = 0f;
        private const int INITIAL_STRIP_SPAWNRATE = 10;
        private const int END_STRIP_SPAWNRATE = 0;
        private const float INITIAL_SL2_POSZ = 11.11f;
        private const float END_SL2_POSZ = 2f;
        private const float SHOWING_DURATION = 1.5f;

        private readonly int PropertyID_SpawnRate = Shader.PropertyToID("SpawnRate");

        private void Awake()
        {
            _ripple = GetComponent<Ripple>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Show();
            }
            
            if (Input.GetKeyDown(KeyCode.C))
            {
                Reset();
            }
        }

        public void Reset()
        {
            _pillar.position = new Vector3(_pillar.position.x, INITIAL_PILLAR_POSY, _pillar.position.z);
            _pillar.localScale = new Vector3(INITIAL_PILLAR_SCALEX, _pillar.localScale.y, _pillar.localScale.z);
            _pillar.gameObject.SetActive(false);
            _strip.SetInt(PropertyID_SpawnRate, INITIAL_STRIP_SPAWNRATE);
            _strip.gameObject.SetActive(false);
            _centerHuman.SetActive(false);
            _spotLight1.gameObject.SetActive(false);
            _spotLight2.transform.position = new Vector3(_spotLight2.transform.position.x, _spotLight2.transform.position.y, INITIAL_SL2_POSZ);
            _spotLight2.gameObject.SetActive(false);
        }

        public void Show()
        {
            ShowPillar();
            ShowStrip();
            ShowHuman();
            Ripple();
        }

        private void ShowPillar()
        {
            _pillar.gameObject.SetActive(true);
            _pillar.DOMoveY(END_PILLAR_POSY, SHOWING_DURATION).SetEase(Ease.OutCirc);
            _pillar.DOScaleX(END_PILLAR_SCALEX, SHOWING_DURATION)
                .SetEase(Ease.InCubic);
        }

        private void ShowStrip()
        {
            _strip.gameObject.SetActive(true);
            var value = INITIAL_STRIP_SPAWNRATE;
            DOVirtual.DelayedCall(SHOWING_DURATION * 4f / 5f,() => _strip.SetInt(PropertyID_SpawnRate, END_STRIP_SPAWNRATE))
                .OnComplete(() =>
                {
                    DOVirtual.DelayedCall(1.5f, () => _strip.gameObject.SetActive(false));
                });
        }

        private void ShowHuman()
        {
            StartCoroutine(ShowHumanSchedule());
        }

        private IEnumerator ShowHumanSchedule()
        {
            yield return new WaitForSeconds(0.3f);
            _centerHuman.SetActive(true);
            _spotLight2.gameObject.SetActive(true);
            _spotLight2.transform.DOMoveZ(END_SL2_POSZ, SHOWING_DURATION)
                .SetEase(Ease.Linear)
                .OnComplete(() => _spotLight2.gameObject.SetActive(false));
            yield return new WaitForSeconds(SHOWING_DURATION / 4f);
            _spotLight1.gameObject.SetActive(true);
        }

        private void Ripple()
        {
            DOVirtual.DelayedCall(0.1f, () =>
            {
                _ripple.Pulse(_pillar.position, Yorunikakeru.Ripple.PulseStrength.Strong);
            });
        }
    }
}