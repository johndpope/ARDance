using DG.Tweening;
using UnityEngine;

public class AVLight : MonoBehaviour
{
    [SerializeField] private MeshRenderer _back;
    [SerializeField] private MeshRenderer _front;
    
    private readonly int PropertyID_Length = Shader.PropertyToID("_Length");
    private readonly int PropertyID_Anchor = Shader.PropertyToID("_Anchor");
    private readonly int PropertyID_Attenuation = Shader.PropertyToID("_Attenuation");
    private readonly int PropertyID_RimPower = Shader.PropertyToID("_RimPower");
    private readonly int PropertyID_Color = Shader.PropertyToID("_Color");
    
    private const float INITIAL_FRONT_LENGTH = -1.0f;
    private float INITIAL_BACK_LENGTH = 7.0f;
    private float INITIAL_ANCHOR_LENGTH = 0.235f;
    private const float LENGTH_RATE = 0.4f;
    private const float ANCHOR_RATE = 0.01f;

    private MaterialPropertyBlock _backBlock;
    private MaterialPropertyBlock _frontBlock;
    
    private void Awake()
    {
        INITIAL_BACK_LENGTH = _back.material.GetFloat(PropertyID_Length);
        INITIAL_ANCHOR_LENGTH = _back.material.GetFloat(PropertyID_Anchor);
        _backBlock = new MaterialPropertyBlock();
        _frontBlock = new MaterialPropertyBlock();
    }
    
    public void ChangeLightLength(float length)
    {
        _back.GetPropertyBlock(_backBlock);
        _backBlock.SetFloat(PropertyID_Length, INITIAL_BACK_LENGTH + length * LENGTH_RATE);
        _backBlock.SetFloat(PropertyID_Anchor, INITIAL_ANCHOR_LENGTH + length * ANCHOR_RATE);
        _back.SetPropertyBlock(_backBlock);
        
        _front.GetPropertyBlock(_frontBlock);
        _frontBlock.SetFloat(PropertyID_Length, INITIAL_FRONT_LENGTH + length);
        _front.SetPropertyBlock(_frontBlock);
        
    }

    public void SetColor(Color col)
    {
        _back.GetPropertyBlock(_backBlock);
        _backBlock.SetColor(PropertyID_Color, col);
        _back.SetPropertyBlock(_backBlock);
        
        _front.GetPropertyBlock(_frontBlock);
        _frontBlock.SetColor(PropertyID_Color, col);
        _front.SetPropertyBlock(_frontBlock);
    }
    
    public void ChangeLengthAuto()
    {
        var value = 0f;
        DOTween.To(() => value, num => value = num, 10, 5).OnUpdate(() =>
        {
            ChangeLightLength(value);
        }).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
}
