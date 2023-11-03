//**********************************************************************
// Script Name          : EffectCenter.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023年11月2日
// Last Modified Time   : 2023年11月2日
// Description          : UI动效运行中心
//**********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using TeaProject.Utility;

namespace TeaProject
{
    /// <summary>
    /// UI动效运行中心
    /// </summary>
    public class EffectCenter : MonoSingleton<EffectCenter>
    {
        #region private fields and properties
        /// <summary>
        /// 保存Func委托的字典，返回值是bool，返回true表示委托已完成，则会清楚掉这个委托
        /// </summary>
        private Dictionary<object, Func<bool>> m_funcDic = new Dictionary<object, Func<bool>>();
        #endregion

        #region Unity Callback

        private void Update()
        {
            //如果没有动画委托，则直接返回
            if (m_funcDic.Count <= 0) return;

            for (int i = 0; i < m_funcDic.Keys.Count; i++)
            {
                if (!m_funcDic[m_funcDic.Keys.ElementAt(i)]()) continue;
                m_funcDic.Remove(m_funcDic.Keys.ElementAt(i));
            }
            
            for (int i = 0; i < m_funcDic.Count; i++)
            {
                if (!m_funcDic[m_funcDic.Keys.ElementAt(i)]()) continue;
                m_funcDic.Remove(m_funcDic.Keys.ElementAt(i));
                i--;
            }
        }

        #endregion

        #region Public or protected method

        /// <summary>
        /// 其中action返回true表示动画已完成，则会清除掉这个委托，此返回值需要自行在逻辑中实现，且必须是在动画执行完后返回
        /// </summary>
        /// <param name="obj">动画物体的唯一标识</param>
        /// <param name="action">动画的运行逻辑</param>
        public void AddAction(object obj, Func<bool> action)
        {
            //如果之前有相同的动画委托，则先清除掉这个动画委托
            if (m_funcDic.ContainsKey(obj))
            {
                m_funcDic.Remove(obj);
            }
            //添加动画委托
            m_funcDic.Add(obj, action);
        }

        #endregion
    }
}