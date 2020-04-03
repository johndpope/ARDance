using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BackgroundPinner))]
public class ShapeDisrupt : VfxIntegrater
{
    [SerializeField] private Text _pinButtonText;
    
    private BackgroundPinner _backgroundPinner;
    private bool _isShowing;
    
    void Awake()
    {
        _backgroundPinner = GetComponent<BackgroundPinner>();
        _visualEffect.enabled = _isShowing;
    }

    public void TakeBackground()
    {
        _backgroundPinner.TakeBackground();
    }

    public void HandlePin()
    {
        if (_isShowing)
        {
            _isShowing = false;
            _backgroundPinner.Unpin();
        }
        else
        {
            _isShowing = true;
            _backgroundPinner.Pin();
        }
        _visualEffect.enabled = _isShowing;
        _pinButtonText.text = _isShowing ? "UnPin" : "Pin";
    }
}
