//**********************************************************************
// Script Name          : UIManager.cs
// Author Name          : 刘坤
// Create Time          : 2023/10/20
// Last Modified Time   : 2023/10/20
// Description          : 
//**********************************************************************

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

    #region Private fields and properties
        private Dictionary<Type, UIElement> m_UIResources = new Dictionary<Type, UIElement>();
    #endregion

    #region Public or protected method
        /// <summary>
        /// 显示指定类型的UI
        /// </summary>
        /// <typeparam name="T">要显示的UI的类型</typeparam>
        public T Show<T>() where T : UIMonoBehaviour
        {
            Type type = typeof(T);
            UIElement info;
            if(m_UIResources.TryGetValue(type, out info))
            {
                if(info.Instance != null)
                {
                    info.Instance.Show();
                    info.Instance.gameObject.SetActive(true);
                }
                else
                {
                    UnityEngine.Object prefab = Resources.Load(info.Resources);
                    if(prefab == null)
                    {
                        Debug.LogError($"指定的类型[{typeof(T).GetType().Name}]在Resources找不到预制体文件!请检查注册时的路径!");
                        return null;
                    }
                    GameObject obj = (GameObject)Instantiate(prefab);
                }
                T res = info.Instance.GetComponent<T>();
                if(res == null)
                {
                    Destroy(info.Instance);
                    info.Instance = null;
                    Debug.LogError($"预制体上没有挂载的指定的组件[{typeof(T).GetType().Name}]!请检查预制体是否正确配置!");
                    return null;
                }
                else
                {
                    info.Instance = res;
                    res.Show();
                    return res;
                }
            }
            Debug.LogError($"无法找到类型为[{typeof(T).GetType().Name}],请检查此类型是否注册到UIManager!");
            return null;
        }

        /// <summary>
        /// 关闭指定类型的UI
        /// </summary>
        /// <typeparam name="T">要关闭的UI的类型</typeparam>
        public void Close(Type type)
        {
            UIElement info;
            if(m_UIResources.TryGetValue(type, out info))
            {
                if(info.Instance == null)
                {
                    Debug.LogError($"类型为[{type.Name}]的UI控件还为生成,无法执行关闭操作!");
                    return;
                }
                info.Instance.Close();
                if(info.Cache)
                {
                    info.Instance.gameObject.SetActive(false);
                }
                else
                {
                    Destroy(info.Instance);
                    info.Instance = null;
                }
            }
            Debug.LogError($"无法找到类型为[{type.Name}],请检查此类型是否注册到UIManager!");
        }
    
        /// <summary>
        /// 清空当前所有缓存的UI控件
        /// </summary>
        public void ClearCache()
        {
            foreach (var kvp in m_UIResources)
            {
                UIElement element = kvp.Value;
                if(element.Cache && !element.Instance.gameObject.activeInHierarchy)
                {
                    Destroy(element.Instance.gameObject);
                    element.Instance = null;
                }
            }
        }
    #endregion

    #region Private method
        private UIManager()
        {
            //请在此处手动注册所有UI类
        }
    #endregion

    #region Private class
        private struct UIElement
        {
            public string Resources;
            public bool Cache;
            public UIMonoBehaviour Instance;
        }
    #endregion

    }
}