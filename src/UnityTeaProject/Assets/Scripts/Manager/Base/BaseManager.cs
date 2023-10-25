//**********************************************************************
// Script Name          : BaseManager.cs
// Author Name          : 刘坤
// Create Time          : 2023/10/19
// Last Modified Time   : 2023/10/19
// Description          : 编写基本管理器类
//**********************************************************************

using System.Collections;
using UnityEngine;

namespace TeaProject.Manager
{
    /// <summary>
    /// 基本管理器类
    /// </summary>
    public class BaseManager<T> : MonoSingleton<T>, IManager where T : MonoSingleton<T>
    {
    #region Public or protected fields and properties
        /// <summary>
        /// 指示当前管理器是否已经初始化
        /// </summary>
        /// <value>如果为 true，则表示管理器已经初始化；反之，则还为初始化</value>
        protected bool IsReady
        {
            get
            {
                return m_IsReady;
            }
            set
            {
                m_IsReady = value;
            }
        }
    #endregion
    
    #region Private fields and properties
        private bool m_IsReady = false;
    #endregion
    
    #region Public or protected method
        /// <summary>
        /// 初始化管理器。此方法应在派生类实现时应先调用基类方法。
        /// </summary>
        public virtual IEnumerator Init(System.Object arg = null)
        {
            if(m_IsReady)
            {
                string name = GetName();
                Debug.LogWarning($"尝试初始化{name}，而{name}已经完成初始化");
                throw new System.Exception($"重复初始化管理器！Name:[{name}]");
            }
            m_IsReady = true;
            return null;
        }

        /// <summary>
        /// 获取当前管理器名称
        /// </summary>
        /// <returns>一个表示管理器名称的字符串</returns>
        public virtual string GetName()
        {
            return GetType().ToString();
        }
    #endregion

    }
}