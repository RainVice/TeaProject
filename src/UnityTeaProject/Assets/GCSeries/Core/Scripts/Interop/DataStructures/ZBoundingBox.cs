////////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2020 , Inc.  All Rights Reserved.
//
////////////////////////////////////////////////////////////////////////////////

using System.Runtime.InteropServices;

namespace GCSeries.Core.Interop
{
    [StructLayout(LayoutKind.Explicit, Pack = 8)]
    public struct ZBoundingBox
    {
        [FieldOffset(0)]
        public ZVector3 lower;

        [FieldOffset(12)]
        public ZVector3 upper;
    }
}
