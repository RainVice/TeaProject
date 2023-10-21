//**********************************************************************
// Script Name          : IManager.cs
// Author Name          : 刘坤
// Create Time          : 2023/10/19
// Last Modified Time   : 2023/10/19
// Description          : 编写管理器接口
//**********************************************************************


namespace TeaProject.Manager
{
    /// <summary>
    /// 管理器接口
    /// </summary>
    public interface IManager
    {
        /// <summary>
        /// 初始化管理器
        /// </summary>
        void Init(System.Object arg);
        /// <summary>
        /// 获取当前管理器名称
        /// </summary>
        /// <returns>一个表示管理器名称的字符串</returns>
        string GetName();
    }
}