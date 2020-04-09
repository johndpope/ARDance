﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class AVController : MonoBehaviour
{
    [SerializeField] private GameObject _avlPrefab;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _radius;
    [SerializeField] private int _num;
    [SerializeField] private float _amp;
    [SerializeField] private float _maxValue;
    [SerializeField] [ColorUsage(false, true)] private Color _baseColor;
    [SerializeField] private float _hueShift;
    [SerializeField] private Transform _centerObj;
    

    private AudioSpectrum _audioSpectrum;
    private List<AVLight> _avLights = new List<AVLight>();

    private void Awake()
    {
        _audioSpectrum = new AudioSpectrum(_audioSource, _num);
        Setup();
    }

    private void Start()
    {
        StartCoroutine(RotateRandom());
        _centerObj.DOLocalRotate(new Vector3(0, 0, 360), 5)
            .SetRelative()
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    private void Update()
    { 
        _audioSpectrum.GetSpectrum(); 
        for (int i = 0; i < _audioSpectrum.MaxInEachUnit.Length; i++)
        {
            _avLights[i].ChangeLightLength(Mathf.Min(_maxValue, _audioSpectrum.MaxInEachUnit[i] * _amp));
        }

        var value = _audioSpectrum.Max * 5f;
        if (value > 1f)
        {
            transform.localScale = Vector3.one * value;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    private void Setup()
    {
        for (int i = 0; i < _num; i++)
        {
            CreateAVL(i);
        }
    }

    private void CreateAVL(int index)
    {
        var avl = Instantiate(_avlPrefab, transform).transform;
        var angle = 2 * Mathf.PI * index / _num;
        var x = _radius * Mathf.Cos(angle);
        var y = _radius * Mathf.Sin(angle);
        avl.localPosition = new Vector3(x, y, 0f);
        avl.localEulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg - 90f);
        var avLight = avl.GetComponent<AVLight>();
        avLight.SetColor(GetColor(index));
        _avLights.Add(avLight);
    }

    private Color GetColor(int index)
    {
        float h; 
        float s; 
        float v;
        Color.RGBToHSV(_baseColor, out h, out s, out v);
        if (index < _num / 2)
        {
            h += _hueShift * index;
        }
        else
        {
            h += _hueShift * (_num - index);
        }
        return Color.HSVToRGB(h, s, v);
    }

    private IEnumerator RotateRandom()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(Random.Range(3, 6));
            transform.DORotate(new Vector3(0, 0, Random.Range(0, 360)), 0.2f);
        }
    }
    
}