//**********************************************************************
// Script Name          : ILevel.cs
// Author Name          : 刘坤
// Create Time          : 2029/10/19
// Last Modified Time   : 2029/10/19
// Description          : 编写关卡接口
//**********************************************************************


namespace TeaProject.Manager
{
    /// <summary>
    /// 关卡接口
    /// </summary>
    public interface ILevel
    {
        /// <summary>
        /// 获取关卡的名称
        /// </summary>
        /// <returns>一个表示关卡名称的字符串</returns>
        string GetLevelName();
        /// <summary>
        /// 获取此关卡的下一个关卡
        /// </summary>
        /// <returns>下一个关卡的接口对象，为空则表示此关卡无后继关卡</returns>
        ILevel GetNextLevel();
        /// <summary>
        /// 获取用于加载场景的索引
        /// </summary>
        /// <returns>场景在Build Setting中的索引</returns>
        int GetLevelIndex();
        /// <summary>
        /// 关卡开始活动时的回调函数
        /// </summary>
        void Begin();
        /// <summary>
        /// 关卡结束活动时的回调函数
        /// </summary>
        void End();
    }
}