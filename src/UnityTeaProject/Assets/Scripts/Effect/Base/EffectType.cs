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
    [Flags]
    public enum EffectType : ushort
    {
        None = 0,
    }
}