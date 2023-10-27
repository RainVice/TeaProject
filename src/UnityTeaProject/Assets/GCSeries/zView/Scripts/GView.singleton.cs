using System;
using System.Collections.Generic;

using UnityEngine;

namespace GCSeries.zView
{
    public partial class GView : MonoBehaviour
    {
        private class GlobalState
        {
            /// <summary>
            /// 
            /// </summary>
            public static GlobalState Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new GlobalState();
                    }

                    return _instance;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public static void DestroyInstance()
            {
                if (_instance != null)
                {
                    _instance.ShutDown();
                }

                _instance = null;
            }

            /// <summary>
            /// Returns a reference to the zView SDK's context.
            /// </summary>
            public IntPtr Context
            {
                get
                {
                    return _context;
                }
                set
                {
                    _context = value;
                }

            }

            /// <summary>
            /// Returns a reference to the standard mode handle.
            /// </summary>
            public IntPtr ModeStandard
            {
                get
                {
                    return _modeStandard;
                }
                set
                {
                    _modeStandard = value;
                }
            }

            /// <summary>
            /// Returns a reference to the standard mode handle.
            /// </summary>
            public IntPtr ModeThreeD
            {
                get
                {
                    return _modeThreeD;
                }
                set
                {
                    _modeThreeD = value;
                }
            }

            /// <summary>
            /// Returns a reference to the augmented reality mode handle.
            /// </summary>
            public IntPtr ModeAugmentedReality
            {
                get
                {
                    return _modeAugmentedReality;
                }
                set
                {
                    _modeAugmentedReality = value;
                }
            }

            /// <summary>
            /// Return a reference to the current active connection.
            /// </summary>
            public IntPtr Connection
            {
                get
                {
                    return _connection;
                }
                set
                {
                    _connection = value;
                }
            }

            /// <summary>
            /// Returns whether the GCSeries zView SDK was properly initialized.
            /// </summary>
            public bool IsInitialized
            {
                get
                {
                    return _isInitialized;
                }
            }

            /// <summary>
            /// 记录当前的VirtualCamera模式
            /// </summary>
            public IntPtr virtualCameraMode = IntPtr.Zero;

            /// <summary>
            /// 记录当前的VideoRecording状态
            /// </summary>
            public VideoRecordingState videoRecordingState = VideoRecordingState.NotAvailable;


            private GlobalState()
            {
                // Initialize the zView context.
                //PluginError error = zvuInitialize(NodeType.Presenter, out _context);
                _context = new IntPtr(0x01);//假装构造了一个

                // Get both standard and augmented reality modes.
                List<ZVSupportedMode> supportedModes = new List<ZVSupportedMode>();

                //_modeStandard = this.GetMode(_context, CompositingMode.None, CameraMode.LocalHeadTracked);
                //if (_modeStandard != IntPtr.Zero)
                //{
                //zview的标准模式就是相机跟随head一起运动的,所以直接支持
                supportedModes.Add(
                    new ZVSupportedMode
                    {
                        mode = _modeStandard,
                        modeAvailability = ModeAvailability.Available
                    });
                //}

                //_modeAugmentedReality = this.GetMode(_context, CompositingMode.AugmentedRealityCamera, CameraMode.RemoteMovable);
                //if (_modeAugmentedReality != IntPtr.Zero)
                //{
                //这里如果是存在罗技相机,那么就应该是支持ar模式?
                supportedModes.Add(
                    new ZVSupportedMode
                    {
                        mode = _modeAugmentedReality,
                        modeAvailability = ModeAvailability.Available
                    });
                //}

                //但是这里是传给c++的,目前不需要了
                // Set the context's supported modes.
                //error = zvuSetSupportedModes(_context, supportedModes.ToArray(), supportedModes.Count);
                //if (error != PluginError.Ok)
                //{
                //    Debug.LogError(string.Format("Failed to set supported modes: ({0})", error));
                //}

                //// Set the context's supported capabilities.
                //error = zvuSetSupportedCapabilities(_context, null, 0);
                //if (error != PluginError.Ok)
                //{
                //    Debug.LogError(string.Format("Failed to set supported capabilities: ({0})", error));
                //}

                //// Start listening for new connections.
                //error = zvuStartListeningForConnections(_context, ZView.StringToNativeUtf8(string.Empty));
                //if (error != PluginError.Ok)
                //{
                //    Debug.LogError(string.Format("Failed to start listening for connections: ({0})", error));
                //}

                //假设我现在已经支持了这两种模式
                _modeStandard = new IntPtr(0x11);
                _modeThreeD = new IntPtr(0x13);
                _modeAugmentedReality = new IntPtr(0x12);

                _isInitialized = true;

            }

            ~GlobalState()
            {
                ShutDown();
            }

            private void ShutDown()
            {
                if (_isInitialized)
                {
                    // Clear out handles.
                    _context = IntPtr.Zero;
                    _modeStandard = IntPtr.Zero;
                    _modeAugmentedReality = IntPtr.Zero;
                    _connection = IntPtr.Zero;

                    _isInitialized = false;
                }
            }

            private string GetProjectName()
            {
                string projectName = string.Empty;

                string[] s = Application.dataPath.Split('/');
                if (s.Length > 1)
                {
                    projectName = s[s.Length - 2];
                }

                return projectName;
            }


            //////////////////////////////////////////////////////////////////
            // Private Members
            //////////////////////////////////////////////////////////////////

            private static GlobalState _instance;

            private IntPtr _context = IntPtr.Zero;
            private IntPtr _modeStandard = IntPtr.Zero;
            private IntPtr _modeThreeD = IntPtr.Zero;
            private IntPtr _modeAugmentedReality = IntPtr.Zero;
            private IntPtr _connection = IntPtr.Zero;
            private bool _isInitialized = false;
        }
    }
}

