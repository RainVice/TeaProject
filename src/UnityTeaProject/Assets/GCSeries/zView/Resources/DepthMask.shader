//////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2016 , Inc.  All Rights Reserved.
//
//////////////////////////////////////////////////////////////////////////

Shader "/zView/DepthMask"
{
    SubShader
    {
        // Only draw to the depth buffer.
        ColorMask 0
        ZWrite On

        Pass
        {
            // Do nothing in this pass.
        }
    }
}