Shader "GcAR/DepthGrayscale"
{
   SubShader 
   {
      Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
      ZTest Always Cull Off ZWrite Off
      Fog { Mode off }
      
      Pass
      {
         // Blend SrcAlpha OneMinusSrcAlpha 
         CGPROGRAM
         #pragma vertex vert
         #pragma fragment frag
         #include "UnityCG.cginc"
 
         sampler2D _webCamTexture;

         struct appdata 
         {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
         };
         struct v2f 
         {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
         };

         //Vertex Shader
         v2f vert (appdata v)
         {
            v2f o;
            o.pos = UnityObjectToClipPos (v.vertex);
            o.uv = v.uv;
            return o;
         }

         //Fragment Shader
         half4 frag (v2f i) : COLOR
         {
            fixed4 col = tex2D(_webCamTexture, i.uv);
            return col;
         }
         ENDCG
      }

      Pass
      {
         Blend One OneMinusSrcAlpha
         CGPROGRAM
         #pragma vertex vert
         #pragma fragment frag
         #include "UnityCG.cginc"
 
         sampler2D _noneCameraDepthTexture;
         sampler2D _RenderTexture;
         sampler2D _customDepthMask;

         struct appdata 
         {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
         };
         struct v2f 
         {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
            float4 scrPos:TEXCOORD1;
         };

         //Vertex Shader
         v2f vert (appdata v)
         {
            v2f o;
            o.pos = UnityObjectToClipPos (v.vertex);
            o.scrPos = ComputeScreenPos(o.pos);
            o.uv = v.uv;
            return o;
         }

         //Fragment Shader
         half4 frag (v2f i) : COLOR
         {
            //float depthValue = Linear01Depth (tex2Dproj(_noneCameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)).r);
            float depthValue = DecodeFloatRGBA (tex2D(_noneCameraDepthTexture, i.uv));

            float customDepth = DecodeFloatRGBA(tex2D(_customDepthMask, i.uv));

            fixed4 result = fixed4(1,1,1,1);
            // 出屏部分
            if(depthValue - customDepth < -0.0001)
               result = tex2D(_RenderTexture, i.uv);
            // 景深部分
            else if(customDepth < 0.0001) 
            {
               result = tex2D(_RenderTexture, i.uv);
               result.w = 1;
            }
            // 深度测试失败，遮挡丢弃部分
            else
               discard;

            return result;
         }
         ENDCG
      }
   }
   FallBack "Diffuse"
}
