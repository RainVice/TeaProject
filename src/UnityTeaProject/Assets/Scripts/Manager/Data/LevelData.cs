//**********************************************************************
// Script Name          : LevelData.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023年10月28日
// Last Modified Time   : 2023年10月28日
// Description          : 保存Level各个场景
//**********************************************************************

using System.Collections.Generic;
using TeaProject.Manager;
using TeaProject.Utility;

namespace TeaProject.Data
{
    /// <summary>
    /// 保存Level各个场景
    /// </summary>
    public class LevelData : IData
    {

        #region Private fields and properties

        //保存Level集合
        private List<Level> m_level;

        #endregion
        
        
        #region Public or protected method
        public void Init(string json)
        {
            m_level = TeaProjectUtility.Deserialize<List<Level>>(json);
        }
        
        public T Get<T>(int index) where T : class
        {
            return m_level[index] as T;
        }


        /// <summary>
        /// 获取Level列表
        /// </summary>
        /// <returns></returns>
        public List<Level> GetList()
        {
            return m_level;
        }
        #endregion
    }
}