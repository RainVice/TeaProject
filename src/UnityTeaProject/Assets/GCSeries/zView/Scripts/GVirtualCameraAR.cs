using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using GCSeries.Core;

namespace GCSeries.zView
{

    public class GVirtualCameraAR : GVirtualCamera
    {
        /// <summary>
        /// class for generate a AR cutout mesh
        /// </summary>
        public class CutoutMesh
        {
            public Mesh BoxMesh { get { return _boxMesh; } }
            Mesh _boxMesh;

            public Mesh ScreenMesh { get { return _screenMesh; } }
            Mesh _screenMesh;

            public Mesh SkyBoxMesh { get { return _skyBoxMesh; } }
            Mesh _skyBoxMesh;

            public Vector2 screenSize
            {
                get { return _screenSize; }
                set { UpdateScreenSize(value); }
            }
            private Vector2 _screenSize;

            public Vector3 cutoutSize
            {
                get { return _cutoutSize; }
                set { UpdateCutoutSize(value); }
            }
            private Vector3 _cutoutSize;

            private CutoutMesh() { }

            public CutoutMesh(Vector2 screenSize, Vector3 cutoutSize)
            {
                _screenSize = screenSize;
                _cutoutSize = cutoutSize;
                _boxMesh = CreateMesh(_screenSize, _cutoutSize);
                _screenMesh = CreatePlane(screenSize);
                _skyBoxMesh = CreateSkyPlane(screenSize);
            }

            public void UpdateScreenSize(Vector2 newScreenSize)
            {
                if (newScreenSize == _screenSize) return;

                _screenSize = newScreenSize;
                Vector3[] vertices = BoxMesh.vertices;
                vertices[0] = new Vector3(_screenSize.x / 2, _screenSize.y / 2f, 0);
                vertices[1] = new Vector3(_screenSize.x / 2, -_screenSize.y / 2f, 0);
                vertices[2] = new Vector3(-_screenSize.x / 2, -_screenSize.y / 2f, 0);
                vertices[3] = new Vector3(-_screenSize.x / 2, _screenSize.y / 2f, 0);
                BoxMesh.vertices = vertices;

                Vector3[] ScreenVertices = ScreenMesh.vertices;
                ScreenVertices[0] = new Vector3(-screenSize.x / 2, -_screenSize.y / 2f, 0);
                ScreenVertices[1] = new Vector3(screenSize.x / 2, -_screenSize.y / 2f, 0);
                ScreenVertices[2] = new Vector3(-screenSize.x / 2, screenSize.y / 2f, 0);
                ScreenVertices[3] = new Vector3(screenSize.x / 2, screenSize.y / 2f, 0);
                ScreenMesh.vertices = ScreenVertices;
            }

            public void UpdateCutoutSize(Vector3 newCutoutSize)
            {
                if (newCutoutSize == _cutoutSize) return;

                _cutoutSize = newCutoutSize;
                Vector3[] vertices = _boxMesh.vertices;

                Vector3 halfSize = _cutoutSize * 0.5f;

                vertices[4] = new Vector3(-halfSize.x, halfSize.y, 0);
                vertices[5] = new Vector3(halfSize.x, halfSize.y, 0);
                vertices[6] = new Vector3(halfSize.x, -halfSize.y, 0);
                vertices[7] = new Vector3(-halfSize.x, -halfSize.y, 0);

                vertices[8] = new Vector3(-halfSize.x, halfSize.y, -_cutoutSize.z);
                vertices[9] = new Vector3(halfSize.x, halfSize.y, -_cutoutSize.z);
                vertices[10] = new Vector3(halfSize.x, -halfSize.y, -_cutoutSize.z);
                vertices[11] = new Vector3(-halfSize.x, -halfSize.y, -_cutoutSize.z);

                _boxMesh.vertices = vertices;
            }

            public void UpdateSize(Vector2 newScreenSize, Vector3 newCutoutSize)
            {
                UpdateScreenSize(newScreenSize);
                UpdateCutoutSize(newCutoutSize);
            }

