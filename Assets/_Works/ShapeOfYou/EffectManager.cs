using System;
using System.Collections;
using Smrvfx;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShapeOfYou
{
    public class EffectManager : MonoBehaviour
    {
        [SerializeField] private GameObject _particle;
        [SerializeField] private GameObject _line;
        [SerializeField] private GameObject _lightening;
        [SerializeField] private Volume _volume;
        [SerializeField] private VolumeProfile _volumeProfile1;
        [SerializeField] private VolumeProfile _volumeProfile2;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private SkinnedMeshBaker _skinnedMeshBaker;
        [SerializeField] private HumanBodyTrackerForHumanoid _humanBodyTracker;

        private void Start()
        {
            _skinnedMeshBaker.enabled = false;
            _humanBodyTracker.OnTrackStart.AddListener(StartEffect);
        }

        private void StartEffect()
        {
            StartCoroutine(Schedule());
        }
        
        private IEnumerator Schedule()
        {
            while (gameObject.activeSelf)
            {
                _audioSource.Play();
                _volume.profile = _volumeProfile2;
                _skinnedMeshBaker.enabled = false;
                yield return new WaitForSeconds(4.7f);
                _particle.SetActive(false);
                _line.SetActive(false);
                _lightening.SetActive(true);
                yield return new WaitForSeconds(1.6f);
                _skinnedMeshBaker.enabled = true;
                yield return new WaitForSeconds(0.5f);
                _volume.profile = _volumeProfile1;
                _particle.SetActive(false);
                _line.SetActive(true);
                _lightening.SetActive(false);
                yield return new WaitForSeconds(2.6f);
                _particle.SetActive(true);
                _line.SetActive(false);
                _lightening.SetActive(false);
                yield return new WaitForSeconds(6f);
                _particle.SetActive(false);
                _line.SetActive(false);
                _lightening.SetActive(false);
            }
        }
    }
}
