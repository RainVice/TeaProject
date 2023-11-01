//**********************************************************************
// Script Name          : TeaInfoData.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023年10月31日
// Last Modified Time   : 2023年10月31日
// Description          : 文件描述（可选）
//**********************************************************************

using System.Collections.Generic;
using System.IO;
using TeaProject.Manager;
using TeaProject.Utility;
using UnityEngine;

namespace TeaProject.Data
{
    /// <summary>
    /// 请修改类描述。
    /// </summary>
    public class TeaInfoData : IData
    {
        #region Private fields and properties

        private List<TeaInfo> m_teaInfos;
        #endregion

        #region Public or protected method
        public void Init(string json)
        {
            m_teaInfos = TeaProjectUtility.Deserialize<List<TeaInfo>>(json);
        }

        /// <summary>
        /// 获取茶叶信息
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public T Get<T>(int index) where T : class
        {
            return m_teaInfos[index] as T;
        }
        #endregion


    }
}