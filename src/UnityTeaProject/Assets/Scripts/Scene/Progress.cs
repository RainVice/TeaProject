//**********************************************************************
// Script Name          : IProgress.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023年11月1日
// Last Modified Time   : 2023年11月1日
// Description          : 保存每个ZFrame面向的进度，此脚本只能绑定在ZFrame上
//**********************************************************************

using System.Collections.Generic;
using GCSeries.Core.Sdk;
using UnityEngine;

namespace TeaProject.Manager
{
    /// <summary>
    /// 保存每个ZFrame面向的进度，此脚本只能绑定在ZFrame上
    /// </summary>
    [RequireComponent(typeof(ZFrustum))]
    public abstract class Progress : MonoBehaviour
    {
        private List<GameObject> m_goList;

        /// <summary>
        /// 实现此步骤的状态初始化
        /// </summary>
        public virtual void Init() {}
        /// <summary>
        /// 开始此步骤
        /// </summary>
        public virtual void Enter(){}
        /// <summary>
        /// 退出步骤时要做的事
        /// </summary>
        public virtual void Exit(){}
    }
}