using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BackgroundPinner))]
public class SimpleShowOff : VfxIntegrater
{
    [SerializeField] private Text _pinButtonText;
    [SerializeField] private CountDown _countDown;
    
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
            _visualEffect.enabled = false;
        }
        else
        {
            _isShowing = true;
            _countDown.StartCountDown(() =>
            {
                _backgroundPinner.Pin();
                _visualEffect.enabled = true;
            });
        }
        _pinButtonText.text = _isShowing ? "UnPin" : "Pin";
    }
}
