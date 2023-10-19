//**********************************************************************
// Script Name          : LevelManager.cs
// Author Name          : 刘坤
// Create Time          : 2023/10/19
// Last Modified Time   : 2023/10/19
// Description          : 编写关卡管理器类
//**********************************************************************

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace TeaProject.Manager
{
    /// <summary>
    /// 关卡管理器
    /// </summary>
    public class LevelManager : BaseManager
    {

    #region Public or protected fields and properties
        /// <summary>
        /// 当前正在活动中的关卡
        /// </summary>
        public ILevel CurrentLevel
        {
            get
            {
                return m_Levels.Peek();
            }
        }
        /// <summary>
        /// 初始化关卡管理器
        /// </summary>
        /// <param name="startLevel">游戏开始时的场景</param>
        public override void Init(System.Object startLevel)
        {
            if(IsReady)
            {
                Debug.LogWarning("尝试初始化关卡管理器，而场景管理器已经完成初始化");
                return;
            }
            ILevel level = startLevel as ILevel;
            if(level == null)
            {
                Debug.LogError("使用了错误的参数来初始化关卡管理器");
                throw new ArgumentException(); 
            } 
            m_Levels.Push(level);
            IsReady = true;
        }
    #endregion

    #region Private fields and properties
        private Stack<ILevel> m_Levels = new Stack<ILevel>();
    #endregion

    #region Public or protected method
        /// <summary>
        /// 加载当前关卡的下一个关卡
        /// </summary>
        public void LoadNextLevel()
        {
            ILevel current = m_Levels.Peek();
            ILevel next = current.GetNextLevel();
            if(next == null)
            {
                Debug.LogWarning("当前活动关卡无后继场景！");
                return;
            }
            StartCoroutine(LoadLevelAsync(next));
            m_Levels.Push(next);
            current.End();
            next.Begin();
        }

        /// <summary>
        /// 加载一个新关卡
        /// </summary>
        /// <param name="level">要加载的关卡</param>
        public void PushLevel(ILevel level)
        {
            if(level == null)
            {
                Debug.LogError("要加载的关卡为空！");
                throw new ArgumentNullException();
            }
            StartCoroutine(LoadLevelAsync(level));
            m_Levels.Peek().End();
            m_Levels.Push(level);
            level.Begin();
        }

        /// <summary>
        /// 回到上一个关卡
        /// </summary>
        public void PopLevel()
        {
            if(m_Levels.Count == 1)
            {
                Debug.LogError("当前关卡是关卡管理器中最后一个场景，无法弹出！");
                return;
            }
            ILevel popLevel = m_Levels.Pop();
            ILevel current = m_Levels.Peek();
            StartCoroutine(LoadLevelAsync(current));
            popLevel.End();
            current.Begin();
        }
    #endregion

    #region Private method
        private IEnumerator LoadLevelAsync(ILevel level)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(level.GetLevelIndex());
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    #endregion

    }
}