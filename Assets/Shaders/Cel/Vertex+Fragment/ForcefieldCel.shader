Shader "Cel/Vertex+Fragment/Forcefield Cel Shader"
{
    Properties {
        _ShieldColor ("Main Color", Color) = (1, 1, 1, 0)
        _FresnelPower ("Fresnel Power", Int) = 5
        _LightColor ("Light Color", Color) = (1, 1, 1, 0)
        _DetailLevel ("Detail Level", Range(0, 1)) = 0.3
        _Brightness ("Brightness Level", Range(0, 1)) = 0.45
        _Strength ("Strength Level", Range(0, 1)) = 0.24
    }

    SubShader {

        Tags {"Queue" = "Transparent"}

        Pass {
            Stencil {
                Ref 1
                Comp Always
                Pass Replace
            }

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            Name "Forcefield"

            CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			float4 _ShieldColor;
			int _FresnelPower;
			float4 _LightColor;
			float _DetailLevel;
			float _Brightness;
			float _Strength;

			struct appdata {
			    float4 vertex : POSITION;
			    float3 normal : NORMAL;
			};

			struct vertexToFragment {
				float4 position : SV_POSITION;
				half3 worldNormal : NORMAL;
				fixed4 color : COLOR0;
			};

			vertexToFragment vert (appdata b) {
				vertexToFragment o;
				o.position = UnityObjectToClipPos(b.vertex);

				o.worldNormal = UnityObjectToWorldNormal(b.normal);

				half3 directionToView = WorldSpaceViewDir(b.vertex);

				o.color = _ShieldColor;

				o.color.a *= pow((1 - dot(normalize(o.worldNormal), normalize(directionToView))), _FresnelPower);

				return o;
			}

			fixed4 frag (vertexToFragment i, uint f : VFACE) : SV_Target {
			    fixed4 c = i.color;
//			    c.rgb *= floor(max(0, dot(normalize(i.worldNormal), normalize(_WorldSpaceLightPos0.xyz))) / _DetailLevel) * _Strength * _LightColor + _Brightness;

                // Fresnel calculation
                // F => Fresnel factor
                // V => Normalized vector pointing towards the viewer
                // N => Normalized surface normal vector
                // f => value for dot(V, N) = 1
                // x => Fresnel power
                //
                // F = f + (1 - f) * pow((1 - dot(V, N)), x)

                c.a = lerp(0, c.a, f > 0);

				return c;
			}

            ENDCG
        }
    }

    Fallback "Diffuse"
}
