////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 , Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System.Linq;

using UnityEditor;
using UnityEngine;

namespace GCSeries.Core
{
    [CustomEditor(typeof(ZCamera))]
    public class ZCameraEditor : Editor
    {
        ////////////////////////////////////////////////////////////////////////
        // Editor Callbacks
        ////////////////////////////////////////////////////////////////////////

        public override void OnInspectorGUI()
        {
            this.InitializeGUIStyles();

            this.CheckIsMainCamera();

            DrawDefaultInspector();
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Methods
        ////////////////////////////////////////////////////////////////////////

        private void InitializeGUIStyles()
        {
            if (this._helpBoxStyle == null)
            {
                this._helpBoxStyle = GUI.skin.GetStyle("HelpBox");
                this._helpBoxStyle.richText = true;
            }
        }

        private void CheckIsMainCamera()
        {
            ZCamera camera = this.target as ZCamera;

            // Check whether this is the main camera.
            if (!camera.CompareTag("MainCamera"))
            {
                EditorGUILayout.HelpBox(
                    "<b>EDITOR:</b> This camera will not render to the " +
                    "XR Overlay. To enable XR Overlay rendering, please " +
                    "set this camera's associated tag to <color=#add8e6ff>" +
                    "MainCamera</color>.",
                    MessageType.Info);

                EditorGUILayout.Space();
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Private Members
        ////////////////////////////////////////////////////////////////////////

        private GUIStyle _helpBoxStyle = null;
    }
}
