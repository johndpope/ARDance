using UnityEngine;

public interface IBlurPostProcess
{
    Material BlurMaterial { get; }
    float Offset { get; }
}
