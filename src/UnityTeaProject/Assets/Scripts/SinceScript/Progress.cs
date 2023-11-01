//**********************************************************************
// Script Name          : IProgress.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023年11月1日
// Last Modified Time   : 2023年11月1日
// Description          : 进度
//**********************************************************************

using System.Collections.Generic;
using GCSeries.Core.Sdk;
using UnityEngine;

namespace TeaProject.Manager
{
    /// <summary>
    /// 请修改类描述。
    /// </summary>
    [RequireComponent(typeof(ZFrustum))]
    public abstract class Progress : MonoBehaviour
    {
        private List<GameObject> m_goList;

        public virtual void Init() {}
        public virtual void Enter(){}
        public virtual void Exit(){}
    }
}