using System.Collections.Generic;
using UnityEngine;

public class SampleShot : HumanSegmentationEffectBase
{
    [SerializeField] private Material _material;
    [SerializeField] private ScreenShot _screenShot;
    [SerializeField] private CountDown _countDown;

    private void Start()
    {
        _humanSegmentMats = new List<Material> { _material };
    }

    public void Take()
    {
        _countDown.StartCountDown(() =>
        {
            _countDown.transform.parent.gameObject.SetActive(false);
            _screenShot.TakeShot();
        }, 7);
    }
}
