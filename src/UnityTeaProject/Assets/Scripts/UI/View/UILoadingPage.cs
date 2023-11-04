//**********************************************************************
// Script Name          : UILoadView.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023/10/21
// Last Modified Time   : 2023/11/02
// Description          : 场景通用加载动画
//**********************************************************************

using TeaProject.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace TeaProject.UI
{
    /// <summary>
    /// 场景通用加载动画
    /// </summary>
    public class UILoadingPage : UIMonoBehaviour
    {
        #region Private fields and properties

        //背景图片
        [SerializeField] private GameObject m_background;

        //进度条
        [SerializeField] private Slider m_progressBar;

        //总加载时间
        // private float m_totalTime = 3;

        //当前时间
        private float m_currentTime;

        //是否开始加载
        private bool m_isLoading;

        #endregion

        #region Public or protected method

        public UILoadingPage()
        {
            m_Cache = true;
        }


        public override void OnShow()
        {
            if (LevelManager.Instance.CurrentLevel is Level currentLevel)
            {
                currentLevel.OnLoad += f => m_progressBar.value = f;
            }
        }
    }

    #endregion
}