//**********************************************************************
// Script Name          : Level.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023/10/27
// Last Modified Time   : 2023/10/27
// Description          : 保存场景信息，跳转伪加载
//**********************************************************************

using System;
using TeaProject.Data;
using TeaProject.Manager;
using TeaProject.UI;
using TeaProject.Utility;
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
        
        //事件委托
        public event Action<AsyncOperation> OnloadEvent;

        //当前场景索引
        public int Index;
        
        //当前场景名
        public string Name;

        //下一个场景索引
        public int NextIndex;
        
        private UILoadingPage m_uiLoadingPage;

        #endregion
        

        #region Public or protected method
        
        public string GetLevelName()
        {
            return Name;
        }

        public ILevel GetNextLevel()
        {
            return DataManager.Instance.GetData<LevelData>().Get<Level>(NextIndex);
        }

        public int GetLevelIndex()
        {
            return Index;
        }

        public void Begin()
        {
            TeaProjectUtility.CloseLoadingPage();
        }

        public void End()
        {
            TeaProjectUtility.ShowLoadingPage();
        }

        public void OnLoad(AsyncOperation operation)
        {
            operation.allowSceneActivation = false;
            if (OnloadEvent != null)
            {
                OnloadEvent(operation);
            }
        }

        #endregion
    }
}