using System.Collections;
using UnityEngine;

namespace HokoriYoke
{
    [RequireComponent(typeof(AudioSource))]
    public class EffectManager : MonoBehaviour
    {
        [SerializeField] private HumanOnVirtualBG _humanOnVirtualBg;
        [SerializeField] private BuildingCreater _buildingCreaterLeft;
        [SerializeField] private BuildingCreater _buildingCreaterRight;
        [SerializeField] private CountDown _countDown;
        [SerializeField] private GameObject _canvas;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void EffectStart(int type)
        {
            switch (type)
            {
                case 0:
                    StartCoroutine(Schedule());
                    break;
                case 1:
                    StartCoroutine(Schedule1());
                    break;
                case 2:
                    StartCoroutine(Schedule2());
                    break;
            }
        }

        private IEnumerator Schedule()
        {
            while (gameObject.activeSelf)
            {
                _canvas.SetActive(true);
                _countDown.StartCountDown(null, 7);
                yield return new WaitForSeconds(7f);
                _canvas.SetActive(false);
                _audioSource.Play();
                yield return new WaitForSeconds(1f);
                _humanOnVirtualBg.SetEffect(HumanOnVirtualBG.EffectType.Galaxy);
                yield return new WaitForSeconds(6f);
                _humanOnVirtualBg.SetEffect(HumanOnVirtualBG.EffectType.Grid);
                yield return new WaitForSeconds(4f);
                _humanOnVirtualBg.SetEffect(HumanOnVirtualBG.EffectType.CRT);
                yield return new WaitForSeconds(5f);
                _humanOnVirtualBg.SetEffect(HumanOnVirtualBG.EffectType.CameraFeed);
                _buildingCreaterLeft.ReCreate();
                _buildingCreaterRight.ReCreate();
            }
        }
        
        private IEnumerator Schedule1()
        {
            while (gameObject.activeSelf)
            {
                _canvas.SetActive(true);
                _countDown.StartCountDown(null, 7);
                yield return new WaitForSeconds(7f);
                _canvas.SetActive(false);
                _audioSource.Play();
                yield return new WaitForSeconds(1f);
                _humanOnVirtualBg.SetEffect(HumanOnVirtualBG.EffectType.Galaxy);
                yield return new WaitForSeconds(15f);
                _humanOnVirtualBg.SetEffect(HumanOnVirtualBG.EffectType.CameraFeed);
                _buildingCreaterLeft.ReCreate();
                _buildingCreaterRight.ReCreate();
            }
        }
        
        private IEnumerator Schedule2()
        {
            while (gameObject.activeSelf)
            {
                _canvas.SetActive(true);
                _countDown.StartCountDown(null, 7);
                yield return new WaitForSeconds(7f);
                _canvas.SetActive(false);
                _audioSource.Play();
                yield return new WaitForSeconds(1f);
                _humanOnVirtualBg.SetEffect(HumanOnVirtualBG.EffectType.Galaxy);
                yield return new WaitForSeconds(10f);
                _humanOnVirtualBg.SetEffect(HumanOnVirtualBG.EffectType.CRT);
                yield return new WaitForSeconds(5f);
                _humanOnVirtualBg.SetEffect(HumanOnVirtualBG.EffectType.CameraFeed);
                _buildingCreaterLeft.ReCreate();
                _buildingCreaterRight.ReCreate();
            }
        }
    }
}
