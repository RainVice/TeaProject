//**********************************************************************
// Script Name          : MainManager.cs
// Author Name          : 刘坤
// Create Time          : 2023/10/23
// Last Modified Time   : 2023/10/23
// Description          : 编写主管理器类
//**********************************************************************

using UnityEngine;

namespace TeaProject.Manager
{
    /// <summary>
    /// 主管理器，此管理器用于控制其他管理器
    /// </summary>
    public class MainManager : BaseManager<MainManager>
    {

    #region Unity Callback
        private void Start()
        {
            //UIManager.Instance.Init();
            //LevelManager.Instance.Init();
        }
    #endregion

    }
}