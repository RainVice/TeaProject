//**********************************************************************
// Script Name          : UILoadView.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023/10/21
// Last Modified Time   : 2023/10/27
// Description          : 场景通用加载动画
//**********************************************************************

using UnityEngine;
using UnityEngine.UI;

namespace TeaProject.UI
{
    /// <summary>
    /// 场景通用加载动画
    /// </summary>
    public class UILoadView:UIMonoBehaviour
    {
        #region Private fields and properties
        //背景图片
        [SerializeField]
        private GameObject m_background;

        //进度条
        private Slider m_progressBar;

        //总加载时间
        private float m_totalTime = 5;

        //当前时间
        private float m_currentTime = 0;

        //是否开始加载
        private bool m_isLoading = false;
        
        //当前所在Level
        private Level m_Level;
        #endregion

        #region Unity Callback
        private void Update()
        {
            //加载动画
            if (m_isLoading)
            {
                m_currentTime += Time.deltaTime;
                if (m_currentTime / m_totalTime < 0.95f)
                {
                    m_progressBar.value = m_currentTime / m_totalTime;
                }
                else
                {
                    m_isLoading = false;
                    m_Level.OnloadEvent += operation => operation.allowSceneActivation = true;
                }
            }
        }
        #endregion

        #region Public or protected method

        public override void OnShow()
        {
            m_progressBar = GetComponentInChildren<Slider>();
            m_isLoading = true;
        }

        public void setLevel(Level level)
        {
            m_Level = level;
        }

        #endregion
    }





}