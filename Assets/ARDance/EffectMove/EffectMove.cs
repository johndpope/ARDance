using System.Collections;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class EffectMove : MonoBehaviour
{
    [SerializeField] private GameObject[] _humans;
    
    [SerializeField] private GameObject[] _humans2;
    [SerializeField] private Material[] _materials2;

    private readonly int PropertyID_DistortionPower = Shader.PropertyToID("_DistortionPower");
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
//            Init();
            Init2();
        }
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
//            StartCoroutine(Move());
            Move2();
        }
    }

    private void Init()
    {
        foreach (var h in _humans)
        {
            h.SetActive(false);
        }
        _humans[0].SetActive(true);
    }

    private IEnumerator Move()
    {
        foreach (var h in _humans)
        {
            h.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            h.SetActive(false);
        }
        _humans.Last().SetActive(true);
    }

    private void Init2()
    {
        _materials2[0].SetFloat(PropertyID_DistortionPower, 0);
        _materials2[1].SetFloat(PropertyID_DistortionPower, 0.45f);
        //_materials2[2].SetFloat(PropertyID_DistortionPower, 0.45f);
        _humans2[0].SetActive(true);
        _humans2[1].SetActive(false);
        //_humans2[2].SetActive(false);
    }
    
    private void Move2()
    {
        Move2Single(0, 0, -0.4f, 0.1f, Ease.InSine).OnComplete(() => 
        { 
            _humans2[0].SetActive(false); 
            _humans2[1].SetActive(true);
            Move2Single(1, 0.45f, 0f, 0.1f, Ease.OutSine).OnComplete(() => 
            {
//                Move2Single(1, 0f, 0.45f,0.1f, Ease.OutSine).OnComplete(() => 
//                {
//                    _humans2[0].SetActive(true); 
//                    _humans2[1].SetActive(false);
//                    Move2Single(0, -0.4f, 0f, 0.1f, Ease.InSine).OnComplete(() =>
//                    {
//                        Move2Single(0, 0, -0.4f, 0.1f, Ease.InSine).OnComplete(() =>
//                        {
//                            _humans2[0].SetActive(false);
//                            _humans2[1].SetActive(true);
//                            Move2Single(1, 0.45f, 0f, 0.1f, Ease.OutSine).OnComplete(() => { });
//                        });
//                    });
//                });
            });
        });
    }

    private Tween Move2Single(int index, float start, float end, float duration, Ease ease)
    {
        var value = start;
        return DOTween.To(() => value, num => value = num, end, duration)
            .SetEase(ease)
            .OnUpdate(() => _materials2[index].SetFloat(PropertyID_DistortionPower, value));
    }
}
