//**********************************************************************
// Script Name          : Animation.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023年10月29日
// Last Modified Time   : 2023年10月29日
// Description          : 动画控制工具类，可在其中添加动画效果，如：移动，旋转，缩放等。
//                        具体是通过 EffectCenter.Instance.AddAction 函数来实现。
//                        其中传入的obj是需要控制的对象，传入的action是动画的委托，
//                        委托需要返回值，其要求是在动画完成时是true。具体参考如下代码。
//**********************************************************************

using System;
using UnityEngine;
using UnityEngine.UI;

namespace TeaProject.Utility
{
    /// <summary>
    /// 动画控制工具类
    /// </summary>
    public static class MotionUtility
    {
        #region Public or protected method

        /// <summary>
        /// 改变UI的PosX的值，向目标点移动
        /// </summary>
        /// <param name="rectTransform">需要移动的RectTransform</param>
        /// <param name="x">目标X的位置</param>
        /// <param name="f">需要的时间</param>
        /// <returns></returns>
        public static void MovePosX(this RectTransform rectTransform, float x, float f)
        {
            EffectHandler.Instance.AddAction(rectTransform, () =>
            {
                var anchoredPosition = rectTransform.anchoredPosition;
                rectTransform.anchoredPosition = Vector2.Lerp(anchoredPosition,
                    new Vector2(x, anchoredPosition.y), Time.deltaTime / f);
                return IsFinish(rectTransform.anchoredPosition.x, x);
            });
        }
        
        /// <summary>
        /// 改变UI的PosY的值，向目标点移动
        /// </summary>
        /// <param name="rectTransform">需要移动的RectTransform</param>
        /// <param name="y">目标Y的位置</param>
        /// <param name="f">需要的时间</param>
        /// <returns></returns>
        public static void MovePosY(this RectTransform rectTransform, float y, float f)
        {
            EffectHandler.Instance.AddAction(rectTransform, () =>
            {
                var anchoredPosition = rectTransform.anchoredPosition;
                rectTransform.anchoredPosition = Vector2.Lerp(anchoredPosition,
                    new Vector2(anchoredPosition.x, y), Time.deltaTime / f);
                return IsFinish(rectTransform.anchoredPosition.y, y);
            });
        }

        /// <summary>
        /// 改变Text的文本颜色
        /// </summary>
        /// <param name="text">需要改变颜色的Text</param>
        /// <param name="toColor">目标颜色</param>
        /// <param name="f">需要的时间</param>
        public static void ChangeColor(this Text text, Color toColor, float f)
        {
            EffectHandler.Instance.AddAction(text, () =>
            {
                text.color = Color.Lerp(text.color, toColor, Time.deltaTime / f);
                return IsFinish(text.color.r, toColor.r) &&
                       IsFinish(text.color.g, toColor.g) &&
                       IsFinish(text.color.b, toColor.b) &&
                       IsFinish(text.color.a, toColor.a);
            });
        }

        #endregion

        #region Private method
        /// <summary>
        /// 判断两个数是否接近，误差为0.01
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        private static bool IsFinish(float f1,float f2)
        {
            return Math.Abs(f1 - f2) < 0.01f;
        }
        #endregion
    }
}