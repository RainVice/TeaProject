//**********************************************************************
// Script Name          : MonoSingleton.cs
// Author Name          : 刘坤
// Create Time          : 2023/10/18
// Last Modified Time   : 2023/10/18
// Description          : 编写单例MonoBehaviour基类
//**********************************************************************

using UnityEngine;
using System;

namespace TeaProject
{
    /// <summary>
    /// 单例MonoBehaviour基类
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {

    #region Public or protected fields and properties
        /// <summary>
        /// 静态实例对象，此对象在unity生命周期中全局唯一
        /// </summary>
        /// <value>调用类的实例</value>
        public static T Instance
        {
            get
            {
                if(m_Instance == null)
                {
                    m_Instance = FindObjectOfType<T>();
                    if(m_Instance == null)
                    {
                        throw new Exception("尝试获取单例实例时出现错误！");
                    }
                }
                return m_Instance;
            }
        }

        /// <summary>
        /// 指示此Mono对象在Unity中是否在切换场景时销毁
        /// </summary>
        /// <value>如果为 true，则在切换场景时不会销毁；如果为 false，则会随着场景切换被销毁</value>
        public bool IsGlobal
        {
            get 
            {
                return m_Instance;
            }
        }
    #endregion

    #region Private fields and properties
        private static T m_Instance = null;

        [SerializeField]
        private bool m_IsGlobal = true;
    #endregion

    #region Unity Callback
        private void Awake()
        {
            if(m_Instance == null)
            {
                m_Instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestory()
        {
            if(m_Instance == this)
            {
                m_Instance = null;
            }
        }
    #endregion

    }
}