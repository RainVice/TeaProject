//**********************************************************************
// Script Name          : TeaIntroduce.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023/11/01
// Last Modified Time   : 2023/11/01
// Description          : 科普板块的介绍逻辑
//**********************************************************************

using TeaProject.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TeaProject
{
    /// <summary>
    /// 科普板块的介绍逻辑
    /// </summary>
    public class TeaIntroduce : MonoBehaviour
    {

        #region Public or protected fields and properties
            
        #endregion

        #region Private fields and properties

        //数据索引
        [SerializeField]
        private int m_index;
        
        //显示框文本
        [SerializeField]
        private Text m_Msg;

        //是否茶叶介绍信息
        [SerializeField] 
        private bool m_isInfo = true;

        private GameObject m_Panel;
        #endregion

        #region Unity Callback

        private void Start()
        {
            m_Panel = m_Msg.transform.parent.gameObject;
        }

        private void OnMouseEnter()
        {
            if (m_isInfo)
            {
                TeaInfo teaInfo = DataManager.Instance.GetData<TeaInfoData>().Get<TeaInfo>(m_index);
                m_Msg.text = $"{teaInfo.TeaName}：{teaInfo.TeaIntroduce}";
            }
            else
            {
                StepInfo stepInfo = DataManager.Instance.GetData<StepInfoData>().Get<StepInfo>(m_index);
                m_Msg.text = $"{stepInfo.StepName}：{stepInfo.Description}";
            }
            m_Panel.SetActive(true);
        }

        private void OnMouseExit()
        {
            m_Panel.gameObject.SetActive(false);
        }

        #endregion

    }
}