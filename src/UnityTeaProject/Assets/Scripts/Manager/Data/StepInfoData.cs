//**********************************************************************
// Script Name          : StepInfoData.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023年10月31日
// Last Modified Time   : 2023年10月31日
// Description          : 文件描述（可选）
//**********************************************************************

using System.Collections.Generic;
using TeaProject.Manager;
using TeaProject.Utility;

namespace TeaProject.Data
{
    /// <summary>
    /// 请修改类描述。
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