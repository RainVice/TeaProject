//**********************************************************************
// Script Name          : UIView.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023年11月3日
// Last Modified Time   : 2023年11月3日
// Description          : 通用UI基类
//**********************************************************************

using System;
using UnityEngine;

namespace TeaProject.UI
{
    /// <summary>
    /// 通用UI基类
    /// </summary>
    public abstract class UIView : MonoBehaviour
    {
        #region Private fields and properties
        //绑定的键盘案件
        [SerializeField] private KeyCode m_keyCode;
        #endregion

        #region Unity Callback
        private void Awake()
        {
            if (m_keyCode != KeyCode.None)
            {
                KeyDownHandler.Instance.AddKeyDownAction(m_keyCode, KeyDownEvent);
            }
        }
        #endregion

        #region Public or protected method
        /// <summary>
        /// 按键按下时需要执行的事件
        /// </summary>
        protected abstract void KeyDownEvent();
        #endregion
    }
}