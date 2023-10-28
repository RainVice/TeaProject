//**********************************************************************
// Script Name          : UILoginActivity.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023/10/21
// Last Modified Time   : 2023/10/27
// Description          : 此文件用于控制登录界面的UI控件逻辑
//**********************************************************************

using TeaProject.Data;
using TeaProject.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace TeaProject.UI
{
    /// <summary>
    /// 此类用于控制登录界面的UI控件逻辑
    /// </summary>
    public class UILoginPanel : MonoBehaviour
    {
        #region Private fields and properties

        [SerializeField]
        private InputField m_inputUsername;

        [SerializeField]
        private InputField m_inputPassword;

        [SerializeField]
        private Button m_btnLogin;


        #endregion

        #region Unity Callback

        private void Start()
        {
            InitEventListener();
        }

        #endregion

        #region Public or protected method

        /// <summary>
        /// 初始化控件事件
        /// </summary>
        protected void InitEventListener()
        {
            m_btnLogin.onClick.AddListener(() =>
            {
                string username = m_inputUsername.text;
                string password = m_inputPassword.text;
                if (true)
                {
                    Level level = DataManager.Instance.GetData<LevelData>()
                        .Get<Level>(1).ShowLoadingPage(transform.parent);
                    LevelManager.Instance.PushLevel(level);
                }
                else
                {
                    // TODO 失败事件
                }
            });
        }

        #endregion


    }
}