using System;
using GCSeries.Core;
using UnityEngine;

namespace GCSeries.zView
{
    public class GVirtualCameraStandard : GVirtualCamera
    {
        void Awake()
        {
            // Dynamically create a new Unity camera and disable it to allow for manual 
            // rendering via Camera.Render().
            _camera = this.gameObject.AddComponent<Camera>();
            if (_camera == null)
            {
                Debug.LogError("GVirtualCameraStandard.Awake():创建相机失败!");
            }
            else
            {
                _camera.enabled = false;
                _camera.targetDisplay = 1;
                _camera.stereoTargetEye = StereoTargetEyeMask.None;
            }

        }

        //////////////////////////////////////////////////////////////////
        // Virtual Camera Overrides
        //////////////////////////////////////////////////////////////////
        public override void SetUp(GView zView, IntPtr connection, GView.ModeSetupPhase phase)
        {
            _zCameraRig = FindObjectOfType<ZCameraRig>();

            _camera.enabled = true;
            _camera.cullingMask = _camera.cullingMask & ~(zView.StandardModeIgnoreLayers);

            switch (phase)
            {
                case GView.ModeSetupPhase.Initialization:
                    //this.UpdateImageResolution(zView, connection);

                    _imageWidth = zView.imageWidth;
                    _imageHeight = zView.imageHeight;
                    RenderTextureDescriptor descriptor = new RenderTextureDescriptor(_imageWidth, _imageHeight, RenderTextureFormat.ARGB32);
                    descriptor.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
                    descriptor.depthBufferBits = 0;

                    if (_renderTexture == null)
                        _renderTexture = Resources.Load<RenderTexture>("gViewRT");

                    // Cache the render texture's native texture pointer. Per Unity documentation,
                    // calling GetNativeTexturePtr() when using multi-threaded rendering will
                    // synchronize with the rendering thread (which is a slow operation). So, only
                    // call and cache once upon initialization.
                    _nativeTexturePtr = _renderTexture.GetNativeTexturePtr();

                    // Cache the camera's culling mask to be restored after it renders the frame.
                    int cullingMask = _camera.cullingMask;

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
                    _camera.CopyFrom(this._zCameraCamera);
                    _camera.enabled = false;
                    _camera.stereoTargetEye = StereoTargetEyeMask.None;

                    // Render the scene.
                    _camera.cullingMask = cullingMask & ~(zView.StandardModeIgnoreLayers);
                    _camera.targetTexture = _renderTexture;

                    break;

                case GView.ModeSetupPhase.Completion:
                    // Grab the image dimensions from the connection settings.


                    // Create the render texture.


                    break;

                default:
                    break;
            }
        }

        public override void TearDown()
        {
            // Reset the camera's target texture.
            _camera.targetTexture = null;
            _camera.enabled = false;

            // Reset the render texture's native texture pointer.
            _nativeTexturePtr = IntPtr.Zero;

            // Clean up the existing render texture.
            // if (_renderTexture != null)
            // {
            //     UnityEngine.Object.Destroy(_renderTexture);
            //     _renderTexture = null;
            // }

            // Reset the image width and height.
            _imageWidth = 0;
            _imageHeight = 0;
        }

        public override void Render(GView zView, IntPtr connection, IntPtr receivedFrame)
        {
            _calculator.SetVector(_zCameraRig.Frame.transform.localToWorldMatrix.MultiplyPoint(new Vector3(-ZProvider.DisplaySize.x / 2f, ZProvider.DisplaySize.y / 2f, 0f)),
                                _zCameraRig.Frame.transform.localToWorldMatrix.MultiplyPoint(new Vector3(-ZProvider.DisplaySize.x / 2f, -ZProvider.DisplaySize.y / 2f, 0f)),
                                _zCameraRig.Frame.transform.localToWorldMatrix.MultiplyPoint(new Vector3(ZProvider.DisplaySize.x / 2f, -ZProvider.DisplaySize.y / 2f, 0f)));

            _camera.transform.position = _zCameraRig.transform.localToWorldMatrix.MultiplyPoint(Vector3.back * ZProvider.WindowSize.magnitude);
            _camera.transform.rotation = _zCameraRig.transform.rotation;
            _camera.projectionMatrix = _calculator.GeneralizedPerspectiveProjection(_camera.transform.position, _camera.nearClipPlane, _camera.farClipPlane);

            _camera.Render();
        }

        public override IntPtr[] GetNativeTexturePtr(out int count)
        {
            count = 1;
            return new IntPtr[] { _nativeTexturePtr };
        }

        public override RenderTexture[] GetRenderTexture(out int count)
        {
            count = 1;
            return new RenderTexture[] { _renderTexture };
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

        private Camera _camera = null;
        private static RenderTexture _renderTexture = null;
        private IntPtr _nativeTexturePtr = IntPtr.Zero;
        private UInt16 _imageWidth = 0;
        private UInt16 _imageHeight = 0;
    }
}