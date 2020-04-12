using UnityEngine;

public interface IScreenMeshDrawInWorld
{
    Mesh QuadMesh { get; }
    Material MeshMaterial { get; }
    bool ShouldDraw { get; }
    Matrix4x4 Matrix { get; }
}
