//**********************************************************************
// Script Name          : RTSlideEffect.cs
// Author Name          : 刘坤
// Create Time          : 2023/11/01
// Last Modified Time   : 2023/11/01
// Description          : 编写UI滑动特效
//**********************************************************************

using UnityEngine;

namespace TeaProject.Effect
{
    /// <summary>
    /// UI滑动特效
    /// </summary>
    public class RTSlideEffect : BaseRectTransformEffect
    {

    #region Private fields and properties
        Vector2 m_StartPosition;
        Vector2 m_EndPosition;
        float m_StartTime;
        float m_Duration;
    #endregion

    #region Public or protected method
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="transform">要使用的RectTransform</param>
        /// <param name="move">要移动的距离</param>
        /// <param name="duration">持续时间</param>
        public RTSlideEffect(RectTransform transform, Vector2 move, float duration) : base(transform)
        { 
            m_StartPosition = transform.anchoredPosition;
            m_EndPosition = transform.anchoredPosition + move;
            m_Duration = duration;
            m_StartTime = Time.time;
        }
        public override void Do()
        {
            float t = (Time.time - m_StartTime) / m_Duration;
            if(t >= 1)
                m_IsDone = true;
            Transform.anchoredPosition = Vector3.Lerp(m_StartPosition, m_EndPosition, t);
        }
    #endregion

    }
}