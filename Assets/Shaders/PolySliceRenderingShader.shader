// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "VolumeRendering/PolySliceRenderingShader"
{
    Properties
    {
        _DataTex("Data Texture (Generated)", 3D) = "" {}
        _TFTex("Transfer Function Texture", 2D) = "white" {}
        _DrawTex("Any drawings and annotations", 2D) = "(0,0,0,0)" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        LOD 100
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile __ ANNOTATION_ON
            #pragma multi_compile __ OVERRIDE_MODEL_MAT


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 relVert : TEXCOORD1;
            };

            sampler3D _DataTex;
            sampler2D _TFTex;
            sampler2D _DrawTex;
            // Parent's inverse transform (used to convert from world space to volume space)
            uniform float4x4 _parentInverseMat;
            // Plane transform
            uniform float4x4 _planeMat;
            uniform float4x4 _planeModelMat;

            v2f vert (appdata v)
            {
                v2f o;
#ifdef OVERRIDE_MODEL_MAT
                o.vertex = mul(UNITY_MATRIX_VP, mul(_planeModelMat, v.vertex));
#else
                o.vertex = UnityObjectToClipPos(v.vertex);
#endif
                // Calculate plane vertex world position.
                float3 vert = mul(_planeMat, float4(0.5f - v.uv.x, 0.0f, 0.5f - v.uv.y, 1.0f));
                o.relVert = mul(_parentInverseMat, float4(vert, 1.0f));
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                float3 dataCoord = i.relVert + float3(0.5f, 0.5f, 0.5f);
                if (dataCoord.x > 1.0f || dataCoord.y > 1.0f || dataCoord.z > 1.0f || dataCoord.x < 0.0f || dataCoord.y < 0.0f || dataCoord.z < 0.0f)
                {
                   return float4(0.0f, 0.0f, 0.0f, 1.0f);
                }
                //return float4(1.0f, .0f, .0f, 1.0f);
                // Sample the volume texture.
                float dataVal = tex3D(_DataTex, dataCoord);
                float4 col = tex2D(_TFTex, float2(dataVal, 0.0f));
                col.a = 1.0f;

#ifdef ANNOTATION_ON
                float4 brush = tex2D(_DrawTex, i.uv);
                if(brush.a != .0f) 
                col = lerp(col, brush, 0.3f);
#endif
                return col;
            }
            ENDCG
        }
    }
}
