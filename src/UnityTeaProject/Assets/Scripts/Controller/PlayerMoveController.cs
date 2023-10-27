//**********************************************************************
// Script Name          : MoveController.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023/10/23
// Last Modified Time   : 2023/10/23
// Description          : 玩家移动控制器
//**********************************************************************

using GCSeries.Core.Utility;
using UnityEngine;

namespace TeaProject
{
    /// <summary>
    /// 玩家移动控制器
    /// </summary>
    public class PlayerMoveController : MonoBehaviour
    {

        #region Public or protected fields and properties

        #endregion

        #region Private fields and properties
        /// <summary>
        /// 玩家移动速度
        /// </summary>
        [SerializeField]
        private float m_moveSpeed = 1f;
        
        /// <summary>
        /// ZDisplayAligner 组件
        /// </summary>
        [SerializeField]
        private ZDisplayAligner m_aligner;

        /// <summary>
        /// 垂直速度
        /// </summary>
        [SerializeField]
        private float m_verticalSpeed = 0.1f;
        #endregion

        #region Unity Callback

        private void FixedUpdate()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            transform.position += m_moveSpeed * v * transform.forward ;
            transform.position += m_moveSpeed * h * transform.right;
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            m_aligner.Angle += y;
            m_aligner.Angle = Mathf.Clamp(m_aligner.Angle, 45f, 135f);
            transform.localEulerAngles += new Vector3(0, x, 0);
        }
        #endregion

        #region Public or protected method

        #endregion

        #region Private method

        #endregion

    }
}