//**********************************************************************
// Script Name          : TestData.cs
// Author Name          : 刘坤
// Create Time          : 创建时间，格式为 YY/MM/DD（必填）
// Last Modified Time   : 2023/10/25
// Description          : 测试文件，待删除
//**********************************************************************

using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using TeaProject.Manager;
using UnityEngine;
using System.IO;

namespace TeaProject
{
    public class IntString
    {
        public int Number;
        public string Description { get; set; }
    }
    public class TestData : IData
    {
        public T Get<T>(int index)
        {
            throw new System.NotImplementedException();
        }

        public void Init(string json)
        {

        }
    }
}