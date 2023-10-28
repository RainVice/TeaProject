//**********************************************************************
// Script Name          : UILoginActivity.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023/10/21
// Last Modified Time   : 2023/10/27
// Description          : 此文件用于控制登录界面的UI控件逻辑
//**********************************************************************

using TeaProject.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace TeaProject.UI
{
    /// <summary>
    /// 此类用于控制登录界面的UI控件逻辑
    /// </summary>
    public class UILoginActivity : UIWindow
    {
        #region Private fields and properties

        [SerializeField]
        private InputField m_inputUsername;

        [SerializeField]
        private InputField m_inputPassword;

        [SerializeField]
        private Button m_btnLogin;

        [SerializeField]
        private GameObject m_textErrorMsg;

        [SerializeField]
        private GameObject m_loadObject;


        #endregion

        #region Unity Callback

        private void Start()
        {
            InitEventListener();
        }

        private void Update()
        {
            //输入框获取焦点事件
            InputFieldFocuseEvent();
        }

        #endregion

        #region Public or protected method

        /// <summary>
        /// 初始化控件事件
        /// </summary>
        protected void InitEventListener()
        {
            m_btnLogin.onClick.AddListener(delegate
            {
                string username = m_inputUsername.text;
                string password = m_inputPassword.text;
                //if (username == "admin" && password == "Yd@123456")
                if (true)
                {
                    // Level level = new Level(1,transform.parent);
                    // level.OnloadEvent += (o) =>
                    // {
                    //     
                    // };
                    //
                    // LevelManager.Instance.PushLevel(level);

                    Level level = DataManager.Instance.GetData<TestData>().Get<Level>(1);
                    Debug.Log(level.GetLevelIndex());
                }
                //else
                //{
                //    // TODO 失败事件
                //    //显示错误提示
                //    m_textErrorMsg.SetActive(true);
                //    //清空密码框
                //    m_inputPassword.text = "";
                //}
            });
        }

        #endregion

        #region Private method

        /// <summary>
        /// 监听输入框获得焦点
        /// </summary>
        private void InputFieldFocuseEvent()
        {
            if (m_inputPassword.isFocused || m_inputUsername.isFocused)
            {
                m_textErrorMsg.SetActive(false);
            }
        }
        #endregion

    }
}