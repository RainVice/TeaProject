//**********************************************************************
// Script Name          : UIWindow.cs（必填）
// Author Name          : 刘坤
// Create Time          : 2023/10/20
// Last Modified Time   : 2023/10/20
// Description          : 编写UI基类
//**********************************************************************

using UnityEngine;

namespace TeaProject.UI
{
    public abstract class UIMonoBehaviour : MonoBehaviour 
    {

    #region Public or protected method
        /// <summary>
        /// UI控件被显示时的回调函数
        /// </summary>
        public virtual void Show() {}
        /// <summary>
        /// UI控件被关闭时的回调函数
        /// </summary>
        public virtual void Close() {}
    #endregion

    }
}