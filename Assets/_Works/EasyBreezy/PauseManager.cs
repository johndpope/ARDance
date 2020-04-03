using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace EazyBreezy
{
    public class PauseManager : MonoBehaviour
    {
        [SerializeField] private AROcclusionManager _arOcclusionManager;
        [SerializeField] private HumanDummy _humanDummy;
        [SerializeField] private Shader _humanStencilMask;
        [SerializeField] private RawImage[] _humanDummyImages;
        [SerializeField] private UVAdjuster _uvAdjuster;
        [SerializeField] private CountDown _countDown;

        private const float DISTANCE_FROM_CAMERA = 1f;
        private Camera _camera;
        private Vector3 _initialPos;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _initialPos = _humanDummyImages[0].transform.position;
        }

        private void Start()
        {
        }

        public void Take()
        {
            _countDown.StartCountDown(() =>
            {
                _humanDummyImages[0].gameObject.SetActive(true);
                _humanDummyImages[0].texture = _humanDummy.GetHumanDummy();
                var mat = new Material(_humanStencilMask);
                var origin = _arOcclusionManager.humanStencilTexture;
                var humanStencil = new RenderTexture(origin.width, origin.height, 0);
                Graphics.Blit(origin, humanStencil);
                _uvAdjuster.SetMaterialProperty(mat, humanStencil, UVAdjuster.TextureType.Stencil);
                _humanDummyImages[0].material = mat;
            });
        }

        public void ScaleSlider(Slider slider)
        {
            var scale = 1f - slider.value;
            _humanDummyImages[0].transform.localScale = Vector3.one * scale;
            DebugTransform();
        }
        
        public void XPlusPosSlider(Slider slider)
        {
            var x = slider.value * 1000f;
            _humanDummyImages[0].transform.position = 
                new Vector3(_initialPos.x + x, _humanDummyImages[0].transform.position.y, _humanDummyImages[0].transform.position.z);
            DebugTransform();
        }
        
        public void XMinusPosSlider(Slider slider)
        {
            var x = slider.value * 1000f;
            _humanDummyImages[0].transform.position = 
                new Vector3(_initialPos.x - x, _humanDummyImages[0].transform.position.y, _humanDummyImages[0].transform.position.z);
            DebugTransform();
        }
        
        public void YPlusPosSlider(Slider slider)
        {
            var y = slider.value * 1000f;
            _humanDummyImages[0].transform.position = 
                new Vector3(_humanDummyImages[0].transform.position.x, _initialPos.y + y, _humanDummyImages[0].transform.position.z);
            DebugTransform();
        }
        
        public void YMinusPosSlider(Slider slider)
        {
            var y = slider.value * 1000f;
            _humanDummyImages[0].transform.position = 
                new Vector3(_humanDummyImages[0].transform.position.x, _initialPos.y - y, _humanDummyImages[0].transform.position.z);
            DebugTransform();
        }

        private Vector3 GetCanvasScale()
        {
            var pointTop = _camera.ScreenToWorldPoint(new Vector3(0, 0, DISTANCE_FROM_CAMERA));
            var pointBottom = _camera.ScreenToWorldPoint(new Vector3(0, _camera.pixelHeight, DISTANCE_FROM_CAMERA));
            var frustumHeight = Vector3.Distance(pointTop, pointBottom);
            return new Vector3(frustumHeight * _camera.aspect, frustumHeight, 1);
        }

        private void DebugTransform()
        {
            var tran = _humanDummyImages[0].transform;
            debug.text =
                "position: (" + tran.position.x + ", " + tran.position.y + ", " + tran.position.z + ")\n" +
                "scale: (" + tran.localScale.x + ", " + tran.localScale.y + ", " + tran.localScale.z + ")";
        }
        
        #region Debug

        [SerializeField] private Text debug;

        public void DebugLog(string log)
        {
            debug.text += log;
        }

        private IEnumerator ClearText()
        {
            yield return new WaitForSeconds(5);
            debug.text = "";
        }

        #endregion
    }
}
