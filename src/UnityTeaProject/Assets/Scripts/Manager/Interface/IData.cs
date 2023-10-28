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
        void Init(List<System.Object> args); 
        /// <summary>
        /// 获取指定类型的数据
        /// </summary>
        /// <param name="index">数据的索引</param>
        /// <typeparam name="T">数据的类型</typeparam>
        /// <returns></returns>
        T Get<T>(int index) where T : class;
    }
}