using System.Net.Mime;
//**********************************************************************
// Script Name          : UIWindow.cs（必填）
// Author Name          : 刘坤
// Create Time          : 2023/10/20
// Last Modified Time   : 2023/10/20
// Description          : 编写UI窗口基类
//**********************************************************************

using UnityEngine;
using UnityEngine.UI;

namespace TeaProject.UI
{
    /// <summary>
    /// 窗口类UI组件的基类
    /// </summary>
    public abstract class UIWindow : UIMonoBehaviour
    {
        [SerializeField]
        private Image m_BackGround;
        [SerializeField]
        private Text m_Context;
        [SerializeField]
        private Text m_Title;
        [SerializeField] 
        private Button m_CloseBtn;
    }
}