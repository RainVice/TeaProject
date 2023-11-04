//**********************************************************************
// Script Name          : UIManager.cs
// Author Name          : 刘坤
// Create Time          : 2023/10/20
// Last Modified Time   : 2023/10/30
// Description          : 编写UI管理器类
//**********************************************************************

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TeaProject.UI;

namespace TeaProject.Manager
{
    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIManager : BaseManager<UIManager>
    {

        #region Public fields and properties

        public Canvas MainCanvas 
        {
            get
            {
                return m_MainCanvas;
            } 
        }

        #endregion
        
    #region Private fields and properties
        private Dictionary<Type, string> m_UIResources = new Dictionary<Type, string>();
        private Dictionary<Type, Queue<UIMonoBehaviour>> m_CachePool = new Dictionary<Type, Queue<UIMonoBehaviour>>();
        private Canvas m_MainCanvas;
    #endregion

    #region Public or protected method
        /// <summary>
        /// 初始化UI管理器，此方法接受一个 List(Tuple(Type, string)) 类型数据
        /// </summary>
        public override IEnumerator Init(object arg = null)
        {
            yield return base.Init();
            List<Tuple<Type, string>> list = arg as List<Tuple<Type, string>>;
            if(list == null)
            {
                Debug.LogError("使用了错误的参数来初始化UI管理器");
                throw new ArgumentException("参数类型应兼容 List(Tuple(Type, string)) "); 
            } 
            foreach (Tuple<Type, string> tuple in list)
            {
                Type type = tuple.Item1;
                string str = tuple.Item2;
                if(m_UIResources.ContainsKey(type))
                {
                    Debug.LogWarning($"UIMananger中注册了多次类型为 {type.Name} 的元素");
                }
                else
                {
                    m_UIResources.Add(type, str);
                }
            }

            m_MainCanvas = transform.GetComponentInChildren<Canvas>();
        }
        
        /// <summary>
        /// 创建一个指定类型的UI，并返回其实例。
        /// </summary>
        /// <typeparam name="T">要创建的UI的类型</typeparam>
        public T Show<T>() where T : UIMonoBehaviour
        {
            Type type = typeof(T);
            string path;
            Queue<UIMonoBehaviour> queue;
            T res;
            if(m_CachePool.TryGetValue(type, out queue) && queue.Count > 0)
            {
                res = queue.Dequeue() as T;
                res.transform.SetParent(m_MainCanvas.transform);
                res.gameObject.SetActive(true);
                return res;
            }
            else if (m_UIResources.TryGetValue(type, out path))
            {
                UnityEngine.Object prefab = Resources.Load(path);
                if(prefab is null)
                {
                    Debug.LogError($"指定的类型[{type.Name}]在Resources下找不到预制体文件!请检查注册时的路径!");
                    return null;
                }
                GameObject obj = (GameObject)Instantiate(prefab, m_MainCanvas.transform);
                res = obj.GetComponent<T>();
                if(res is null)
                {
                    Debug.LogError($"预制体上没有挂载的指定的组件[{type.Name}]!请检查预制体是否正确配置!");
                    return null;
                }
                res.OnShow();
                return res;
            }
            else
            {
                Debug.LogError($"无法找到类型为[{type.Name}]的UI控件路径,请检查此类型是否注册到UIManager!");
                return null;
            }
        }

        /// <summary>
        /// 关闭指定的UI
        /// </summary>
        /// <typeparam name="T">要关闭的UI的类型</typeparam>
        public void Close<T>(T ui) where T : UIMonoBehaviour
        {
            if(ui is null) throw new ArgumentNullException("参数不能为空类型！");
            ui.OnClose();
            if(ui.Cache)
            {
                ui.transform.SetParent(transform);
                ui.gameObject.SetActive(false);
                Type type = typeof(T);
                if(!m_CachePool.ContainsKey(type))
                    m_CachePool.Add(type, new Queue<UIMonoBehaviour>());
                m_CachePool[type].Enqueue(ui);                  
            }
            else
            {
                Destroy(ui.gameObject);
            }
        }
    #endregion
    
    }
}