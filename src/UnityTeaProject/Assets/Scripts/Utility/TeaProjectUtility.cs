//**********************************************************************
// Script Name          : TeaProjectUtility.cs
// Author Name          : 刘坤
// Create Time          : 2023/10/23
// Last Modified Time   : 2023/10/23
// Description          : 编写项目工具类
//**********************************************************************

using UnityEngine;
using System;
using System.Text.Json;

namespace TeaProject.Utility
{
    /// <summary>
    /// 项目工具类
    /// </summary>
    public static class TeaProjectUtility
    {

    #region Public or protected fields and properties
        /// <summary>
        /// 反序列化设置，此值决定了 TeaProjectUtility.Deserialize 会如何反序列化
        /// </summary>
        public static JsonSerializerOptions DeserializerOptions
        {
            get
            {
                return m_DeserializerOptions;
            }
            set
            {
                m_DeserializerOptions = value;
            }
        }
    #endregion

    #region Private fields and properties
        private static JsonSerializerOptions m_DeserializerOptions = new JsonSerializerOptions { IncludeFields = true };
    #endregion

    #region Public or protected method
        /// <summary>
        /// 将Json字符串反序列化为指定的对象
        /// </summary>
        /// <param name="json">要反序列的Json字符串</param>
        /// <typeparam name="T">要反序列化为的对象类型</typeparam>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            try
            {
                T res = JsonSerializer.Deserialize<T>(json, m_DeserializerOptions);
                return res;
            }
            catch (Exception ex)
            {
                Debug.LogError($"反序列化Json字符串时出现错误！");
                throw ex;
            }
        }
    #endregion

    }
}