using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCS : MonoBehaviour
{
    struct ThreadSize
    {
        public int x;
        public int y;
        public int z;

        public ThreadSize(uint x, uint y, uint z)
        {
            this.x = (int) x;
            this.y = (int) y;
            this.z = (int) z;
        }
    }

    [SerializeField] private ComputeShader _computeShader;
    [SerializeField] private RawImage _rawImage;
    
    RenderTexture _renderTexture;
    ThreadSize _kernelThreadSize;
    int _shaderKernel;
    int _positionMapID;
    
    void Start()
    {
        if (null == _computeShader) return;
        _shaderKernel = _computeShader.FindKernel("CSMain");
        _positionMapID = Shader.PropertyToID("textureBuffer");

        _renderTexture = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32)
        {
            enableRandomWrite = true
        };
        _renderTexture.Create();

        uint threadSizeX, threadSizeY, threadSizeZ;
        _computeShader.GetKernelThreadGroupSizes(_shaderKernel, out threadSizeX, out threadSizeY, out threadSizeZ);
        _kernelThreadSize = new ThreadSize(threadSizeX, threadSizeY, threadSizeZ);


        _computeShader.SetTexture(0, _positionMapID, _renderTexture);
        _computeShader.Dispatch(_shaderKernel, _renderTexture.width / _kernelThreadSize.x,
            _renderTexture.height / _kernelThreadSize.y, _kernelThreadSize.z);

        _rawImage.texture = _renderTexture;
    }

    // Update is called once per frame
    void Update()
    {
    }
}