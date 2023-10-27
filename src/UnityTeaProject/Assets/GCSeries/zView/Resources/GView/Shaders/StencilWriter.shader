Shader "GcAr/StencilWriter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-1"}
        LOD 100
        ColorMask 0
        ZWrite Off
        Stencil{
            Ref 2
            Comp Always
            Pass Replace
        }
        Pass
        {

        }
    }
}
