using System;
using UnityEngine;
using GCSeries.Core;
using F3Device.Device;

namespace GCSeries.zView
{
    public class GVirtualCameraThreeD : GVirtualCamera
    {
        /// <summary>
        /// 默认瞳距
        /// </summary>
        private float _ipd = 0.03f;

        /// <summary>
        /// 设置瞳距
        /// </summary>
        /// <value></value>
        public float ipd
        {
            get
            {
                return _ipd;
            }
            set
            {
                _ipd = Mathf.Clamp(value, 0f, 0.1f);
            }
        }

        void Awake()
        {
            // Dynamically create a new Unity camera and disable it to allow for manual 
            // rendering via Camera.Render().
            GameObject leftCam = new GameObject("leftCam");
            leftCam.transform.SetParent(this.transform);

            GameObject rightCam = new GameObject("rightCam");
            rightCam.transform.SetParent(this.transform);

            _cameras[0] = leftCam.AddComponent<Camera>();
            _cameras[1] = rightCam.AddComponent<Camera>();

            foreach (var cam in _cameras)
            {
                cam.enabled = false;
                cam.targetDisplay = 1;
                cam.stereoTargetEye = StereoTargetEyeMask.None;
            }
        }

        //////////////////////////////////////////////////////////////////
        // Virtual Camera Overrides
        //////////////////////////////////////////////////////////////////
        public override void SetUp(GView zView, IntPtr connection, GView.ModeSetupPhase phase)
        {
            _zCameraRig = FindObjectOfType<ZCameraRig>();

            foreach (var cam in _cameras)
            {
                cam.enabled = true;
                cam.cullingMask = cam.cullingMask & ~(zView.StandardModeIgnoreLayers);
            }

            switch (phase)
            {
                case GView.ModeSetupPhase.Initialization:
                    //this.UpdateImageResolution(zView, connection);

                    _imageWidth = (ushort)(zView.imageWidth / 1.8);
                    _imageHeight = zView.imageHeight;
                    RenderTextureDescriptor descriptor = new RenderTextureDescriptor(_imageWidth, _imageHeight, RenderTextureFormat.ARGB32);
                    descriptor.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
                    descriptor.depthBufferBits = 0;

                    if (_renderTextures == null)
                    {
                        _renderTextures = new RenderTexture[2];
                        _renderTextures[0] = Resources.Load<RenderTexture>("gViewRT");
                        _renderTextures[1] = Resources.Load<RenderTexture>("gViewRT_2");
                    }

                    // Cache the render texture's native texture pointer. Per Unity documentation,
                    // calling GetNativeTexturePtr() when using multi-threaded rendering will
                    // synchronize with the rendering thread (which is a slow operation). So, only
                    // call and cache once upon initialization.
                    _nativeTexturePts[0] = _renderTextures[0].GetNativeTexturePtr();
                    _nativeTexturePts[1] = _renderTextures[1].GetNativeTexturePtr();

                    // Cache the camera's culling mask to be restored after it renders the frame.
                    int cullingMask = _cameras[0].cullingMask;

                    if (zView.ActiveZCamera == null)
                    {
                        Debug.LogWarning("ZView's public property ActiveZCamera" +
                            "is null, but must be assigned.");
                        return;
                    }

                    if (zView.ActiveZCamera.transform != this._zCameraTransform)
                    {
                        this._zCameraTransform = zView.ActiveZCamera.transform;
                        this._zCameraCamera = this._zCameraTransform.GetComponent<Camera>();
                    }

                    // Copy the center eye camera's attributes to the standard mode primary camera.

                    _cameras[0].CopyFrom(this._zCameraCamera);
                    _cameras[0].enabled = false;
                    _cameras[0].stereoTargetEye = StereoTargetEyeMask.None;
                    _cameras[0].cullingMask = cullingMask & ~(zView.StandardModeIgnoreLayers);
                    _cameras[0].targetTexture = _renderTextures[0];

                    _cameras[1].CopyFrom(this._zCameraCamera);
                    _cameras[1].enabled = false;
                    _cameras[1].stereoTargetEye = StereoTargetEyeMask.None;
                    _cameras[1].cullingMask = cullingMask & ~(zView.StandardModeIgnoreLayers);
                    _cameras[1].targetTexture = _renderTextures[1];

                    F3DService.Instance.SwitchScreenState(F3DService.Instance.projectionDevice, true);
                    break;
                case GView.ModeSetupPhase.Completion:
                    break;
                default:
                    break;
            }
        }

