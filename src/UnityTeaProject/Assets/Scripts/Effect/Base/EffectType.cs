//**********************************************************************
// Script Name          : EffectType.cs
// Author Name          : 刘坤
// Create Time          : 2023/10/30
// Last Modified Time   : 2023/10/30
// Description          : 编写特效类型枚举
//**********************************************************************

using System;

namespace TeaProject.Effect
{
    /// <summary>
    /// 一个用于表示特效类型的，可组合的枚举类型
    /// </summary>
    [Flags]
    public enum EffectType : ushort
    {
        None = 0,           //无
        RectTransform = 2,  //表示此特效是一个RectTransform类型特效
    }
}