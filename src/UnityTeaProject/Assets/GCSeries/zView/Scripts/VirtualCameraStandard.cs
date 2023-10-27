//////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 , Inc.  All Rights Reserved.
//
//////////////////////////////////////////////////////////////////////////

using System;

using UnityEngine;


namespace GCSeries.zView
{
    public class VirtualCameraStandard : VirtualCamera
    {
        //////////////////////////////////////////////////////////////////
        // Unity MonoBehaviour Callbacks
        //////////////////////////////////////////////////////////////////

        void Awake()
        {
            // Dynamically create a new Unity camera and disable it to allow for manual 
            // rendering via Camera.Render().
            _camera = this.gameObject.AddComponent<Camera>();
            _camera.enabled = false;
        }

        //////////////////////////////////////////////////////////////////
        // Virtual Camera Overrides
        //////////////////////////////////////////////////////////////////

        public override void SetUp(ZView zView, IntPtr connection, ZView.ModeSetupPhase phase)
        {
            switch (phase)
            {
                case ZView.ModeSetupPhase.Initialization:
                    this.UpdateImageResolution(zView, connection);
                    break;

                case ZView.ModeSetupPhase.Completion:
                    // Grab the image dimensions from the connection settings.
                    UInt16 imageWidth = zView.GetSettingUInt16(connection, ZView.SettingKey.ImageWidth);
                    UInt16 imageHeight = zView.GetSettingUInt16(connection, ZView.SettingKey.ImageHeight);

                    // Create the render texture.
                    _renderTexture = new RenderTexture((int)imageWidth, (int)imageHeight, 24, RenderTextureFormat.ARGB32);
                    _renderTexture.filterMode = FilterMode.Point;
                    _renderTexture.name = "RenderTextureStandard";
                    _renderTexture.Create();

                    // Cache the render texture's native texture pointer. Per Unity documentation,
                    // calling GetNativeTexturePtr() when using multi-threaded rendering will
                    // synchronize with the rendering thread (which is a slow operation). So, only
                    // call and cache once upon initialization.
                    _nativeTexturePtr = _renderTexture.GetNativeTexturePtr();

                    break;

                default:
                    break;
            }
        }

        public override void TearDown()
        {
            // Reset the camera's target texture.
            _camera.targetTexture = null;

            // Reset the render texture's native texture pointer.
            _nativeTexturePtr = IntPtr.Zero;

            // Clean up the existing render texture.
            if (_renderTexture != null)
            {
                UnityEngine.Object.Destroy(_renderTexture);
                _renderTexture = null;
            }

            // Reset the image width and height.
            _imageWidth = 0;
            _imageHeight = 0;
        }

        public override void Render(ZView zView, IntPtr connection, IntPtr receivedFrame)
        {
            // Check to see if the image width or height changed and update them
            // accordingly.
            this.UpdateImageResolution(zView, connection);

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
            _camera.transform.position = this._zCameraTransform.position;
            _camera.transform.rotation = this._zCameraTransform.rotation;
            _camera.projectionMatrix = this._zCameraCamera.projectionMatrix;

            // Render the scene.
            _camera.cullingMask = cullingMask & ~(zView.StandardModeIgnoreLayers);
            _camera.targetTexture = _renderTexture;
            _camera.Render();

            // Restore the camera's culling mask.
            _camera.cullingMask = cullingMask;
        }

        public override IntPtr GetNativeTexturePtr()
        {
            return _nativeTexturePtr;
        }


        //////////////////////////////////////////////////////////////////
        // Private Methods
        //////////////////////////////////////////////////////////////////

        private void UpdateImageResolution(ZView zView, IntPtr connection)
        {
            // Get the current viewport size.
            UInt16 imageWidth = (UInt16)Screen.currentResolution.width;
            UInt16 imageHeight = (UInt16)Screen.currentResolution.height;

            // Set image width and height.
            if (imageWidth != _imageWidth || imageHeight != _imageHeight)
            {
                // Begin settings batch.
                try
                {
                    zView.BeginSettingsBatch(connection);
                }
                catch (PluginException e)
                {
                    Debug.LogError(string.Format("Failed to begin settings batch for updating image resolution: {0}", e.PluginError));
                    return;
                }

                // Update image resolution.
                try
                {
                    // Update the connection's image resolution settings.
                    zView.SetSetting(connection, ZView.SettingKey.ImageWidth, imageWidth);
                    zView.SetSetting(connection, ZView.SettingKey.ImageHeight, imageHeight);

                    // Update the internally cached image resolution in order to check
                    // if the image resolution has changed in subsequent frames.
                    _imageWidth = imageWidth;
                    _imageHeight = imageHeight;
                }
                catch (PluginException e)
                {
                    Debug.LogError(string.Format("Failed to set image resolution settings: {0}", e.PluginError));
                }

                // End settings batch.
                try
                {
                    zView.EndSettingsBatch(connection);
                }
                catch (PluginException e)
                {
                    Debug.LogError(string.Format("Failed to end settings batch for updating image resolution: {0}", e.PluginError));
                }
            }
        }

        private Matrix4x4 FlipHandedness(Matrix4x4 matrix)
        {
            return s_flipHandednessMap * matrix * s_flipHandednessMap;
        }


        //////////////////////////////////////////////////////////////////
        // Private Members
        //////////////////////////////////////////////////////////////////

        private static readonly Matrix4x4 s_flipHandednessMap = Matrix4x4.Scale(new Vector4(1.0f, 1.0f, -1.0f));

        private Camera _zCameraCamera = null;
        private Transform _zCameraTransform = null;

        private Camera _camera = null;
        private RenderTexture _renderTexture = null;
        private IntPtr _nativeTexturePtr = IntPtr.Zero;
        private UInt16 _imageWidth = 0;
        private UInt16 _imageHeight = 0;
    }
}