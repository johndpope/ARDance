using UnityEngine;

public interface IAddtionalPostProcess
{
    Material MaterialForPostProcess { get; }
    bool IsEnable { get; }
}
