//**********************************************************************
// Script Name          : UILeftPanel.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023年10月28日
// Last Modified Time   : 2023年10月28日
// Description          : 侧边面板
//**********************************************************************

using TeaProject.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TeaProject.UI
{
    /// <summary>
    /// 侧边面板
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
        [SerializeField]
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
            m_rectTransform.MovePosX(m_isLeft ? 0 : 1920f, 0.1f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_rectTransform.MovePosX(m_posX, 0.1f);
        }
        #endregion

    }
}