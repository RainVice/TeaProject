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
    /// <summary>
    /// UI控件基类
    /// </summary>
    public abstract class UIMonoBehaviour : MonoBehaviour 
    {

    #region Public or protected fields and properties
        /// <summary>
        /// 指示此UI控件是否要缓存
        /// </summary>
        /// <value>如果为 true，则表示需要缓存；如果为 false，则无需缓存。</value>
        public bool Cache
        {
            get
            {
                return m_Cache;
            }
        }
    #endregion

    #region Private fields and properties
        /// <summary>
        /// 指示缓存的内部变量。派生类中可以修改此值。
        /// </summary>
        protected bool m_Cache = false;
    #endregion

    #region Public or protected method
        /// <summary>
        /// UI控件被显示时的回调函数
        /// </summary>
        public virtual void OnShow() {}
        /// <summary>
        /// UI控件被关闭时的回调函数
        /// </summary>
        public virtual void OnClose() {}
    #endregion

    }
}