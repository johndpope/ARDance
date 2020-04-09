using UnityEngine;

public class HumanTexture : HumanSegmentationEffectBase, IAddtionalPostProcess
{
    public Material MaterialForPostProcess => _material;
    public bool IsEnable => true;
    
    [SerializeField] private Material _material;

    private void Start()
    {
        _humanSegmentMats.Add(_material);
    }
}
