using System.Globalization;
using System;
using System.Collections.Generic;
using F3Device.Device;

/// <summary>
/// F3D设备
/// </summary>
public class F3DService
{
    private static F3DService _instance;
    /// <summary>
    /// 单例
    /// </summary>
    public static F3DService Instance
    {
        get
        {
            if (_instance == null)
                _instance = new F3DService();

            _instance.Init();

            return _instance;
        }
    }


    private F3Device.Device.BaseDevice _mainDevice;

    /// <summary>
    /// 主屏窗口所在设备 (IGraph3Device：左右画面  IFrame3Device：帧连续画面)
    /// </summary>
    public F3Device.Device.BaseDevice mainDevice
    {
        get
        {
            return _mainDevice;
        }
    }

    private F3Device.Device.BaseDevice _projectionDevice;

    /// <summary>
    /// 投屏窗口所在设备 (IGraph3Device：左右画面  IFrame3Device：帧连续画面)
    /// </summary>
    public F3Device.Device.BaseDevice projectionDevice
    {
        get
        {
            return _projectionDevice;
        }
    }

    private IntPtr _mainWindowHandle = IntPtr.Zero;

    /// <summary>
    /// 主窗口句柄
    /// </summary>
    public IntPtr mainWindowHandle
    {
        get
        {
            return _mainWindowHandle;
        }
    }

    private F3DService() { }

    /// <summary>
    /// 初始化
    /// 当设备环境更改时需重新初始化
    /// </summary>
    public void Init()
    {
        _mainDevice = FindMainDevice();

        F3Device.Screen.Monitor monitor = F3Device.DeviceManager.Instance.FindProjectionMonitor(_mainWindowHandle);
        if (monitor != null)
        {
            F3Device.Screen.RECT rect = monitor.m_MonitorInfo.rcMonitor;
            _projectionDevice = F3Device.DeviceManager.Instance.FindDevice(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }
    }

    /// <summary>
    /// 返回是否未来立体设备
    /// </summary>
    /// <returns></returns>
    public bool IsGCDevice()
    {
        if (_mainDevice != null)
        {
            if (_mainDevice is GC3000Device)
            {
                return true;
            }
            else if (_mainDevice is GCDevice)
            {
                return true;
            }
        }

        if (projectionDevice != null)
        {
            if (projectionDevice is GC3000Device)
            {
                return true;
            }

            else if (projectionDevice is GCDevice)
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// 控制设备切换2/3D
    /// </summary>
    /// <param name="dev">设备</param>
    /// <param name="is3D">是否为3D</param>
    public void SwitchScreenState(F3Device.Device.BaseDevice dev, bool is3D)
    {
        if (dev == null) return;

        if (dev is IFrame3Device)
        {
            IFrame3Device iDevice = dev as IFrame3Device;
            if (is3D)
                iDevice.Switch_IFrame_3D();
            else
                iDevice.Switch_IFrame_2D();
        }
        else if (dev is IGraph3Device)
        {
            IGraph3Device iDevice = dev as IGraph3Device;
            if (is3D)
                iDevice.Switch_IGraph_LR_3D();
            else
                iDevice.Switch_IGraph_2D();
        }
    }

    /// <summary>
    /// 找出应用所在屏幕的当前设备
    /// </summary>
    private F3Device.Device.BaseDevice FindMainDevice()
    {
        _mainWindowHandle = F3Device.API.GetProcessWnd();

#if UNITY_EDITOR
        _mainWindowHandle = IntPtr.Zero;
        List<IntPtr> list = F3Device.API.GetProcessWndList();
        foreach (IntPtr intptr in list)
            F3Device.API.EnumChildWindows(intptr, new F3Device.API.CHILDWNDENUMPROC(EnumGameViewWindow), 0);
#endif
        return F3Device.DeviceManager.Instance.FindDevice(_mainWindowHandle);
    }

    /// <summary>
    /// 编辑器中找出视图窗口
    /// </summary>
    /// <param name="hwnd"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    private bool EnumGameViewWindow(IntPtr hwnd, int lParam)
    {
        int cTxtLen = F3Device.API.GetWindowTextLength(hwnd.ToInt32()) + 1;
        System.Text.StringBuilder text = new System.Text.StringBuilder(cTxtLen);
        F3Device.API.GetWindowText(hwnd.ToInt32(), text, cTxtLen);
        string title = text.ToString();
        if (title.Contains("GameView"))
        {
            _mainWindowHandle = hwnd;
            return false;
        }
        return true;
    }
}
