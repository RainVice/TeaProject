//**********************************************************************
// Script Name          : SideButton.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023/11/1
// Last Modified Time   : 2023/11/1
// Description          : ZFrame切换控制器
//**********************************************************************

using System;
using System.Collections.Generic;
using GCSeries.Core;
using TeaProject.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace TeaProject.UI
{
    /// <summary>
    /// ZFrame切换控制器
    /// </summary>
    public class SideButtonController : MonoBehaviour
    {

        #region Public or protected fields and properties
        public static SideButtonController Instance;
        #endregion

        #region Private fields and properties
        //相机
        [SerializeField] private ZCameraRig m_zCameraRig;
        //ZFrame集合
        [SerializeField] private List<ZFrame> m_frames;
        //当前步骤
        private int m_currentProgress;
        #endregion

        #region Unity Callback
        private void Start()
        {
            Instance = this;
            InitOnClick();
        }
        #endregion

        #region Private method
        /// <summary>
        /// 点击各个button的点击事件
        /// </summary>
        private void InitOnClick()
        {
            var toggles = GetComponentsInChildren<Toggle>();
            foreach (var toggle in toggles)
            {
                toggle.onValueChanged.AddListener(isSelect =>
                {
                    if (!isSelect) return;
                    //获取当前点击的索引
                    var index = Array.IndexOf(toggles, toggle);
                    //切换当前场景，不进行操作
                    if (index == m_currentProgress)
                    {
                        return;
                    }
                    //初始化进度
                    var progress = m_frames[index].gameObject.GetComponent<Progress>();
                    if (progress != null)
                    {
                        progress.Init();
                        progress.Enter();
                    }
                    //切换视角
                    m_zCameraRig.Frame = m_frames[index];
                    //设置当前步骤
                    m_currentProgress = index;
                });
            }
        }
        #endregion

    }
}