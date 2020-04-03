using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class PeopleOcclusion : MonoBehaviour, IScreenRTBlit, IScreenMeshDrawInWorld, IAddtionalPostProcess
{
    public RenderTexture LatestCameraFeedBuffer => _cameraFeed;
    
    public Mesh QuadMesh => _mesh;
    public Material MeshMaterial => _backgroundMaterial;
    public bool ShouldDraw => true;
    public Matrix4x4 Matrix => GetMatrix();
    
    public Material MaterialForPostProcess => _occlusionMaterial;
    public bool IsEnable => _isActive;

    private const float DISTANCE_FROM_CAMERA = 3f;
    
    [SerializeField] private AROcclusionManager _arOcclusionManager;
    [SerializeField] private ARCameraManager _arCameraManager;
    [SerializeField] private Material _occlusionMaterial;
    [SerializeField] private Material _dummyMaterial;
    [SerializeField] private UVAdjuster _uvAdjuster;
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Shader _backgroundShader;
    [SerializeField] private RenderTexture _cameraFeed;
    
    private Material _backgroundMaterial;
    private bool _isActive;
    private Camera _camera;
    private Transform _anchor;
    private Vector3 _meshScale = new Vector3(-1f, 0f, 0f);
    
    private readonly int k_DisplayTransformId = Shader.PropertyToID("_UnityDisplayTransform");

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _isActive = true;
        _backgroundMaterial = new Material(_backgroundShader);
    }

    private void OnEnable()
    {
        _arCameraManager.frameReceived += OnCameraFrameReceived;
    }

    private void OnDisable()
    {
        _arCameraManager.frameReceived -= OnCameraFrameReceived;
    }
    private void Start()
    {
        _anchor = new GameObject("Draw Mesh Anchor").transform;
        _anchor.SetParent(_camera.transform);
        _anchor.localPosition = new Vector3(0, 0, DISTANCE_FROM_CAMERA);
        
        _uvAdjuster.SetTexture(_occlusionMaterial, _cameraFeed, UVAdjuster.TextureType.CameraFeed);
        _uvAdjuster.SetTexture(_dummyMaterial, _cameraFeed, UVAdjuster.TextureType.Main);
    }

    private void Update()
    {
        var humanDepth = _arOcclusionManager.humanDepthTexture;
        if (humanDepth)
        {
            _uvAdjuster.SetMaterialProperty(_occlusionMaterial, humanDepth, UVAdjuster.TextureType.Depth);
            _uvAdjuster.SetMaterialProperty(_dummyMaterial, humanDepth, UVAdjuster.TextureType.Depth);
        }
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                
            }
        }
    }
    
    private Matrix4x4 GetMatrix()
    {
        if (_anchor == null)
        {
            return Matrix4x4.identity;
        }
        if (_meshScale.x < 0f)
        {
            _meshScale = GetMeshScale();
        }
        return Matrix4x4.TRS(_anchor.position, _anchor.rotation, _meshScale);
    }
    
    private Vector3 GetMeshScale()
    {
        var pointTop = _camera.ScreenToWorldPoint(new Vector3(0, 0, DISTANCE_FROM_CAMERA));
        var pointBottom = _camera.ScreenToWorldPoint(new Vector3(0, _camera.pixelHeight, DISTANCE_FROM_CAMERA));
        var frustumHeight = Vector3.Distance(pointTop, pointBottom);
        return new Vector3(frustumHeight * _camera.aspect, frustumHeight, 1);
    }

    private void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        if (_backgroundMaterial != null)
        {
            var count = eventArgs.textures.Count;
            for (int i = 0; i < count; ++i)
            {
                _backgroundMaterial.SetTexture(eventArgs.propertyNameIds[i], eventArgs.textures[i]);
            }

            if (eventArgs.displayMatrix.HasValue)
            {
                _backgroundMaterial.SetMatrix(k_DisplayTransformId, eventArgs.displayMatrix.Value);
            }
        }
    }

    #region Debug

    [SerializeField] private Text debug;

    public void DebugLog(string log)
    {
        debug.text += log;
    }

    private IEnumerator ClearText()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(5);
            debug.text = "";
        }
    }
    #endregion
}