            private Mesh CreateMesh(Vector2 screenSize, Vector3 size)
            {
                // Create the mesh.
                Mesh temp_mesh = new Mesh();
                temp_mesh.name = "BoxMask";
                temp_mesh.vertices = new Vector3[12];
                temp_mesh.triangles = new int[]
                {
                    // Back Face Top:
                    0, 4, 5,
                    0, 5, 1,

                    // Back Face Right:
                    1, 5, 6,
                    1, 6, 2,

                    // Back Face Bottom:
                    2, 6, 7,
                    2, 7, 3,

                    // Back Face Left:
                    3, 7, 4,
                    3, 4, 0,

                    // Top Face:
                    4, 8, 9,
                    4, 9, 5,

                    // Right Face:
                    5, 9, 10,
                    5, 10, 6,

                    // Bottom Face:
                    6, 10, 11,
                    6, 11, 7,

                    // Right Face:
                    7, 11, 8,
                    7, 8, 4,

                    // Front Face:
                    8, 10, 9,
                    8, 11, 10,
                };

                Vector2 halfSize = Vector2.one * 0.5f;
                Vector3[] vertices = temp_mesh.vertices;

                vertices[0] = new Vector3(screenSize.x / 2, screenSize.y / 2f, 0);
                vertices[1] = new Vector3(screenSize.x / 2, -screenSize.y / 2f, 0);
                vertices[2] = new Vector3(-screenSize.x / 2, -screenSize.y / 2f, 0);
                vertices[3] = new Vector3(-screenSize.x / 2, screenSize.y / 2f, 0);

                halfSize = size * 0.5f;

                vertices[4] = new Vector3(-halfSize.x, halfSize.y, 0);
                vertices[5] = new Vector3(halfSize.x, halfSize.y, 0);
                vertices[6] = new Vector3(halfSize.x, -halfSize.y, 0);
                vertices[7] = new Vector3(-halfSize.x, -halfSize.y, 0);

                vertices[8] = new Vector3(-halfSize.x, halfSize.y, -size.z);
                vertices[9] = new Vector3(halfSize.x, halfSize.y, -size.z);
                vertices[10] = new Vector3(halfSize.x, -halfSize.y, -size.z);
                vertices[11] = new Vector3(-halfSize.x, -halfSize.y, -size.z);

                temp_mesh.vertices = vertices;

                return temp_mesh;
            }

            /// <summary>
            /// 屏幕平面
            /// </summary>
            /// <param name="screenSize"></param>
            /// <returns></returns>
            private Mesh CreatePlane(Vector2 screenSize)
            {
                Mesh temp_mesh = new Mesh();
                temp_mesh.name = "screenPlane";
                temp_mesh.vertices = new Vector3[4]
                {
                    new Vector3(-screenSize.x / 2, -screenSize.y / 2f, 0),
                    new Vector3(screenSize.x / 2, -screenSize.y / 2f, 0),
                    new Vector3(-screenSize.x / 2, screenSize.y / 2f, 0),
                    new Vector3(screenSize.x / 2, screenSize.y / 2f, 0)
                };
                temp_mesh.triangles = new int[]
                {
                    // lower left triangle
                    0, 2, 1,
                    // upper right triangle
                    2, 3, 1
                };
                return temp_mesh;
            }

            /// <summary>
            /// 远处天空平面
            /// </summary>
            /// <param name="screenSize"></param>
            /// <returns></returns>
            private Mesh CreateSkyPlane(Vector2 screenSize)
            {
                Mesh temp_mesh = new Mesh();
                temp_mesh.name = "skyPlane";
                temp_mesh.vertices = new Vector3[4]
                {
                    new Vector3(-0.5f, -0.5f, 0),
                    new Vector3(0.5f, -0.5f, 0),
                    new Vector3(-0.5f, 0.5f, 0),
                    new Vector3(0.5f, 0.5f, 0)
                };

                Vector2[] uv = new Vector2[4]
                {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
                };

                temp_mesh.triangles = new int[]
                {
                    // lower left triangle
                    0, 2, 1,
                    // upper right triangle
                    2, 3, 1
                };
                temp_mesh.uv = uv;

                return temp_mesh;
            }
        }

        //标定位置信息
        private Matrix4x4 _camPoseMatrixInDisplaySpace;
        private CommandBuffer _arBuffer;
        private CommandBuffer _cullBuffer;
        private CutoutMesh _cutoutMesh;
        private RenderTexture _depthMask;
        private MeshFilter _skyMeshFilter;
        private MeshFilter _boxMeshFilter;
        private Camera secondCamera;
        private Camera skyBoxCamera;
        private RenderTexture _secondRT;
        private RenderTexture _skyboxRT;
        private GView _zView;
        private static RenderTexture _renderTexture = null;

