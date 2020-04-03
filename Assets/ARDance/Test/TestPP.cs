using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPP : MonoBehaviour, IAddtionalPostProcess
{
    public Material MaterialForPostProcess => _material;
    public bool IsEnable => _isActive;
    
    [SerializeField] private Material _material;
    private bool _isActive;
    
    void Start()
    {
        _isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
