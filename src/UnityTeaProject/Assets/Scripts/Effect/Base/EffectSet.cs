//**********************************************************************
// Script Name          : EffectSet.cs
// Author Name          : 刘坤
// Create Time          : 2023/10/30
// Last Modified Time   : 2023/10/30
// Description          : 编写特效集类
//**********************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeaProject.Effect
{
    /// <summary>
    /// 特效集
    /// </summary>
    public class EffectSet
    {

    #region Private fields and properties
        Dictionary<EffectType, BaseEffect> m_EffectDictionary = new Dictionary<EffectType, BaseEffect>();
    #endregion

    #region Public or protected method
        /// <summary>
        /// 添加一个特效到此特效集中
        /// </summary>
        /// <param name="effect">要添加的特效</param>
        public void AddEffect(BaseEffect effect)
        {
            if(m_EffectDictionary.ContainsKey(effect.EffectType))
            {
                Debug.LogWarning($"尝试添加特效到特效集中，然而特效集中已有[{effect.EffectType}]类型的特效，新特效将覆盖原有特效。");
                m_EffectDictionary[effect.EffectType] = effect;
            }
            else
            {
                m_EffectDictionary.Add(effect.EffectType, effect);
            }
        }
        /// <summary>
        /// 执行此特效集
        /// </summary>
        public void Execute()
        {
            foreach (var kvp in m_EffectDictionary)
            {
                kvp.Value.Do();
            }
        }
    #endregion

    }
}