using System.IO;
//**********************************************************************
// Script Name          : DataManager.cs
// Author Name          : 刘坤
// Create Time          : 2023/10/25
// Last Modified Time   : 2023/10/25
// Description          : 编写数据管理器类
//**********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using TeaProject.Manager;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

namespace TeaProject
{
    /// <summary>
    /// 数据管理器
    /// </summary>
    public class DataManager : BaseManager<DataManager>
    {

    #region Public or protected fields and properties        
        /// <summary>
        /// 解密器对象
        /// </summary>
        /// <value>如果此值不为空，则数据管理器在读取文件时会使用此解码器来解析文件流；如果此值为空，则默认文件未加密</value>
        public Func<byte[], byte[]> Decrypt
        {
            get
            {
                return m_Decrypt;
            }
            set
            {
                m_Decrypt = value;
            }
        }
        /// <summary>
        /// 指示数据管理器编码格式
        /// </summary>
        /// <value>如果此值不为空，则数据管理器会使用此编码格式来读取文件；如果此值为空，则默认使用 UTF-8 编码格式</value>
        public Encoding DataEncoding
        {
            get
            {
                return m_DataEncoding;
            }
            set
            {
                m_DataEncoding = value;
            }
        }
    #endregion

    #region Private fields and properties
        private Func<byte[], byte[]> m_Decrypt;
        private Encoding m_DataEncoding;
        private Dictionary<Type, IData> m_DataDictionary = new Dictionary<Type, IData>();
    #endregion

    #region Public or protected method
        /// <summary>
        /// 初始化数据管理器，此方法接受一个 IList(Tuple(Type), string)) 类型数据
        /// </summary>
        /// <param name="obj">一个包含反序列化类型和配置文件路径的IList对象</param>
        public override IEnumerator Init(System.Object obj)
        {
            base.Init();
            IList<Tuple<Type, string>> args = obj as IList<Tuple<Type, string>>;
            if(args == null)
            {
                Debug.LogError("使用了错误的参数来初始化数据管理器");
                throw new ArgumentException("参数类型应兼容 System.Collections.Generic.IList<Tuple<Type, string>> ");
            }
            foreach (var kvp in args)
            {
                Type t = kvp.Item1;
                if(!typeof(IData).IsAssignableFrom(t))
                {
                    Debug.LogError("使用了错误的参数来初始化数据管理器");
                    throw new ArgumentException($"类型 {t.FullName} 不实现接口 {typeof(IData).FullName}");
                }
                if(m_DataDictionary.ContainsKey(t))
                {
                    Debug.LogError("使用了错误的参数来初始化数据管理器");
                    throw new ArgumentException($"类型 {t.FullName} 出现了多次！应只出现一次");
                }
                IData da = t.Assembly.CreateInstance(t.FullName) as IData;
                yield return ReadJson(kvp.Item2, da);
                m_DataDictionary.Add(kvp.Item1, da);
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T">要获取的数据类型</typeparam>
        /// <returns>如果存在指定的类型，则返回对象；如果不存在指定的类型，则返回空</returns>
        public IData GetData<T>() where T : IData
        {
            Type t = typeof(T);
            if(m_DataDictionary.ContainsKey(t))
                return m_DataDictionary[t];
            else
                return null;
        }
    #endregion

    #region Private method
        IEnumerator ReadJson(string path, IData data)
        {
            path = Application.streamingAssetsPath + "/" + path;
            using (UnityWebRequest request = UnityWebRequest.Get(path))
            {
                yield return request.SendWebRequest();
                if(request.isDone && request.result == UnityWebRequest.Result.Success)
                {
                    string jsonStr;
                    byte[] buffer = request.downloadHandler.data;
                    if(m_Decrypt != null) buffer = m_Decrypt(buffer);
                    if(m_DataEncoding == null)
                        jsonStr = Encoding.UTF8.GetString(buffer);
                    else
                        jsonStr = m_DataEncoding.GetString(buffer);
                    data.Init(jsonStr);
                }
                else
                {
                    Debug.LogError("读取配置文件时发生错误！");
                    throw new Exception(request.error);
                }
            }
        }
    #endregion

    }
}