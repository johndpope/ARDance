using UnityEngine;

public interface IScreenMeshDrawMulti
{
    Material[] MeshMaterials { get; }
    bool ShouldDraw { get; }
}
