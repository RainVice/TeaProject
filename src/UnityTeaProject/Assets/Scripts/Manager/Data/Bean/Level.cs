//**********************************************************************
// Script Name          : Level.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023/10/27
// Last Modified Time   : 2023/10/27
// Description          : 保存场景信息，跳转伪加载
//**********************************************************************

using TeaProject.Data;
using TeaProject.Manager;
using TeaProject.Utility;

namespace TeaProject
{
    /// <summary>
    /// 保存场景信息，跳转伪加载
    /// </summary>
    public class Level : BaseLevel
    {

        #region  Public fields and properties

        
        //当前场景名
        public string Name;
        
        //下一个场景索引
        public int NextIndex;
        

        #endregion
        #region Public or protected method
        
        public Level()
        {
            OnBegin += TeaProjectUtility.CloseLoadingPage;
            OnEnd += TeaProjectUtility.ShowLoadingPage;
        }
        
        public override BaseLevel GetNextLevel()
        {
            return DataManager.Instance.GetData<LevelData>().Get<Level>(1);
        }
        #endregion
    }
}