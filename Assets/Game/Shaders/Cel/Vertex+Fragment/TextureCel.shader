Shader "Cel/Vertex+Fragment/Texture Cel Shader"
{
    Properties {
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness ("Outline Thickness", Float) = 0.005
        _Texture ("Texture", 2D) = "" {} 
        _LightColor ("Light Color", Color) = (1, 1, 1, 0)
        _DetailLevel ("Detail Level", Range(0, 1)) = 0.3
        _Brightness ("Brightness Level", Range(0, 1)) = 0.45
        _Strength ("Strength Level", Range(0, 1)) = 0.24
    }

    SubShader {

        UsePass "Cel/Surface/Texture Cel Shader/Outline"

        Pass {
            Stencil {
                Ref 1
                Comp Always
                Pass Replace
            }

            Name "TextureCel"

            CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

            sampler2D _Texture;
			float4 _Texture_ST;
			float4 _LightColor;
			float _DetailLevel;
			float _Brightness;
			float _Strength;

            struct appdata {
			    float4 vertex : POSITION;
			    float3 normal : NORMAL;
			    float4 uv : TEXCOORD0;
			};

			struct vertexToFragment {
				float4 position : SV_POSITION;
				half3 worldNormal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			vertexToFragment vert (appdata b) {
				vertexToFragment o;
				o.position = UnityObjectToClipPos(b.vertex);

				o.worldNormal = UnityObjectToWorldNormal(b.normal);

				o.uv = TRANSFORM_TEX(b.uv, _Texture);

				return o;
			}

			fixed4 frag (vertexToFragment i) : SV_Target {
			    fixed4 c = tex2D(_Texture, i.uv);
			    c *= floor(max(0, dot(normalize(i.worldNormal), normalize(_WorldSpaceLightPos0.xyz))) / _DetailLevel) * _Strength * _LightColor + _Brightness;

				return c;
			}

            ENDCG
        }
    }

    Fallback "Diffuse"
}
