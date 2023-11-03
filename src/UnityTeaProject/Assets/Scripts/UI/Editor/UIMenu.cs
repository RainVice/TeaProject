//**********************************************************************
// Script Name          : UIMune.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023年10月28日
// Last Modified Time   : 2023年10月28日
// Description          : 右键菜单中创建UI界面通用面板
//**********************************************************************

using TeaProject.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TeaProject.Menu
{
    /// <summary>
    /// 右键菜单中创建UI界面通用面板
    /// </summary>
    public class UIMenu : MonoBehaviour
    {


        #region Public or protected method
        /// <summary>
        /// 创建左侧面板
        /// </summary>
        [MenuItem("GameObject/Tea/LeftPanel",false,0)]
        static void CreateLeftPanel()
        {
            CreateSidePanel("LeftPanel",true);

        }
        /// <summary>
        /// 创建右侧面板
        /// </summary>
        [MenuItem("GameObject/Tea/RightPlan",false,0)]
        static void CreateRightPanel()
        {
            GameObject rightPanel = CreateSidePanel("RightPanel",false);
            UISidePanel uiSidePanel = rightPanel.GetComponent<UISidePanel>();
            uiSidePanel.IsLift = false;
        }
        /// <summary>
        /// 创建侧边按钮Group
        /// </summary>
        [MenuItem("GameObject/Tea/SideBtnGroup",false,0)]
        static void CreateSideBtnGroup()
        {
            string[] prefab = AssetDatabase.FindAssets("SideBtnGroup t:Prefab");
            foreach (string s in prefab)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(s);
                GameObject perfab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                GameObject obj = Instantiate(perfab, Selection.activeGameObject.transform);
                obj.name = "SideBtnGroup";
            }
        }
        ///
        /// <summary>
        /// 创建侧边按钮
        /// </summary>
        [MenuItem("GameObject/Tea/SideBtnItem",false,0)]
        static void CreateSideBtnItem()
        {
            string[] prefab = AssetDatabase.FindAssets("SideBtnItem t:Prefab");
            foreach (string s in prefab)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(s);
                GameObject perfab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                GameObject obj = Instantiate(perfab, Selection.activeGameObject.transform);
                obj.GetComponent<Toggle>().group = obj.GetComponentInParent<ToggleGroup>();
                obj.name = "SideBtnItem";
            }
        }
        
        /// <summary>
        /// 创建侧边面板
        /// </summary>
        /// <param name="name">面板名</param>
        /// <param name="isLeft">是否左边</param>
        /// <returns></returns>
        static GameObject CreateSidePanel(string name,bool isLeft)
        {
            string[] prefab = AssetDatabase.FindAssets("SidePanel t:Prefab");
            foreach (string s in prefab)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(s);
                GameObject perfab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                GameObject obj = Instantiate(perfab, Selection.activeGameObject.transform);
                obj.name = name;
                Undo.RegisterCreatedObjectUndo(obj, $"Create {obj.name}");
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(isLeft?-100:2020, 60);
                return obj;
            }
            return null;
        }

        #endregion

    }
}