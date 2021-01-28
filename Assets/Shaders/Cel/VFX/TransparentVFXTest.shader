Shader "Cel/VFX/Transparent VFX Test" {
    Properties {
        _MainTex ("Main (Noise) Texture", 2D) = "gray" {}
        [NoScaleOffset] _GradientMap ("Gradient Map Texture", 2D) = "white" {}
        [Toggle(VERTEX_COLOR)] _VertexColor ("Use Vertex Color", float) = 0
        [HDR] _Color ("Light Color", Color) = (1, 1, 1, 1)
        _Contrast ("Light Contrast", Range(0, 1)) = 1
        _Intensity ("Light Intensity", float) = 1
        _PanningSpeed ("Texture Panning Speed", Vector) = (0, 0, 0, 0)
        /*
        note: in _PanningSpeed, the X and Y values refer to the main
        texture's panning speed, and the Z and W values refer to the
        displacement texture's panning speed
        */

        [Header(Secondary Texture)]
        [Toggle(SECONDARY_TEX)] _HasSecondaryTex ("Enable Secondary Texture", float) = 0
        _SecondaryTex ("Secondary (Noise) Texture", 2D) = "white" {}
        _SecondaryPanningSpeed ("Secondary Texture Panning Speed", Vector) = (0, 0, 0, 0)

        [Header(Culling)]
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode ("Cull Mode", Int) = 2

        [Header(Color Banding)]
        [Toggle(COLOR_BANDING)] _ColorBanding ("Enable Color Banding", float) = 0
        _NumberOfBands ("Number of Bands", Range(1, 10)) = 2

        [Header(Soft Blend)]
        [Toggle(SOFT_BLEND)] _SoftBlend ("Enable Soft Blend", float) = 0
        _IntersectionMaxThreshold ("Intersection Max Threshold", float) = 1

        [Header(Displacement)]
        _DisplacementTex ("Displacement Texture", 2D) = "gray" {}
        _DisplacementAmount ("Displacement Amount", float) = 0
    }
    SubShader {
        Tags {"RenderType" = "Transparent" "Queue" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Offset -1, -1
        Cull [_CullMode]

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature VERTEX_COLOR
            #pragma shader_feature SECONDARY_TEX
            #pragma shader_feature COLOR_BANDING
            #pragma shader_feature SOFT_BLEND

            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _GradientMap;
            fixed4 _Color;
            float _Contrast;
            float _Intensity;
            float4 _PanningSpeed;

            sampler2D _SecondaryTex;
            float4 _SecondaryTex_ST;
            float4 _SecondaryPanningSpeed;

            float _NumberOfBands;

            sampler2D _CameraDepthTexture;
            float _IntersectionMaxThreshold;

            sampler2D _DisplacementTex;
            float4 _DisplacementTex_ST;
            float _DisplacementAmount;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                fixed4 color : COLOR;
            };

            struct vertexToFragment {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float2 displacementUV : TEXCOORD2;
                float2 uv2 : TEXCOORD3;
                float4 screenSpaceUV : TEXCOORD4;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };            

            vertexToFragment vert (appdata v) {
                vertexToFragment o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv, _SecondaryTex);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenSpaceUV = ComputeScreenPos(o.vertex);
                o.displacementUV = TRANSFORM_TEX(v.uv, _DisplacementTex);
                o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (vertexToFragment i) : SV_Target {
                float2 uv = i.uv;
                float2 uv2 = i.uv2;
                float2 displacementUV = i.displacementUV;

                uv += _Time.y * _PanningSpeed.xy;
                displacementUV += _Time.y * _PanningSpeed.zw;
                uv2 += _Time.y * _SecondaryPanningSpeed.xy;

                float2 displacement = tex2D(_DisplacementTex, displacementUV).xy;
                displacement = ((displacement * 2) - 1) * _DisplacementAmount;


                float4 col = pow(saturate(lerp(0.5, tex2D(_MainTex, uv + displacement).x, _Contrast)), _Intensity);
                #ifdef SECONDARY_TEX
                col *= pow(saturate(lerp(0.5, tex2D(_SecondaryTex, uv2 + displacement).x, _Contrast)), _Intensity) * 2;
                #endif

                float originalCol = col;

                #ifdef COLOR_BANDING
                col = round(col * _NumberOfBands) / _NumberOfBands;
                #endif

                float4 finalColor = col;
                finalColor.rgb *= _Color * finalColor.a;
                
                #ifdef VERTEX_COLOR
                finalColor.rgb *= i.color.rgb;
                #endif

                finalColor.a = tex2D(_MainTex, uv + displacement).a * _Color.a * i.color.a;
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, finalColor);

                #ifdef SOFT_BLEND
                float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenSpaceUV)));
                float depthDifference = saturate(_IntersectionMaxThreshold * (depth - i.screenSpaceUV.w));
                finalColor.a *= depthDifference;
                #endif

                return finalColor;
            }
            ENDCG
        }
    }
}
