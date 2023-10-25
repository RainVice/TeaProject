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
        public bool GetBool(int index)
        {
            throw new System.NotImplementedException();
        }

        public double GetDouble(int index)
        {
            throw new System.NotImplementedException();
        }

        public float GetFloat(int index)
        {
            throw new System.NotImplementedException();
        }

        public int GetInt32(int index)
        {
            throw new System.NotImplementedException();
        }

        public long GetInt64(int index)
        {
            throw new System.NotImplementedException();
        }

        public string GetString(int index)
        {
            throw new System.NotImplementedException();
        }

        public void Init(Stream json)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<IntString>));
            List<IntString> list = serializer.ReadObject(json) as List<IntString>;
        }
    }
}