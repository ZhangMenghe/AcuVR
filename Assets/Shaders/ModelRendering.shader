Shader "VolumeRendering/ModelRendering"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        //_CrossSectionNum("Number Of Cross Planes", Integer) = 0
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            LOD 100
            Cull Off
            ZTest LEqual
            //ZWrite On

            Pass
            {
                Blend SrcAlpha OneMinusSrcAlpha

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct vert_in
                {
                    float4 vertex : POSITION;
                    float4 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                };

                struct frag_in
                {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 vertexLocal : TEXCOORD1;
                    float3 normal : NORMAL;
                };
                sampler2D _MainTex;
                fixed4 _Color;
                half _Glossiness;
                half _Metallic;

                int _CrossSectionNum;
                float4x4 _CrossSectionMatrices[5];
                float _CrossSectionInBounds[5];
                
                bool IsCutout(float3 currPos)
                {
                    float3 pos = currPos;
                    for (int i = 0; i < _CrossSectionNum; i++) {
                        if (_CrossSectionInBounds[i] < .0f) continue;
                        // Convert from model space to plane's vector space
                        float3 planeSpacePos = mul(_CrossSectionMatrices[i], float4(pos, 1.0f));
                        if (planeSpacePos.z > 0.0f) return true;
                    }
                    return false;
                }

                frag_in vert(vert_in v)
                {
                    frag_in o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.vertexLocal = v.vertex;
                    o.normal = UnityObjectToWorldNormal(v.normal);
                    return o;
                }

                fixed4 frag(frag_in i) : SV_Target
                {
                    if (IsCutout(i.vertexLocal)) discard;
                    fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                    return col;
                }

            ENDCG
        }
        }
}
