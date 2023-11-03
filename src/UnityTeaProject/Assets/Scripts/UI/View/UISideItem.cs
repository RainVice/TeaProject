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
    /// 请修改类描述。
    /// </summary>
    public class UISideItem : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        #region Private fields and properties
        //提示文本
        [SerializeField] private Text m_text;
        //携程
        private Coroutine m_coroutine;
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
    }
}