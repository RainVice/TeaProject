////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 , Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using GCSeries.Core.Extensions;
using GCSeries.Core.Interop;

namespace GCSeries.Core.Utility
{
    public static class ZApplicationWindow
    {
        ////////////////////////////////////////////////////////////////////////
        // Public Static Properties
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The virtual desktop position and size in pixels of the 
        /// application window.
        /// </summary>
        /// 
        /// <remarks>
        /// When running from the Unity Editor, the position and size 
        /// correspond to the Game View window.
        /// </remarks>
        public static RectInt Rect
        {
            get
            {
                return new RectInt(0, 0, 1920, 1080);
            }
        }

        /// <summary>
        /// The size in pixels of the application window.
        /// </summary>
        /// 
        /// <remarks>
        /// When running from the Unity Editor, the size corresponds 
        /// to the Game View window.
        /// </remarks>
        public static Vector2Int Size
        {
            get
            {
                return Rect.size;
            }
        }
    }
}
