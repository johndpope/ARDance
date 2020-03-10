using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicShowSimpleUpDown : DynamicShow
{
    protected override float _minThreshold { get; } = -0.1f;
}
