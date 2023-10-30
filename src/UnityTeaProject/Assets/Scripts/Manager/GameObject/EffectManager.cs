//**********************************************************************
// Script Name          : EffectManager.cs
// Author Name          : 刘坤
// Create Time          : 2023/10/30
// Last Modified Time   : 2023/10/30
// Description          : 编写特效管理器类
//**********************************************************************

using System.Collections;
using UnityEngine;

namespace TeaProject.Manager
{
    /// <summary>
    /// 特效管理器
    /// </summary>
    public class EffectManager : BaseManager<EffectManager>
    {

    #region Public or protected fields and properties
        public override IEnumerator Init(object arg = null)
        {
            yield return base.Init(arg);
        }
    #endregion

    #region Private fields and properties
        
    #endregion

    #region Unity Callback
        private void Update()
        {
            
        }
    #endregion

    #region Public or protected method
            
    #endregion

    #region Private method
        
    #endregion

    }
}