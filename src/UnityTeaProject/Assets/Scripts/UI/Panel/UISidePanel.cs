//**********************************************************************
// Script Name          : UILeftPanel.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023年10月28日
// Last Modified Time   : 2023年10月28日
// Description          : 文件描述（可选）
//**********************************************************************

using System;
using TeaProject.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace TeaProject.UI
{
    /// <summary>
    /// 请修改类描述。
    /// </summary>
    public class UISidePanel : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {

        #region Public or protected fields and properties

        public bool IsLift
        {
            set => m_isLeft = value;
        }
        #endregion

        #region Private fields and properties
        /// <summary>
        /// 是否左边面板，true表示左边面板，从左边滑出，否则从右边
        /// </summary>
        [FormerlySerializedAs("isLeft")] [SerializeField]
        private bool m_isLeft = true;
        
        // 初始位置
        private float m_posX;
        
        //RectTransform
        private RectTransform m_rectTransform;
        private Coroutine m_coroutine;

        #endregion

        #region Unity Callback
        private void Start()
        {
            m_rectTransform = GetComponent<RectTransform>();
            m_rectTransform.pivot = m_isLeft ? new Vector2(0, 0.5f) : new Vector2(1, 0.5f);
            m_rectTransform.anchoredPosition += new Vector2((m_isLeft ? -1 : 1) * m_rectTransform.rect.width / 2, 0);
            m_posX = m_rectTransform.anchoredPosition.x;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // todo 面板移动
            if (m_coroutine != null)
            {
                StopCoroutine(m_coroutine);
            }
            m_coroutine = StartCoroutine(m_rectTransform.MovePosX(m_isLeft ? 0 : 1920f, 0.1f));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (m_coroutine != null)
            {
                StopCoroutine(m_coroutine);
            }
            m_coroutine = StartCoroutine(MotionUtility.MovePosX(m_rectTransform,m_posX,0.1f));
        }
        #endregion

        #region Public or protected method
        
        #endregion

        #region Private method

        #endregion

    }
}