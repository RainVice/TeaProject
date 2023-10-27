Shader "GcAR/DepthRenderer"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-100"}
        ZWrite Off

        Pass 
        {
            Fog { Mode Off }
        
            CGPROGRAM
        
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct v2f 
                {
                    float4 pos : SV_POSITION;
                    float depth : FLOAT;
                };
                
                v2f vert(appdata_base v) 
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.depth = -UnityObjectToViewPos(v.vertex).z * _ProjectionParams.w;
                    return o;
                }

                float4 frag(v2f i) : COLOR 
                {
                    return EncodeFloatRGBA(i.depth);
                }
            
            ENDCG
        }
    }
}
