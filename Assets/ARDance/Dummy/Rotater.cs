using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Rotater : MonoBehaviour
{
    [SerializeField] private bool _inverse;
    [SerializeField] private Transform _object;
    
    private Vector3 _center = new Vector3(0, 0.3f, 0.5f);
    private float _radius = 0.3f;
    private const float _minSpeed = 5f; // seconds per a round
    private const float _maxSpeed = 10f; // seconds per a round
    private float _speedTheta;
    [SerializeField] [Range(0, 360)] private float _angleTheta;
    [SerializeField] [Range(0, 360)] private float _anglePhi;
    private bool _isRotating;
    private Coroutine _coroutine;
    private bool _isChagedRoot;
    private float _xAdjust = 1f;
    private bool _isMoving;
    
    private void Start()
    {
        _speedTheta = (_inverse ? -1 : 1) * Random.Range(_minSpeed, _maxSpeed);
        _object.DOLocalRotate(new Vector3(0, 360, 0), 3f)
            .SetRelative()
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }
    
    public void StartRotate()
    {
        _isRotating = true;
        SetParams();
        _coroutine = StartCoroutine(AutoRandomSpeed());
        _angleTheta = Random.Range(0f, 360f);
        _anglePhi = Random.Range(0f, 360f) * Mathf.Deg2Rad;
    }

    public void StopRotate()
    {
        _isRotating = false;
        SetParams();
        if (_coroutine != null) StopCoroutine(_coroutine);
    }

    public void RandomSetPosition(int index)
    {
        if (_isMoving) return;
        _isMoving = true;
        StopRotate();
        _angleTheta *= Mathf.Deg2Rad;
        var rand = GetUniformRandom(index);
        var targetTheta = rand.Item1 * Mathf.Deg2Rad;
        var targetPhi = rand.Item2 * Mathf.Deg2Rad;
        var seq = DOTween.Sequence();
        seq
            .Join(DOTween.To(() => _angleTheta, num => _angleTheta = num, targetTheta, 0.2f)
                    .OnUpdate(() => { transform.position = GetPositionOnSphere(_angleTheta, _anglePhi, _radius, _center); }))
            .Join(DOTween.To(() => _anglePhi, num => _anglePhi = num, targetPhi, 0.2f)
                .OnUpdate(() => { transform.position = GetPositionOnSphere(_angleTheta, _anglePhi, _radius, _center); }))
            .OnComplete(() => { _isMoving = false; });
    }

    private void SetParams()
    {
        if (_isRotating)
        {
            _center = new Vector3(0, 0.3f, 0.5f);
            _radius = 0.3f;
            _xAdjust = 1f;
        }
        else
        {
            _center = new Vector3(0, 0.2f, 0.7f);
            _radius = 0.3f;
            _xAdjust = 0.6f;
        }
    }

    private void Update()
    {
        if (_isRotating)
        {
            _angleTheta += 360f / _speedTheta * Time.deltaTime;
            if (Mathf.Abs(_angleTheta) % 360f < 10)
            {
                if (!_isChagedRoot)
                {
                    _isChagedRoot = true;
                    _anglePhi = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                }
            }
            else
            {
                _isChagedRoot = false;
            }
            transform.position = GetPositionOnSphere(_angleTheta * Mathf.Deg2Rad, _anglePhi, _radius, _center);
        }
        
        //transform.position = GetPositionOnSphere(_angleTheta * Mathf.Deg2Rad, _anglePhi * Mathf.Deg2Rad, _radius, _center);
    }
    
    private Vector3 GetPositionOnSphere(float theta, float phi, float r, Vector3 center)
    {
        float x = r * 0.5f * Mathf.Sin(theta) * Mathf.Cos(phi);
        float y = r * Mathf.Sin(theta) * Mathf.Sin(phi);
        float z = r * Mathf.Cos(theta);
        return center + new Vector3(x, y, z);
    }

    private IEnumerator AutoRandomSpeed()
    {
        while (_isRotating)
        {
            yield return new WaitForSeconds(Random.Range(3, 6));
            _speedTheta = (_inverse ? -1 : 1) * Random.Range(_minSpeed, _maxSpeed);
        }
    }

    public (float, float) GetUniformRandom(int index)
    {
        switch (index)
        {
            case 0:
                return (Random.Range(220f, 340f), Random.Range(280f, 350f));
            case 1:
                return (Random.Range(210f, 240f), Random.Range(200f, 340f));
            case 2:
                return (Random.Range(20f, 100f), Random.Range(10f, 80f));
            case 3:
                return (Random.Range(220f, 340f), Random.Range(10f, 80f));
            case 4:
                return (Random.Range(210f, 240f), Random.Range(20f, 160f));
            default:
                return (Random.Range(20f, 100f), Random.Range(280f, 350f));
        }
    }
}
