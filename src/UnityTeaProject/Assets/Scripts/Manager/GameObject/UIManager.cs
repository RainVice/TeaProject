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
        private Dictionary<Type, string> m_UIResources = new Dictionary<Type, string>();
    #endregion

    #region Public or protected method
        /// <summary>
        /// 初始化UI管理器
        /// </summary>
        public override void Init(object arg = null)
        {
            base.Init();
        }
        
        /// <summary>
        /// 创建一个指定类型的UI，并返回其实例。
        /// </summary>
        /// <typeparam name="T">要创建的UI的类型</typeparam>
        public T Create<T>() where T : UIMonoBehaviour
        {
            Type type = typeof(T);
            string path;
            if (m_UIResources.TryGetValue(type, out path))
            {
                UnityEngine.Object prefab = Resources.Load(path);
                if(prefab is null)
                {
                    Debug.LogError($"指定的类型[{type.Name}]在Resources下找不到预制体文件!请检查注册时的路径!");
                    return null;
                }
                GameObject obj = (GameObject)Instantiate(prefab);
                T res = obj.GetComponent<T>();
                if(res is null)
                {
                    Debug.LogError($"预制体上没有挂载的指定的组件[{typeof(T).GetType().Name}]!请检查预制体是否正确配置!");
                    return null;
                }
                res.Show();
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
            ui.Close();
            Destroy(ui.gameObject);
        }
    #endregion

    }
}