        private LayerMask _ignoreLayer;
        private LayerMask _environmentMask;

        /// <summary>
        /// 裁剪盒大小
        /// </summary>
        private Vector3 _cutoutMeshSizeScaler = Vector3.one;

        /// <summary>
        /// 屏幕背景色
        /// </summary>
        //private Color _screenMaskColor = new Color(0.572549f, 0.7019608f, 0.8588236f, 1f);

        void Awake()
        {
            // Dynamically create a new Unity camera and disable it to allow for manual 
            // rendering via Camera.Render().
            _arCamera = gameObject.AddComponent<Camera>();

            if (_arCamera == null)
            {
                Debug.LogError("GVirtualCameraAR.Awake():创建相机失败!");
            }
            else
            {
                _arCamera.enabled = false;
                _arCamera.nearClipPlane = 0.03f;

                _arCamera.targetDisplay = 0;
                _arCamera.stereoTargetEye = StereoTargetEyeMask.None;
            }

        }

        void OnPreCull()
        {
            if (skyBoxCamera != null)
            {
                skyBoxCamera.Render();
            }

            if (secondCamera != null)
            {
                secondCamera.Render();
            }
        }

        public override void SetUp(GView zView, IntPtr connection, GView.ModeSetupPhase phase)
        {
            _zView = zView;
            _arCamera.enabled = true;

            _zCameraRig = FindObjectOfType<ZCameraRig>();

            _imageWidth = zView.imageWidth;
            _imageHeight = zView.imageHeight;

            if (_renderTexture == null)
                _renderTexture = Resources.Load<RenderTexture>("gViewRT");

            _arCamera.targetTexture = _renderTexture;

            _ignoreLayer = zView.ARModeIgnoreLayers;
            _environmentMask = zView.ARModeEnvironmentLayers;
            _cutoutMeshSizeScaler = zView.ARModeMaskSize;
            //_screenMaskColor = zView.screenMaskColor;

            _cullBuffer = null;
            _arBuffer = null;
            ReadArCamTransform(out _camPoseMatrixInDisplaySpace);
            StartCoroutine(InitCamera(SetupCommandBuffer));
        }

        public override void TearDown()
        {
            CloseCamera();

            if (_cullBuffer != null)
            {
                _cullBuffer.Release();
                _cullBuffer = null;
            }

            if (_arBuffer != null)
            {
                _arBuffer.Release();
                _arBuffer = null;
            }

            if (secondCamera != null)
            {
                secondCamera.enabled = false;
            }

            _arCamera.RemoveAllCommandBuffers();
            _arCamera.enabled = false;

            if (_depthMask != null)
            {
                _depthMask.Release();
                _depthMask = null;
            }

            if (_secondRT != null)
            {
                _secondRT.Release();
                _secondRT = null;
            }

            if (_skyboxRT != null)
            {
                _skyboxRT.Release();
                _skyboxRT = null;
            }

            if (_skyMeshFilter != null)
            {
                _skyMeshFilter.gameObject.SetActive(false);
            }
        }

        public override void Render(GView zView, IntPtr connection, IntPtr receivedFrame)
        {
            if (_cutoutMesh != null)
            {
                // The camera's parent transform represents viewport center and
                // is used here to align the AR camera against.
                Transform cameraParentTransform = zView.ActiveZCamera?.transform.parent;

                // 实时更新相机位置
                // TODO:GCSeries编辑器下这个值不会随拖动编辑器窗口而修改，发布后表现一致
                Matrix4x4 temp_matrix = Matrix4x4.TRS(cameraParentTransform.position,
                                                       cameraParentTransform.rotation,
                                                       Vector3.one);
                //Debug.Log($"GVirtualCameraAr.Render(): ViewportMatrix = {temp_matrix}");
                Matrix4x4 displayToWorld = cameraParentTransform?.localToWorldMatrix ?? Matrix4x4.identity;
                //displayToWorld.SetColumn(3, new Vector4(0.1257f, -0.04232f, 0.0f, 1f));//这个是返回的结果
                Matrix4x4 cameraWorldMatrix = displayToWorld * _camPoseMatrixInDisplaySpace;
                transform.position = cameraWorldMatrix.GetColumn(3);
                transform.rotation = Quaternion.LookRotation(cameraWorldMatrix.GetColumn(2), cameraWorldMatrix.GetColumn(1));

                //更新遮挡mesh
                _cutoutMesh.screenSize = ZProvider.DisplayReferenceSize * _zCameraRig.ViewerScale;
                _cutoutMesh.cutoutSize = _cutoutMeshSizeScaler * _zCameraRig.ViewerScale;

                _boxMeshFilter.mesh.vertices = _cutoutMesh.BoxMesh.vertices;
                _boxMeshFilter.gameObject.transform.position = cameraParentTransform.position;
                _boxMeshFilter.gameObject.transform.rotation = cameraParentTransform.rotation;

                if (_cullBuffer != null)
                {
                    //更新绘制命令矩阵
                    _cullBuffer.Clear();
                    _cullBuffer.SetRenderTarget(_depthMask);
                    _cullBuffer.ClearRenderTarget(true, true, Color.black);
                    _cullBuffer.DrawMesh(_cutoutMesh.BoxMesh, temp_matrix, _depthRenderMat);
                }
                _arCamera.Render();
            }
        }

