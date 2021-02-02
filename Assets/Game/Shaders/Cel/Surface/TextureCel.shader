Shader "Cel/Surface/Texture Cel Shader" {
    Properties {
        [Toggle(USE_VERTEX_COLOR)] _VertexColor ("Use vertex color", float) = 0

        [Header(Scripting Only)]
        _Blink ("Enable Blink", int) = 0
        _BlinkColor ("Blink Color", Color) = (1, 1, 1, 1)

        [Header(Base)]
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Main Texture", 2d) = "white" {}
        [Normal] _Normal ("Normal Map", 2d) = "bump" {}
        _LightCutoff ("Light Cutoff", Range(0, 1)) = 0.4
        _ShadowBands ("Shadow Bands", Range(1, 10)) = 2

        [Header(Lighting Correction)]
        _Strength ("Light Strength", Range(0, 1)) = 1
        _Brightness ("Additional Brightness", Range(0, 1)) = 0
        
        [Header(Rim)]
        _RimSize ("Rim Size", Range(0, 1)) = 0
        [HDR] _RimColor ("Rim Color", Color) = (0, 0, 0, 1)
        [Toggle(SHADOWED_RIM)] _ShadowedRim ("Rim Affected by Shadow", float) = 0

        [Header(Outline)]
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness ("Outline Thickness", Range(0, 5)) = 0

        [Header(Specular)]
        _Glossiness ("Smoothness", Range(0, 1)) = 0.5
        _Specular ("Specular Map", 2d) = "white" {}
        [HDR] _SpecularColor ("Specular Color", Color) = (0, 0, 0, 1)

        [Header(Emission)]
        [HDR] _EmissionColor ("Emission Color", Color) = (0, 0, 0, 1)
    }

    SubShader {
        Tags {"RenderType" = "Opaque"}

        Pass {
            Cull Front

            Name "Outline"

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _OutlineThickness;
            fixed4 _OutlineColor;

            struct appdata {
			    float4 vertex : POSITION;
			    float3 normal : NORMAL;
			};

            struct vertexToFragment {
                float4 position : SV_POSITION;
            };

            vertexToFragment vert (appdata b) {
                vertexToFragment o;

                o.position = UnityObjectToClipPos(b.vertex);

                float3 clipSpaceNormal = mul((float3x3)UNITY_MATRIX_VP, mul((float3x3)UNITY_MATRIX_M, b.normal));

                o.position.xy += normalize(clipSpaceNormal.xy) / _ScreenParams.xy * 2 * _OutlineThickness * o.position.w;

                return o;
            }

            fixed4 frag (vertexToFragment i) : SV_Target {
                return _OutlineColor;
            }


            ENDCG

        }

        CGPROGRAM
        #pragma surface surf CelStyle fullforwardshadows exclude_path:deferred exclude_path:prepass
        #pragma shader_feature USE_VERTEX_COLOR
        #pragma shader_feature SHADOWED_RIM

        int _Blink;
        fixed4 _BlinkColor;

        fixed4 _Color;
        sampler2D _MainTex;
        sampler2D _Normal;
        float _LightCutoff;
        float _ShadowBands;

        float _Strength;
        float _Brightness;

        float _RimSize;
        fixed4 _RimColor;

        float _Glossiness;
        sampler2D _Specular;
        fixed4 _SpecularColor;

        fixed4 _EmissionColor;

        struct Input {
            float2 uv_Normal;
            float2 uv_Specular;
            #ifdef USE_VERTEX_COLOR
            float4 color : COLOR;
            #else
            float2 uv_MainTex;
            #endif
        };

        struct CelStyleOutput {
            fixed3 Albedo;
            fixed3 Normal;
            float Smoothness;
            half3 Emission;
            fixed Alpha;
        };

        half4 LightingCelStyle (CelStyleOutput o, half3 lightDir, half3 viewDir, half atten) {
            half normalDotLight = saturate(dot(o.Normal, normalize(lightDir)));
            half diffuse = round(saturate(normalDotLight / _LightCutoff) * _ShadowBands) / _ShadowBands;

            float3 rim = step(1 - _RimSize, 1 - saturate(dot(normalize(viewDir), o.Normal))) * _RimColor;

            float3 reflection = reflect(normalize(lightDir), o.Normal);
            float viewDotInvertedReflection = dot(viewDir, -reflection);
            float3 specular = _SpecularColor.rgb * step(1 - o.Smoothness, viewDotInvertedReflection);

            half stepAtten = round(atten);
            half shadow = (diffuse * stepAtten) + _Brightness;

            half3 col = (o.Albedo + specular) * _LightColor0;

            half4 c;
            
            #ifdef SHADOWED_RIM
            c.rgb = (col + rim) * shadow;
            #else
            c.rgb = col * shadow + rim;
            #endif

            c.rgb *= _Strength;

            c.a = o.Alpha;

            return c;
        }

        void surf (Input i, inout CelStyleOutput o) {
            #ifdef USE_VERTEX_COLOR
            fixed4 c = i.color * _Color;
            #else
            fixed4 c = tex2D(_MainTex, i.uv_MainTex) * _Color;
            #endif
            c.rgb -= c.rgb * _Blink;
            c.rgb += _BlinkColor * _Blink;
            o.Albedo = c.rgb;
            o.Normal = UnpackNormal(tex2D(_Normal, i.uv_Normal));
            o.Smoothness = tex2D(_Specular, i.uv_Specular).x * _Glossiness;
            o.Emission = o.Albedo * _EmissionColor;
            o.Alpha = c.a;
        }

        ENDCG
    }

    Fallback "Diffuse"
}