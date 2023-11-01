//**********************************************************************
// Script Name          : TeaIntroduce.cs
// Author Name          : 欧阳晨昊
// Create Time          : #CREATETIME#
// Last Modified Time   : YY/MM/DD
// Description          : 文件描述（可选）
//**********************************************************************

using System;
using Codice.CM.Client.Differences.Graphic;
using TeaProject.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TeaProject
{
    /// <summary>
    /// 请修改类描述。
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

        #region Public or protected method
                
        #endregion

        #region Private method
            
        #endregion

    }
}