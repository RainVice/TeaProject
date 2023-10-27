using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace GCSeries
{
    public static class FARDll
    {
        [DllImport("f-ar")]
        internal static extern int fmBeginFrame();
        [DllImport("f-ar")]
        internal static extern int fmEndFrame();
        [DllImport("f-ar")]
        private static extern int fmSetStereoDisplayTextures(System.IntPtr leftNativePTR, System.IntPtr rightNativePTR, int dxgiFormat);
        [DllImport("f-ar")]
        private static extern int fmARStartViewDX11(System.IntPtr hWnd, System.IntPtr leftNativePTR, System.IntPtr rightNativePTR, int w, int h);
        [DllImport("f-ar")]
        private static extern void fmARSwitchProjector(int type);
        [DllImport("f-ar")]
        private static extern void fmARIsGamaSpace(int cSpace);
        [DllImport("f-ar")]
        private static extern void fmARStopView();
        /// <summary>
        /// 帧结束回调
        /// </summary>
        /// <returns></returns>
        [DllImport("f-ar")]
        public static extern IntPtr GetRenderEventFunc();

        /// <summary> 
        /// 描述屏幕坐标信息与ID信息  
        /// </summary> 
        [StructLayout(LayoutKind.Sequential)]
        public struct GCinfo
        {
            public bool isGCmonitor;
            public int RCleft;
            public int RCright;
            public int RCtop;
            public int RCbottom;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
            public string DeviceName;
        };

        /// <summary> 
        /// 投屏状态值 
        /// </summary> 
        public enum FAR_Status
        {
            /// <summary> 
            /// 成功 
            /// </summary> 
            FAR_Ok = 1,
            /// <summary> 
            /// 没被初始化或正在初始化中 
            /// </summary> 
            FAR_NotInitialized = 0,
            /// <summary> 
            /// 非法硬件设备 
            /// </summary> 
            FAR_Illegal = -1,
            /// <summary> 
            /// 窗口句柄丢失 
            /// </summary> 
            FAR_InvaliHwndHandle = -2,
            /// <summary> 
            /// 渲染设备初始化失败 
            /// </summary> 
            FAR_D3DFailed = -3,
            /// <summary> 
            /// 纹理句柄丢失 
            /// </summary> 
            FAR_InvaliTexturedHandle = -4,
            /// <summary> 
            /// 渲染等待进程超时 
            /// </summary> 
            FAR_Timeout = -5,
            /// <summary> 
            /// windows版本低于win10 
            /// </summary> 
            FAR_SysNnotSupported = -6,
            /// <summary> 
            /// 显卡型号小于900 
            /// </summary> 
            FAR_GraphicsCardUnsupported = -7
        }

        /// <summary> 
        /// 通过EDID获取屏幕信息,返回屏幕坐标列表 
        /// </summary> 
        /// <returns> 
        /// result >= 0 返回当前屏幕个数 
        /// result = -1 获取驱动失败 
        /// result = -2 读取EDID失败 
        /// </returns> 
        [DllImport("f-ar")]
        public static extern int fmARUpdatePhysicalMonitor();

        /// <summary> 
        /// 获取缓存中所有显示器数量 
        /// </summary> 
        /// <returns>缓存中所有显示器数量</returns> 
        [DllImport("f-ar")]
        public static extern int fmARGetMonitorCount();

        /// <summary> 
        /// 输入index返回GCinfo数据 
        /// GCinfo* out_struct需要在外部创建内存 
        /// </summary> 
        /// <param name="out_struct">显示器坐标与设备ID信息</param> 
        /// <param name="index">需要获取的屏幕索引</param> 
        /// <returns> 
        /// result =  1 数据获取成功 
        /// result = -1 out_struct为空指针 
        /// result = -2 index越界 
        /// </returns> 
        [DllImport("f-ar")]
        public static extern int fmARGetMonitorInfoByIndex(ref GCinfo out_struct, int index);

        ///------------------------------------------------------------------------------------------------- 
        /// <summary> 独一个json文件路径. </summary> 
        /// 
        /// <remarks> Dx, 2019/5/31. </remarks> 
        /// 
        /// <param name="fviewFile"> [in,out] If non-null, the 
        ///                          fview file. </param> 
        /// 
        /// <returns> An int. </returns> 
        ///------------------------------------------------------------------------------------------------- 
        [DllImport("f-ar")]
        public static extern int fmFViewReadJson();
        //返回标定的位置信息 
        [DllImport("f-ar")]
        public static extern void fmFViewGetPosition(IntPtr value);
        //返回标定的旋转信息 
        [DllImport("f-ar")]
        public static extern void fmFViewGetRotation(IntPtr value);


        /// <summary> 
        /// user32 API 用于处理投屏窗口位置 
        /// </summary> 
        /// <param name="lpClassName"></param> 
        /// <param name="lpWindowName"></param> 
        /// <returns></returns> 
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        ///根据窗口句柄获取pid 
        [DllImport("User32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        ///移动窗口位置 
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool BRePaint);

        /// <summary> 
        /// 当前项目的色彩空间 
        /// PlayerSetting->OtherSetting->ColorSpase 
        /// </summary> 
        public enum U3DColorSpace
        {
            Gama = 0,
            Linear = 1
        }
        private static U3DColorSpace currentColorSpace = FARDll.U3DColorSpace.Gama;
        /// <summary>
        /// 记录当前的色彩空间
        /// </summary>
        public static U3DColorSpace CurrentColorSpace
        {
            get
            {
                return currentColorSpace;

            }
            set
            {
                //设置当前的色彩空间，u3d默认是Gama空间 
                FARDll.SetColorSpace(value);
                currentColorSpace = value;
            }
        }

        /// <summary>
        /// 帧顺序输出执行
        /// </summary>
        /// <returns></returns>
        public static int BeginFrame()
        {
            return fmBeginFrame();
        }

        /// <summary>
        /// 帧顺序输出执行
        /// </summary>
        /// <returns></returns>
        public static int EndFrame()
        {
            return fmEndFrame();
        }

        /// <summary>
        /// 将纹理目标传递到底层
        /// </summary>
        /// <param name="leftNativePTR"></param>
        /// <param name="rightNativePTR"></param>
        /// <param name="dxgiFormat"></param>
        /// <returns></returns>
        public static int SetStereoDisplayTextures(System.IntPtr leftNativePTR, System.IntPtr rightNativePTR, int dxgiFormat)
        {
            return fmSetStereoDisplayTextures(leftNativePTR, rightNativePTR, dxgiFormat);
        }

        /// <summary> 
        /// 投屏投影方式 
        /// </summary> 
        public enum ProjectorType
        {
            /// <summary> 
            /// 投屏为2D模式 
            /// ---------- 
            /// |    L   |  
            /// ----------- 
            /// </summary> 
            _2D = 0,
            /// <summary> 
            /// 投屏为左右3D模式 
            /// ----------- 
            /// |  L |  R |  
            /// -----------  
            /// </summary> 
            LeftRight = 1//左右投影 
        }
        public static ProjectorType CurProjectorType = ProjectorType._2D;
        /// <summary> 
        /// 在目标窗口中渲染纹理指针 
        /// </summary> 
        /// <param name="hWnd">创建好的窗口句柄</param> 
        /// <param name="leftNativePTR">左半屏的左眼纹理指针</param> 
        /// <param name="rightNativePTR">右半屏的右眼纹理指针，此参数为空代表2D投屏</param> 
        /// <returns> 
        ///  返回错误代码 
        /// errorCode >0 成功 
        /// errorCode ==-1 不在自己的机器上 
        /// errorCode ==-2 窗口句柄丢失 
        /// errorCode ==-3 纹理句柄丢失 
        /// errorCode ==-4 渲染设备初始化失败 
        /// errorCode ==-5 渲染等待进程超时 
        /// errorCode ==-6 windows版本低于win10 
        /// </returns> 
        public static int StartView(IntPtr hWnd, IntPtr leftNativePTR, IntPtr rightNativePTR)
        {
            errorCode = fmARStartViewDX11(hWnd, leftNativePTR, rightNativePTR, SwapchainWidth, SwapchanHeight);
            return errorCode;
        }

        /// <summary> 
        /// 设置当前的色彩空间，u3d默认是Gama空间 
        /// </summary> 
        /// <param name="cSpace">u3d工程使用的色彩空间</param> 
        private static void SetColorSpace(U3DColorSpace cSpace)
        {
            currentColorSpace = cSpace;
            fmARIsGamaSpace((int)cSpace);
        }

        /// <summary> 
        ///切换投影方式 
        ///调用StartView_LR(...)后，可切换到只显示左画面到投屏窗口或左右一起显示  
        /// -----------       ---------- 
        /// |  L |  R |  or   |   L    |  
        /// -----------       ----------  
        ///如果只调用StartView(...)，则此函数无效  
        /// </summary> 
        /// <param name="type">目标投屏方式</param> 
        public static void SwitchProjector(ProjectorType type)
        {
            fmARSwitchProjector((int)type);
            CurProjectorType = type;
        }

        /// <summary> 
        /// 安全关闭窗口 
        /// </summary> 
        public static void CloseDown()
        {
            fmARStopView();
        }
        /// <summary> 
        /// 投屏窗口交换链路宽度 
        /// </summary> 
        public const int SwapchainWidth = 1920;
        /// <summary> 
        /// 投屏窗口交换链路高度 
        /// </summary> 
        public const int SwapchanHeight = 1080;
        private static int errorCode = 0;
        /// <summary> 
        /// 显示窗口进程 
        /// </summary> 
        public static Process viewProcess;
    }
}
