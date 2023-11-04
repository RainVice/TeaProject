//**********************************************************************
// Script Name          : StepInfoData.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023年10月31日
// Last Modified Time   : 2023年10月31日
// Description          : 制作茶叶步骤信息管理
//**********************************************************************

using System.Collections.Generic;
using TeaProject.Manager;
using TeaProject.Utility;

namespace TeaProject.Data
{
    /// <summary>
    /// 制作茶叶步骤信息管理
    /// </summary>
    public class StepInfoData : IData
    {
        #region Private fields and properties
        private List<StepInfo> m_stepInfos;
        #endregion
        

        #region Public or protected method
        public void Init(string json)
        {
            m_stepInfos = TeaProjectUtility.Deserialize<List<StepInfo>>(json);
        }

        public T Get<T>(int index) where T : class
        {
            return m_stepInfos[index] as T;
        }
        #endregion

    }
}