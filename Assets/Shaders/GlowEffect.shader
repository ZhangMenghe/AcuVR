Shader "Custom/GlowEffect"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_GlowColor("Glow Color", Color) = (1,1,1,1)
		_GlowIntensity("Glow Intensity", Range(0, 100)) = 0.5
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				struct v2f {
					float2 uv : TEXCOORD0;
					float3 worldPos : TEXCOORD1;
					float3 worldNormal : TEXCOORD2;
					float4 screenPos : SV_POSITION;
				};
				sampler2D _MainTex;
				float4 _Color;
				float4 _GlowColor;
				float _GlowIntensity;

				v2f vert(appdata v) {
					v2f o;
					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					o.uv = v.uv;
					o.screenPos = UnityObjectToClipPos(v.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					float4 col = tex2D(_MainTex, i.uv) * _Color;
					float3 worldNormal = normalize(i.worldNormal);
					float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

					float rim = 1.0 - max(0.0, dot(worldNormal, viewDir));
					float rimGlow = pow(rim, 3.0) *_GlowIntensity;

					fixed4 glowColor = _GlowColor * rimGlow;
					col.rgb += glowColor.rgb;
					return col;
				}
				ENDCG
			}
		}
}