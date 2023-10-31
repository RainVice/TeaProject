//**********************************************************************
// Script Name          : EffectManager.cs
// Author Name          : 刘坤
// Create Time          : 2023/10/30
// Last Modified Time   : 2023/10/30
// Description          : 编写特效管理器类
//**********************************************************************

using System.Collections;
using System.Collections.Generic;
using TeaProject.Effect;
using UnityEngine;

namespace TeaProject.Manager
{
    /// <summary>
    /// 特效管理器
    /// </summary>
    public class EffectManager : BaseManager<EffectManager>
    {

    #region Public or protected fields and properties
        public override IEnumerator Init(object arg = null)
        {
            yield return base.Init(arg);
        }
    #endregion

    #region Private fields and properties
        private HashSet<EffectSet> m_ActiveEffects = new HashSet<EffectSet>();
        private List<EffectSet> m_DeleteList = new List<EffectSet>();
    #endregion

    #region Unity Callback
        private void LateUpdate()
        {
            foreach(EffectSet effectSet in m_ActiveEffects)
            {
                effectSet.Execute();
                if(effectSet.IsDone)
                    m_DeleteList.Add(effectSet);
            }
            foreach (EffectSet delete in m_DeleteList)
            {
                if(m_ActiveEffects.Contains(delete))
                    m_ActiveEffects.Remove(delete);
            }
            m_DeleteList.Clear();
        }
    #endregion

    #region Public or protected method
        /// <summary>
        /// 开始执行一个特效集
        /// </summary>
        /// <param name="effectSet">要执行的特效集</param>
        public void ExecuteEffect(EffectSet effectSet)
        {
            m_ActiveEffects.Add(effectSet);
        }

        public void StopEffectSet(EffectSet effectSet)
        {
            m_DeleteList.Add(effectSet);
        }
    #endregion

    }
}