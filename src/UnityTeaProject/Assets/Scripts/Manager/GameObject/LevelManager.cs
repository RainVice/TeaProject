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
    public class LevelManager : BaseManager<LevelManager>
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
    #endregion

    #region Private fields and properties
        private Stack<ILevel> m_Levels = new Stack<ILevel>();
    #endregion

    #region Public or protected method
        /// <summary>
        /// 初始化关卡管理器
        /// </summary>
        /// <param name="startLevel">游戏开始时的场景</param>
        public override void Init(System.Object startLevel)
        {
            base.Init();
            ILevel level = startLevel as ILevel;
            if(level == null)
            {
                Debug.LogError("使用了错误的参数来初始化关卡管理器");
                throw new ArgumentException(); 
            } 
            m_Levels.Push(level);
            IsReady = true;
        }
        
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

        /// <summary>
        /// 回到指定的关卡
        /// </summary>
        /// <param name="level"></param>
        /// <returns>如果给定的场景在关卡栈中，则会回到指定的关卡并返回 true；如果不在关卡栈中，则不会加载关卡并返回 false</returns>
        public bool PopTo(ILevel level)
        {
            Stack<ILevel> st = new Stack<ILevel>();
            while(m_Levels.Count > 0 && !m_Levels.Peek().Equals(level))
            {
                st.Push(m_Levels.Pop());
            }
            
            if(m_Levels.Count == 0)
            {
                while(st.Count > 0)
                {
                    m_Levels.Push(st.Pop());
                }
                return false;
            }
            else
            {
                StartCoroutine(LoadLevelAsync(m_Levels.Peek()));
                return true;
            }
        }
    #endregion

    #region Private method
        private IEnumerator LoadLevelAsync(ILevel level)
        {
            int index = level.GetLevelIndex();
            if(index < 0 || index >= SceneManager.sceneCount)
            {
                Debug.LogError("要加载的场景索引不存在！");
                throw new ArgumentException("关卡的索引不存在");
            }
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);
            while (!asyncLoad.isDone)
            {
                level.OnLoad(asyncLoad);
                yield return null;
            }
            level.OnLoad(asyncLoad);
        }
    #endregion

    }
}