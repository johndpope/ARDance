using System.Collections.Generic;
using UnityEngine;

public class TestTest : HumanSegmentationEffectBase
{
    [SerializeField] private Material _humanStencilMask;

    void Start()
    {
        _humanSegmentMats = new List<Material> { _humanStencilMask };
    }
}
