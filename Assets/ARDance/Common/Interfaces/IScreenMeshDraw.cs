using UnityEngine;

public interface IScreenMeshDraw
{
    Material MeshMaterial { get; }
    bool ShouldDraw { get; }
}
