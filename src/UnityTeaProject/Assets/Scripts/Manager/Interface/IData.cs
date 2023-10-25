//**********************************************************************
// Script Name          : IData.cs（必填）
// Author Name          : 作者名（必填）
// Create Time          : 创建时间，格式为 YY/MM/DD（必填）
// Last Modified Time   : 最后一次修改时间，格式为 YY/MM/DD（必填）
// Description          : 文件描述（可选）
//**********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TeaProject.Manager
{
    /// <summary>
    /// 数据类接口
    /// </summary>
    public interface IData
    {
        /// <summary>
        /// 初始化数据类，此方法在实现时不应该依赖任何实例对象数据。
        /// </summary>
        /// <param name="json">序列化的Json文本流</param>
        void Init(Stream json); 
        /// <summary>
        /// 获取字符串型数据
        /// </summary>
        string GetString(int index);
        /// <summary>
        /// 获取32位整型数据
        /// </summary>
        int GetInt32(int index);
        /// <summary>
        /// 获取64位长整型数据
        /// </summary>
        long GetInt64(int index);
        /// <summary>
        /// 获取16位浮点型数据
        /// </summary>
        float GetFloat(int index);
        /// <summary>
        /// 获取32位浮点型数据
        /// </summary>
        double GetDouble(int index);
        /// <summary>
        /// 获取布尔型数据
        /// </summary>
        bool GetBool(int index);
    }
}