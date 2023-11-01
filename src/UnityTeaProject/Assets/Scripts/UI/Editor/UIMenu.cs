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

namespace TeaProject.Menu
{
    /// <summary>
    /// 右键菜单中创建UI界面通用面板
    /// </summary>
    public class UIMenu : MonoBehaviour
    {


        #region Public or protected method

        [MenuItem("GameObject/Tea/LeftPanel",false,0)]
        static void CreateLeftPanel()
        {
            CreateSidePanel("LeftPanel",true);

        }
        [MenuItem("GameObject/Tea/RightPlan",false,0)]
        static void CreateRightPanel()
        {
            GameObject rightPanel = CreateSidePanel("RightPanel",false);
            UISidePanel uiSidePanel = rightPanel.GetComponent<UISidePanel>();
            uiSidePanel.IsLift = false;
        }
        static GameObject CreateSidePanel(string name,bool isLeft)
        {
            string[] prefab = AssetDatabase.FindAssets("SidePanel t:Prefab");
            foreach (string s in prefab)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(s);
                GameObject gmobj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                GameObject obj = Instantiate(gmobj, Selection.activeGameObject.transform);
                obj.name = name;
                Undo.RegisterCreatedObjectUndo(obj, $"Create {obj.name}");
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(isLeft?-147:2067, 0);
                return obj;
            }
            return null;
        }

        #endregion

    }
}