        public override void TearDown()
        {
            // Reset the camera's target texture.
            _cameras[0].targetTexture = null;
            _cameras[0].enabled = false;

            _cameras[1].targetTexture = null;
            _cameras[1].enabled = false;

            // Reset the render texture's native texture pointer.
            _nativeTexturePts[0] = IntPtr.Zero;
            _nativeTexturePts[1] = IntPtr.Zero;

            // Clean up the existing render texture.
            // if (_renderTexture != null)
            // {
            //     UnityEngine.Object.Destroy(_renderTexture);
            //     _renderTexture = null;
            // }

            _imageWidth = 0;
            _imageHeight = 0;

            F3DService.Instance.SwitchScreenState(F3DService.Instance.projectionDevice, false);
        }

        public override void Render(GView zView, IntPtr connection, IntPtr receivedFrame)
        {
            _calculator.SetVector(_zCameraRig.Frame.transform.localToWorldMatrix.MultiplyPoint(new Vector3(-ZProvider.DisplaySize.x / 2f, ZProvider.DisplaySize.y / 2f, 0f)),
                                _zCameraRig.Frame.transform.localToWorldMatrix.MultiplyPoint(new Vector3(-ZProvider.DisplaySize.x / 2f, -ZProvider.DisplaySize.y / 2f, 0f)),
                                _zCameraRig.Frame.transform.localToWorldMatrix.MultiplyPoint(new Vector3(ZProvider.DisplaySize.x / 2f, -ZProvider.DisplaySize.y / 2f, 0f)));


            _cameras[0].transform.position = _zCameraRig.transform.localToWorldMatrix.MultiplyPoint(Vector3.back * ZProvider.WindowSize.magnitude + new Vector3(ipd / -2f, 0f, 0f));
            _cameras[0].transform.rotation = _zCameraRig.transform.rotation;
            _cameras[0].projectionMatrix = _calculator.GeneralizedPerspectiveProjection(_cameras[0].transform.position, _cameras[0].nearClipPlane, _cameras[0].farClipPlane);

            _cameras[1].transform.position = _zCameraRig.transform.localToWorldMatrix.MultiplyPoint(Vector3.back * ZProvider.WindowSize.magnitude + new Vector3(ipd / 2f, 0f, 0f));
            _cameras[1].transform.rotation = _zCameraRig.transform.rotation;
            _cameras[1].projectionMatrix = _calculator.GeneralizedPerspectiveProjection(_cameras[1].transform.position, _cameras[1].nearClipPlane, _cameras[1].farClipPlane);

            foreach (var cam in _cameras)
            {
                cam.Render();
            }
        }

        public override IntPtr[] GetNativeTexturePtr(out int count)
        {
            count = 2;
            return _nativeTexturePts;
        }

        public override RenderTexture[] GetRenderTexture(out int count)
        {
            count = 2;
            return _renderTextures;
        }

        //////////////////////////////////////////////////////////////////
        // Private Methods
        //////////////////////////////////////////////////////////////////

        private void UpdateImageResolution(GView zView, IntPtr connection)
        {
            Debug.LogError("GVirtualCameraStandard.UpdateImageResolution():设置图像分辨率没实现");
        }

        private Matrix4x4 FlipHandedness(Matrix4x4 matrix)
        {
            return s_flipHandednessMap * matrix * s_flipHandednessMap;
        }


        //////////////////////////////////////////////////////////////////
        // Private Members
        //////////////////////////////////////////////////////////////////

        private static readonly Matrix4x4 s_flipHandednessMap = Matrix4x4.Scale(new Vector4(1.0f, 1.0f, -1.0f));

        private ZCameraRig _zCameraRig;
        private ProjectionMatrixCalc _calculator = new ProjectionMatrixCalc();
        private Camera _zCameraCamera = null;
        private Transform _zCameraTransform = null;

        private Camera[] _cameras = new Camera[2];
        private static RenderTexture[] _renderTextures = null;
        private IntPtr[] _nativeTexturePts = new IntPtr[2];
        private UInt16 _imageWidth = 0;
        private UInt16 _imageHeight = 0;
    }
}