        // public override IntPtr GetNativeTexturePtr()
        // {
        //     Debug.LogError("GVirtualCameraAR.GetNativeTexturePtr():未使用渲染到纹理");
        //     if (_renderTexture == null) return IntPtr.Zero;
        //     return _renderTexture.GetNativeTexturePtr();
        // }
        public override IntPtr[] GetNativeTexturePtr(out int count)
        {
            count = 1;
            if (_renderTexture == null) return new IntPtr[] { IntPtr.Zero };
            return new IntPtr[] { _renderTexture.GetNativeTexturePtr() };
        }

        public override RenderTexture[] GetRenderTexture(out int count)
        {
            count = 1;
            return new RenderTexture[] { _renderTexture };
        }

        Material _depthRenderMat;

        private void SetupCommandBuffer(Texture webCamTexture)
        {
            // The camera's parent transform represents viewport center and
            // is used here to align the AR camera against.
            Transform cameraParentTransform = _zView.ActiveZCamera?.transform.parent;

            //arCamera.depthTextureMode = DepthTextureMode.Depth;
            _arCamera.renderingPath = RenderingPath.Forward;
            _arCamera.clearFlags = CameraClearFlags.Color;
            _arCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
            _arCamera.cullingMask = _arCamera.cullingMask & ~_ignoreLayer;
            _cutoutMesh = new CutoutMesh(ZProvider.DisplayReferenceSize, Vector3.one);

            if (_skyMeshFilter == null)
            {
                GameObject skyOBJ = new GameObject();
                skyOBJ.transform.parent = transform;
                skyOBJ.hideFlags = HideFlags.HideAndDontSave;
                skyOBJ.name = "HiddenSkyObj";
                _skyMeshFilter = skyOBJ.AddComponent<MeshFilter>();
                MeshRenderer boxRenderer = skyOBJ.AddComponent<MeshRenderer>();
                Material boxMat = new Material(Shader.Find("GcAR/SkyPlane"));
                boxRenderer.material = boxMat;
                boxRenderer.material.mainTexture = _skyboxRT;
                _skyMeshFilter.mesh = _cutoutMesh.SkyBoxMesh;
            }
            _skyMeshFilter.gameObject.SetActive(true);
            _skyMeshFilter.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            _skyMeshFilter.gameObject.transform.rotation = Quaternion.identity;
            _skyMeshFilter.mesh.vertices = new Vector3[4]
            {
                _arCamera.ScreenToWorldPoint(new Vector3(0f, 0f, _arCamera.farClipPlane * 0.9f)),
                _arCamera.ScreenToWorldPoint(new Vector3(_imageWidth, 0f, _arCamera.farClipPlane * 0.9f)),
                _arCamera.ScreenToWorldPoint(new Vector3(0f, _imageHeight, _arCamera.farClipPlane * 0.9f)),
                _arCamera.ScreenToWorldPoint(new Vector3(_imageWidth, _imageHeight, _arCamera.farClipPlane * 0.9f))
            };

            //这个buffer可以避免透明物体与环境物体混合问题
            //不过环境物体需要使用本例中StandardEnvironment.shader材质
            if (_boxMeshFilter == null)
            {
                GameObject boxOBJ = new GameObject();
                boxOBJ.transform.parent = transform;
                boxOBJ.hideFlags = HideFlags.HideAndDontSave;
                boxOBJ.name = "HiddenBoxObj";
                _boxMeshFilter = boxOBJ.AddComponent<MeshFilter>();
                MeshRenderer boxRenderer = boxOBJ.AddComponent<MeshRenderer>();
                Material boxMat = new Material(Shader.Find("GcAr/StencilWriter"));
                boxRenderer.materials = new Material[] { boxMat };
                _boxMeshFilter.mesh = _cutoutMesh.BoxMesh;
            }
            _boxMeshFilter.gameObject.transform.position = cameraParentTransform.position;
            _boxMeshFilter.gameObject.transform.rotation = cameraParentTransform.rotation;

            if (_cullBuffer == null)
            {
                _cullBuffer = new CommandBuffer();
                if (_depthMask == null)
                    _depthMask = new RenderTexture(_imageWidth, _imageHeight, 24);
                _cullBuffer.SetRenderTarget(_depthMask);
                _cullBuffer.ClearRenderTarget(true, true, Color.black);
                _cullBuffer.name = "Draw Cutout Mesh";
                _depthRenderMat = new Material(Shader.Find("GcAR/DepthRenderer"));
                Matrix4x4 temp_matrix = Matrix4x4.TRS(cameraParentTransform.position,
                                                        cameraParentTransform.rotation,
                                                        Vector3.one);
                _cullBuffer.DrawMesh(_cutoutMesh.BoxMesh, temp_matrix, _depthRenderMat);
                _arCamera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, _cullBuffer);
            }

