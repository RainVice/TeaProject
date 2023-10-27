////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 , Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System.Runtime.InteropServices;

namespace GCSeries.Core.Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct ZDisplayIntersectionInfo
    {
        [MarshalAs(UnmanagedType.Bool)]
        public bool hit;
        public int x;
        public int y;
        public int nx;
        public int ny;
        public float distance;
    }
}
