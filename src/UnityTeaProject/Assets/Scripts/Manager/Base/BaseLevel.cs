//**********************************************************************
// Script Name          : ILevel.cs
// Author Name          : 刘坤
// Create Time          : 2029/10/19
// Last Modified Time   : 2029/11/12
// Description          : 编写关卡基类
//**********************************************************************

using UnityEngine.SceneManagement;
using UnityEngine;
using System;

namespace TeaProject.Manager
{
    /// <summary>
    /// 关卡基类类
    /// </summary>
    public abstract class BaseLevel
    {
        
    #region Public or protected fields and properties
        public Action OnBegin;
        public Action OnEnd;
        public Action<float> OnLoad;
        public int Index
        {
            get
            {
                return m_Index;
            }
            protected set
            {
                if(value < 0 || value >= SceneManager.sceneCountInBuildSettings)
                    Debug.LogWarning("尝试设置关卡索引为一个错误的值！");
                m_Index = value;
            }
        }
    #endregion

    #region Private fields and properties
        private int m_Index;
    #endregion

    #region #region Public or protected method
        public virtual BaseLevel GetNextLevel() {return null;}
    #endregion

    }
}