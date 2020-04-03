using UnityEngine;

public class CRTEffect : MonoBehaviour, IAddtionalPostProcess
{
    public Material MaterialForPostProcess => _material;
    public bool IsEnable => true;
    
    [SerializeField] private Shader _shader;
    [SerializeField] private Material _material;

    private void Awake()
    {
        if (_shader)
        {
            _material = new Material(_shader);
        }
    }
}
