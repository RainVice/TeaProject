//**********************************************************************
// Script Name          : TestData.cs
// Author Name          : 刘坤
// Create Time          : 创建时间，格式为 YY/MM/DD（必填）
// Last Modified Time   : 2023/10/25
// Description          : 测试文件，待删除
//**********************************************************************

using System;
using System.Collections.Generic;
using TeaProject.Manager;
using System.Text.Json;
using UnityEngine;

namespace TeaProject
{
    public class IntString
    {
        public int Number;
        public string Description { get; set; }
    }

    public class TestData : IData
    {
        private List<Level> m_levels;

        public T Get<T>(int index) where T : class
        {
            if (typeof(T) == typeof(Level))
            {
                return m_levels[index] as T;
            }
            throw new AggregateException($"参数类型应兼容 {typeof(Level).FullName}");
        }

        public void Init(string json)
        {
            Debug.Log(json);
            List<Level> deserialize = JsonSerializer.Deserialize<List<Level>>(json);
            m_levels = deserialize;
            foreach (Level level in m_levels)
            {
                Debug.Log(level.GetLevelIndex());
            }
        }
    }
}