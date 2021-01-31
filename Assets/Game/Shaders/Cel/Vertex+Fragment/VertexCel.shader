Shader "Cel/Vertex+Fragment/Vertex Cel Shader"
{
    Properties {
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness ("Outline Thickness", Float) = 0.005
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

            Name "Cel"

            CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			float4 _LightColor;
			float _DetailLevel;
			float _Brightness;
			float _Strength;

            struct appdata {
			    float4 vertex : POSITION;
			    float3 normal : NORMAL;
			    fixed4 color : COLOR;
			};

			struct vertexToFragment {
				float4 position : SV_POSITION;
				fixed3 baseColor : COLOR0;
				half3 worldNormal : NORMAL;
				float4 worldPosition : TEXCOORD4;
			};

			vertexToFragment vert (appdata b) {
				vertexToFragment o;
				o.position = UnityObjectToClipPos(b.vertex);

				o.baseColor = b.color;

				o.worldPosition = mul(unity_ObjectToWorld, o.position);

				o.worldNormal = UnityObjectToWorldNormal(b.normal);

				return o;
			}

			fixed4 frag (vertexToFragment i) : SV_Target {
			    fixed4 c = fixed4(i.baseColor, 1);
			    c *= floor(max(0, dot(normalize(i.worldNormal), normalize(_WorldSpaceLightPos0.xyz))) / _DetailLevel) * _Strength * _LightColor + _Brightness;

				return c;
			}

            ENDCG
        }
    }

    Fallback "Diffuse"
}
