//**********************************************************************
// Script Name          : KeyDownHandler.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023年11月4日
// Last Modified Time   : 2023年11月4日
// Description          : 快捷建监听处理器
//**********************************************************************

using System;
using System.Collections.Generic;
using TeaProject.Utility;
using UnityEngine;

namespace TeaProject
{
    /// <summary>
    /// 快捷建监听处理器
    /// </summary>
    public class KeyDownHandler : MonoSingleton<KeyDownHandler>
    {
        #region Public or protected fields and properties

        #endregion

        #region Private fields and properties

        /// <summary>
        /// 保存键盘按键,对应相对事件
        /// </summary>
        private Dictionary<KeyCode, Action> m_actionDic = new Dictionary<KeyCode, Action>();

        #endregion

        #region Unity Callback

        private void Update()
        {
            //监听按键，执行对应委托
            foreach (var (key, value) in m_actionDic)
            {
                if (Input.GetKeyDown(key))
                {
                    value();
                }
            }
        }

        #endregion

        #region Public or protected method

        public void AddKeyDownAction(KeyCode keyCode, Action action)
        {
            //如果keyCode是空，则不添加
            if (keyCode == KeyCode.None)
            {
                return;
            }
            //如果有此快捷键，则报快捷键冲突
            if (m_actionDic.ContainsKey(keyCode))
            {
                Debug.LogError($"快捷键冲突：{keyCode}");
                throw new Exception("快捷键冲突");
            }

            m_actionDic.Add(keyCode, action);
        }

        #endregion
    }
}