using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class VfxTestIntegrater : MonoBehaviour
{
    public bool IsEnable
    {
        set
        {
            _isEnable = value;
        }
    }
    private bool _isEnable;
    
    [SerializeField] private ComputeShader _computeShader;
    [SerializeField] private RenderTexture _positionMapPortrait;
    [SerializeField] private RenderTexture _positionMapLandscape;
    [SerializeField] private Texture _depthSample;

    private HumanDepthConverter _humanDepthConverter;
    private DeviceOrientation _lastDeviceOrientation;

    private void Awake()
    {
        _humanDepthConverter = new HumanDepthConverter(GetComponent<Camera>(), _computeShader);
    }

    private void Start()
    {
        _lastDeviceOrientation = Input.deviceOrientation;
        _humanDepthConverter.InitSetup(_lastDeviceOrientation == DeviceOrientation.Portrait ? _positionMapPortrait : _positionMapLandscape);
    }

    private void Update()
    {
        if (!_isEnable) return;
        if (_depthSample)
        {
            if (_lastDeviceOrientation != Input.deviceOrientation)
            {
                _lastDeviceOrientation = Input.deviceOrientation;
                if (_lastDeviceOrientation == DeviceOrientation.Portrait)
                {
                    _humanDepthConverter.InitSetup(_positionMapPortrait);
                }
                else
                {
                    _humanDepthConverter.InitSetup(_positionMapLandscape);
                }
            }

            if (_lastDeviceOrientation == DeviceOrientation.Portrait)
            {
                _humanDepthConverter.ConvertDepth(_depthSample, _positionMapPortrait, true);
            }
            else
            {
                _humanDepthConverter.ConvertDepth(_depthSample, _positionMapLandscape, false);
            }
        }
    }
}
