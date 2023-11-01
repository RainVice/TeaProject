//**********************************************************************
// Script Name          : BaseEffect.cs
// Author Name          : 刘坤
// Create Time          : 2023/10/30
// Last Modified Time   : 2023/10/30
// Description          : 编写特效基类
//**********************************************************************

using UnityEngine;

namespace TeaProject.Effect
{
    /// <summary>
    /// 特效基类
    /// </summary>
    public abstract class BaseEffect
    {

    #region Public or protected fields and properties
        /// <summary>
        /// 指示特效是否完成的内部变量，派生类中可以修改此值
        /// </summary>
        protected bool m_IsDone; 

        /// <summary>
        /// 指示此特效是否已经完成
        /// </summary>
        /// <value>如果为 true，则此特效被视为完成；如果为 false，则此特效被视为未完成</value>
        public bool IsDone
        {
            get
            {
                return m_IsDone;
            }
        }

        /// <summary>
        /// 指示特效的类型的内部变量，派生类中可以修改此值
        /// </summary>
        protected EffectType m_EffectType;
        /// <summary>
        /// 指示此特效的类型
        /// </summary>
        public EffectType EffectType
        {
            get
            {
                return m_EffectType;
            }
        }
    #endregion

    #region Public or protected method
        /// <summary>
        /// 特效的每帧执行的逻辑
        /// </summary>
        public abstract void Do();
    #endregion

    }
}