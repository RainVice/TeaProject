//**********************************************************************
// Script Name          : UISideItem.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023年11月1日
// Last Modified Time   : 2023年11月1日
// Description          : 切换按钮的控制脚本
//**********************************************************************

using TeaProject.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TeaProject.UI
{
    /// <summary>
    /// 切换按钮的控制脚本
    /// </summary>
    public class UISideItem : UIView,IPointerEnterHandler,IPointerExitHandler
    {
        #region Private fields and properties
        //Toggle
        [SerializeField] private Toggle m_Toggle;
        //提示文本
        [SerializeField] private Text m_text;
        #endregion

        #region Unity Callback
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            m_text.ChangeColor(Color.gray,0.1f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_text.ChangeColor(Color.clear, 0.1f);
        }
        #endregion

        protected override void KeyDownEvent()
        {
            m_Toggle.isOn = false;
            m_Toggle.isOn = true;
        }
    }
}