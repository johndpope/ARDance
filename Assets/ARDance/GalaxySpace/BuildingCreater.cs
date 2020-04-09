using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BuildingCreater : MonoBehaviour
{
    [SerializeField] private float _startPosZ;
    [SerializeField] private float _width;
    [SerializeField] private float _gap;
    [SerializeField] private int _num;
    [SerializeField] private int _gridNum;
    [SerializeField] private bool _isLeft;
    [SerializeField] private Material _material;

    private List<Transform> _cubes = new List<Transform>();
    private readonly int PropertyID_TilingFactor = Shader.PropertyToID("_TilingFactor");
    private readonly int PropertyID_GridNum = Shader.PropertyToID("_GridNum");
    private readonly int PropertyID_MoveDirection = Shader.PropertyToID("_MoveDirection");

    private void Start()
    {
        for (int i = 0; i < _num; i++)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            cube.SetParent(transform);
            SetSize(cube);
            if (i == 0)
            {
                SetPos(null, cube);
            }
            else
            {
                SetPos(_cubes[i - 1], cube);
            }
            SetMaterial(cube);
            _cubes.Add(cube);
        }
    }


    private void SetPos(Transform before, Transform self)
    {
        self.localPosition = new Vector3
        (
            0f,
            self.localScale.y / 2f,
            before == null ? _startPosZ : before.position.z + (before.localScale.z + self.localScale.z) / 2 + _gap
        );
    }

    private void SetSize(Transform cube)
    {
        var w = Random.Range(0.3f, 0.7f);
        cube.localScale = new Vector3
        (
            _width,
            Random.Range(0.4f, 1.4f),
            w
            
        );
    }

    private void SetMaterial(Transform cube)
    {
        var ren = cube.GetComponent<MeshRenderer>();
        ren.material = _material;
        var block = new MaterialPropertyBlock();
        ren.GetPropertyBlock(block);
        block.SetVector(PropertyID_TilingFactor, new Vector4(cube.localScale.z, cube.localScale.y, 0, 0));
        block.SetFloat(PropertyID_GridNum, _gridNum);
        var dir = _isLeft ? new Vector2(1, 0) : new Vector2(-1, 0); 
        block.SetVector(PropertyID_MoveDirection, dir);
        ren.SetPropertyBlock(block);
    }
}