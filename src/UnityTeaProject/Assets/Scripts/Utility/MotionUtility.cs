//**********************************************************************
// Script Name          : Animation.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023年10月29日
// Last Modified Time   : 2023年10月29日
// Description          : 文件描述（可选）
//**********************************************************************

using System.Collections;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;

namespace TeaProject.Utility
{
    /// <summary>
    /// 请修改类描述。
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
        public static IEnumerator MovePosX(this RectTransform  rectTransform, float x, float f)
        {
            while (!Mathf.Approximately(rectTransform.anchoredPosition.x,x))
            {
                var anchoredPosition = rectTransform.anchoredPosition;
                rectTransform.anchoredPosition = Vector2.Lerp(anchoredPosition,
                    new Vector2(x, anchoredPosition.y),Time.deltaTime/f);
                yield return null;
            }
        }

        public static IEnumerator ChangeColor(this Text text, Color toColor, float f)
        {
            while (!(Mathf.Approximately(text.color.r, toColor.r) &&
                    Mathf.Approximately(text.color.g, toColor.g) &&
                    Mathf.Approximately(text.color.b, toColor.b) &&
                    Mathf.Approximately(text.color.a, toColor.a)))
            {
                text.color = Color.Lerp(text.color, toColor, Time.deltaTime / f);
                yield return null;
            }
            yield return null;
        }
        #endregion

        #region Private method
        #endregion
    }
}