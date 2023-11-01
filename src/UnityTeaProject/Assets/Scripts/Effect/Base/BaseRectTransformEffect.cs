//**********************************************************************
// Script Name          : BaseVector3Effect.cs
// Author Name          : 刘坤
// Create Time          : 2023/11/01
// Last Modified Time   : 2023/11/01
// Description          : 编写RectTransform特效的基类
//**********************************************************************

using UnityEngine;

namespace TeaProject.Effect
{
    /// <summary>
    /// RectTransform特效的基类
    /// </summary>
    public abstract class BaseRectTransformEffect : BaseEffect
    {

    #region Public or protected fields and properties
        /// <summary>
        /// 此特效使用的RectTransform
        /// </summary>
        protected RectTransform Transform
        {
            get
            {
                return m_transform;
            }
        }
    #endregion

    #region Private fields and properties
        private RectTransform m_transform;
    #endregion

    #region Public or protected method
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="transform">特效要使用的RectTransform</param>
        public BaseRectTransformEffect(RectTransform transform)
        {
            m_transform = transform;
            m_EffectType |= EffectType.RectTransform;
        }
    #endregion

    }
}