            if (_arBuffer == null)
            {
                _arBuffer = new CommandBuffer();
                Material material = new Material(Shader.Find("GcAR/DepthGrayscale"));

                _arBuffer.name = "GcAr Buffer";
                int customDepthID = Shader.PropertyToID("_customDepthMask");
                _arBuffer.GetTemporaryRT(customDepthID, -1, -1, 0, FilterMode.Bilinear);
                _arBuffer.Blit(_depthMask, customDepthID);

                int depthCameraID = Shader.PropertyToID("_noneCameraDepthTexture");
                _arBuffer.GetTemporaryRT(depthCameraID, -1, -1, 0, FilterMode.Bilinear);
                _arBuffer.Blit(_secondRT, depthCameraID);

                if (webCamTexture != null)
                {
                    int webCamTextureID = Shader.PropertyToID("_webCamTexture");
                    _arBuffer.GetTemporaryRT(webCamTextureID, -1, -1, 0, FilterMode.Bilinear);
                    _arBuffer.Blit(webCamTexture, webCamTextureID);
                }

                int texID = Shader.PropertyToID("_RenderTexture");
                _arBuffer.GetTemporaryRT(texID, -1, -1, 0, FilterMode.Bilinear);
                _arBuffer.Blit(BuiltinRenderTextureType.CameraTarget, texID);
                _arBuffer.Blit(texID, BuiltinRenderTextureType.CameraTarget, material);
                _arCamera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, _arBuffer);
            }

        }

        /// <summary>
        /// 读取标定位置
        /// </summary>
        private void ReadArCamTransform(out Matrix4x4 viewPoseMatrixInDisplaySpace)
        {
            viewPoseMatrixInDisplaySpace = Matrix4x4.identity;
            bool readPoseSuccess = false;
            try
            {
                Matrix4x4 mat = new Matrix4x4();
                GView.FARError res = GView.xxGetARCameraPose(out mat);
                if (res == GView.FARError.Ok)
                {
                    viewPoseMatrixInDisplaySpace = mat;
                    readPoseSuccess = true;
                    UnityEngine.Debug.Log("GVirtualCameraAR.ReadArCamTransform():FAR相机Pose读取成功.");
                }
                else if (res == GView.FARError.Unknown)
                {
                    UnityEngine.Debug.LogError("GVirtualCameraAR.ReadArCamTransform():未知错误!");
                }
                else if (res == GView.FARError.NoCalib)
                {
                    UnityEngine.Debug.LogError("GVirtualCameraAR.ReadArCamTransform():先使用工具软件进行罗技摄像头的标定.");
                }
                else if (res == GView.FARError.NoLicense)
                {
                    UnityEngine.Debug.LogError("GVirtualCameraAR.ReadArCamTransform():系统没有FAR的License.");
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("GVirtualCameraAR.ReadArCamTransform():执行xxGetARCameraPose()异常!e=" + e.Message);
            }
            if (!readPoseSuccess)
            {
                Debug.LogWarning("GVirtualCameraAR.ReadArCamTransform():FAR结果读取失败,设置一个默认值.");
                //*********GCSeries的标定位置矩阵**********
                viewPoseMatrixInDisplaySpace.SetColumn(0, new Vector4(0.58063f, -0.57597f, -0.57543f, 0f));
                viewPoseMatrixInDisplaySpace.SetColumn(1, new Vector4(0.38956f, 0.81716f, -0.42485f, 0f));
                viewPoseMatrixInDisplaySpace.SetColumn(2, new Vector4(0.71492f, 0.02251f, 0.69885f, 0f));
                viewPoseMatrixInDisplaySpace.SetColumn(3, new Vector4(-0.3647f, -0.02495f, -0.3946f, 1f));
                //*********************
            }
        }

        /// <summary>
        /// 相机采图
        /// </summary>
        private static WebCamTexture _camTex;

        IEnumerator InitCamera(Action<Texture> callback = null)
        {
            if (!Application.isPlaying) yield break;
            //获取授权
            //yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                var devices = WebCamTexture.devices;
                string _deviceName = "";
                foreach (var item in devices)
                {
                    //因为红外追踪相机都会以F3DXXXX命名
                    if (!item.name.Contains("F3D"))
                    {
                        _deviceName = item.name;
                        _arCamera.fieldOfView = 42.3f;

                        if (secondCamera == null)
                        {
                            GameObject secondCameraOBJ = new GameObject();
                            secondCameraOBJ.hideFlags = HideFlags.HideAndDontSave;
                            secondCameraOBJ.name = "SecondCamera";
                            secondCameraOBJ.transform.parent = transform;
                            secondCamera = secondCameraOBJ.AddComponent<Camera>();
                            secondCamera.stereoTargetEye = StereoTargetEyeMask.None;
                        }
                        if (_secondRT == null)
                            _secondRT = new RenderTexture(_imageWidth, _imageHeight, 24);

                        secondCamera.enabled = false; //渲染深度图可能会慢一帧
                        secondCamera.CopyFrom(_arCamera);
                        secondCamera.cullingMask = _arCamera.cullingMask & ~(_ignoreLayer) & ~(_environmentMask);
                        secondCamera.SetReplacementShader(Shader.Find("GcAR/DepthReplacement"), "");
                        secondCamera.targetTexture = _secondRT;

                        if (skyBoxCamera == null)
                        {
                            GameObject skyBoxCameraOBJ = new GameObject();
                            skyBoxCameraOBJ.hideFlags = HideFlags.HideAndDontSave;
                            skyBoxCameraOBJ.name = "skyBoxCameraOBJ";
                            skyBoxCameraOBJ.transform.parent = transform;
                            skyBoxCamera = skyBoxCameraOBJ.AddComponent<Camera>();
                            skyBoxCamera.CopyFrom(_arCamera);
                            skyBoxCamera.cullingMask = 0;
                            skyBoxCamera.clearFlags = CameraClearFlags.Skybox;


                            if (_skyboxRT == null)
                                _skyboxRT = new RenderTexture(_imageWidth, _imageHeight, 24);
                            skyBoxCamera.targetTexture = _skyboxRT;
                            skyBoxCamera.enabled = false;
                        }

                        break;
                    }
                }
                if (string.IsNullOrEmpty(_deviceName))
                {
                    UnityEngine.Debug.LogError("GVirtualCameraAR.InitCamera():相机启动失败，没有外接相机");
                    yield break;
                }
                else
                {
                    if (_camTex == null)
                    {
                        _camTex = new WebCamTexture(_deviceName, 1280, 720, 30);//设置为1280x720可以减少相机延迟
                        _camTex.Play();
                    }
                    else
                    {
                        _camTex.Play();
                    }

                    if (callback != null)
                        callback.Invoke(_camTex);
                    UnityEngine.Debug.Log("GVirtualCameraAR.InitCamera():相机启动");
                }

            }
        }

        /// <summary>
        /// 关闭罗技相机采图
        /// </summary>
        public void CloseCamera()
        {
            if (_camTex != null)
            {
                if (_camTex.isPlaying)
                {
                    _camTex.Stop();
                }
            }
        }

        private void OnApplicationQuit()
        {
            CloseCamera();
        }

        private Camera _arCamera = null;
        private UInt16 _imageWidth = 1920;
        private UInt16 _imageHeight = 1080;
        private ZCameraRig _zCameraRig;
        // private readonly Vector2 _screenSize = new Vector2(0.52f, 0.2925f);
    }
}