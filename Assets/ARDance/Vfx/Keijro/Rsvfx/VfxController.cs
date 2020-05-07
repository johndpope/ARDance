using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class VfxController : MonoBehaviour
{
    public RenderTexture CurrentPositionMap;
    public RenderTexture CurrentColorMap;
    
    [SerializeField] private VisualEffect[] _visualEffects;
    private int _total;
    private int _index;

    private readonly int PropertyID_PositionMap = Shader.PropertyToID("PositionMap");
    private readonly int PropertyID_ColorMap = Shader.PropertyToID("ColorMap");
    
    private void Awake()
    {
        _total = _visualEffects.Length;
        _index = 0;
    }

    private void Start()
    {
        StartCoroutine(ChangeVfx());
    }

    private IEnumerator ChangeVfx()
    {
        while (true)
        {
            var current = _index % _total;
            var old = current - 1;
            _visualEffects[old < 0 ? _total - 1 : old].gameObject.SetActive(false);
            _visualEffects[current].gameObject.SetActive(true);
            _visualEffects[current].SetTexture(PropertyID_PositionMap, CurrentPositionMap);
            _visualEffects[current].SetTexture(PropertyID_ColorMap, CurrentColorMap);
            yield return new WaitForSeconds(6f);
            _index++;
        }
    }
}
