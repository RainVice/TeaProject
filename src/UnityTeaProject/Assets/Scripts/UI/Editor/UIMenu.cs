//**********************************************************************
// Script Name          : UIMune.cs
// Author Name          : 欧阳晨昊
// Create Time          : 2023年10月28日
// Last Modified Time   : 2023年10月28日
// Description          : 右键菜单中创建UI界面通用面板
//**********************************************************************

using GCSeries.Core;
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
        /// 创建左侧面板，需要在ZCanvas下创建
        /// </summary>
        [MenuItem("GameObject/Tea/LeftPanel",false,0)]
        static void CreateLeftPanel()
        {
            CreateSidePanel("LeftPanel",true);
        }
        
        /// <summary>
        /// 创建右侧面板，需要在ZCanvas下创建
        /// </summary>
        [MenuItem("GameObject/Tea/RightPlan",false,0)]
        static void CreateRightPanel()
        {
            GameObject rightPanel = CreateSidePanel("RightPanel",false);
            UISidePanel uiSidePanel = rightPanel.GetComponent<UISidePanel>();
            uiSidePanel.IsLift = false;
        }
        
        /// <summary>
        /// 创建侧边按钮Group，需要在ZCanvas下创建，其中会自动根据ZFrame数量创建SideBtnItem
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
                //获取SideButtonController
                var sideButtonController = obj.GetComponent<SideButtonController>();
                //获取场景中所有ZFrame组件
                var frames = FindObjectsOfType<ZFrame>();
                //添加到SideBtnGroup
                sideButtonController.ZFrames = frames;
                //找到ZCameraRig,添加到SideButtonController
                sideButtonController.ZCameraRig = FindObjectOfType<ZCameraRig>();
                
                
                ToggleGroup toggleGroup = obj.GetComponentInChildren<ToggleGroup>();
                var prefabId = AssetDatabase.FindAssets("SideBtnItem t:Prefab")[0];
                var itemPath = AssetDatabase.GUIDToAssetPath(prefabId);
                var atPath = AssetDatabase.LoadAssetAtPath(itemPath, typeof(GameObject)) as GameObject;
                //生成item
                for (var i = 0; i < frames.Length; i++)
                {
                    var instantiate = Instantiate(atPath, toggleGroup.transform);
                    instantiate.GetComponent<Toggle>().group = toggleGroup;
                }
                Undo.RegisterCreatedObjectUndo(obj, $"Create {obj.name}");
            }
        }
        
        ///
        /// <summary>
        /// 创建侧边按钮，普通按钮，需要自定义按钮点击事件
        /// </summary>
        [MenuItem("GameObject/Tea/SideBtn",false,0)]
        static void CreateSideBtn()
        {
            string[] prefabs = AssetDatabase.FindAssets("SideBtn t:Prefab");
            foreach (string s in prefabs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(s);
                GameObject perfab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                GameObject obj = Instantiate(perfab, Selection.activeGameObject.transform);
                obj.GetComponent<Toggle>().group = obj.GetComponentInParent<ToggleGroup>();
                Undo.RegisterCreatedObjectUndo(obj, $"Create {obj.name}");
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