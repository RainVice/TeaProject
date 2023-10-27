//**********************************************************************
// Script Name          : Level.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023/10/27
// Last Modified Time   : 2023/10/27
// Description          : 保存场景信息，跳转伪加载
//**********************************************************************

using System;
using TeaProject.Manager;
using TeaProject.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeaProject
{
    /// <summary>
    /// 保存场景信息，跳转伪加载
    /// </summary>
    public class Level : ILevel
    {

        #region  Public fields and properties

        public event Action<AsyncOperation> OnloadEvent;

        #endregion
        
        #region Private fields and properties
        private int m_index;
        private int m_nextIndex;
        #endregion

        #region Public or protected method

        /// <summary>
        /// 需要跳转场景时使用
        /// </summary>
        /// <param name="mIndex">需要跳转场景的索引</param>
        /// <param name="parent">加载动画加载的位置，需要ZCanvas的Transform，不传默认直接跳转</param>
        /// <param name="mNextIndex">下一个场景的索引</param>
        public Level(int mIndex, Transform parent = null, int mNextIndex = -1)
        {
            m_index = mIndex;
            m_nextIndex = mNextIndex;
            if (parent != null)
            {
                UILoadView uiLoadView = UIManager.Instance.Show<UILoadView>(parent);
                uiLoadView.setLevel(this);
            }
        }

        public string GetLevelName()
        {
            return SceneManager.GetSceneAt(m_index).name;
        }

        public ILevel GetNextLevel()
        {
            return new Level(m_nextIndex);
        }

        public int GetLevelIndex()
        {
            return m_index;
        }

        public void Begin()
        {
        }

        public void End()
        {
        }

        public void OnLoad(AsyncOperation operation)
        {
            if (OnloadEvent != null)
            {
                OnloadEvent(operation);
            }
        }

        #endregion
    }
}