//**********************************************************************
// Script Name          : UIPanel.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023年10月28日
// Last Modified Time   : 2023年10月28日
// Description          : UI面板基类
//**********************************************************************

using System;
using UnityEngine;

namespace TeaProject.UI
{
    /// <summary>
    /// UI面板基类
    /// </summary>
    public class UIPanel : MonoBehaviour
    {
        #region Public or protected fields and properties

        #endregion

        #region Private fields and properties

        #endregion

        #region Unity Callback

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // todo 显示设置面板
            }
        }

        #endregion

        #region Public or protected method

        #endregion

        #region Private method

        #endregion
    }
}