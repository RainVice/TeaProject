//**********************************************************************
// Script Name          : MainManager.cs
// Author Name          : 刘坤
// Create Time          : 2023/10/23
// Last Modified Time   : 2023/10/25
// Description          : 编写主管理器类
//**********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using TeaProject.UI;
using UnityEngine;

namespace TeaProject.Manager
{
    /// <summary>
    /// 主管理器，此管理器用于控制其他管理器
    /// </summary>
    public class MainManager : BaseManager<MainManager>
    {

    #region Public or protected fields and properties
        /// <summary>
        /// 此值指示主管理器是否完成初始化
        /// </summary>
        /// <value>如果为 true，则已经完成初始化；如果为 false，则还未完成初始化</value>
        public new bool IsReady
        {
            get
            {
                return base.IsReady;
            }
        }
    #endregion

    #region Unity Callback
        private IEnumerator Start()
        {
            base.Init();
            base.IsReady = false;
            yield return null;
            // ***********************************
            // 此处初始化所有的管理器
            yield return DataManager.Instance.Init(new List<Tuple<Type, string>>
            {

            });
            yield return UIManager.Instance.Init(new List<Tuple<Type, string>>
            {
                new Tuple<Type, string>(typeof(UILoadView),"Prefabs/UI/panleLoading")
            });
            //yield return LevelManager.Instance.Init();
            // ***********************************
            base.IsReady = true;
            yield return null;
        }
    #endregion

    }
}