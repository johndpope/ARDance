using UnityEngine;

public class BigTransition : MonoBehaviour
{
    [SerializeField] private Material _humanMat;
    [SerializeField] private Material _floorMat;
    
    private Renderer _floor;
    private Renderer _humanRen;
    
    public void Init(Renderer floor, Renderer humanRen)
    {
        _floor = floor;
        _humanRen = humanRen;
    }

    public void Transit()
    {
        _floor.material = _floorMat;
        _humanRen.material = _humanMat;
    }
}
