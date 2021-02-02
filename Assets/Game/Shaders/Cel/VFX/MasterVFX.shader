Shader "Cel/VFX/Master VFX" {
    Properties {
        _MainTex ("Main (Noise) Texture", 2D) = "gray" {}
        [NoScaleOffset] _GradientMap ("Gradient Map Texture", 2D) = "white" {}
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

        [Header(Polar Coordinates)]
        [Toggle(POLAR_COORDINATES)] _PolarCoordinates ("Enable Polar Coordinates", float) = 0

        [Header(Circular Mask)]
        [Toggle(CIRCULAR_MASK)] _CircularMask ("Enable Circular Mask", float) = 0
        _InnerRadius ("Circle Inner Radius", Range(-1, 1)) = 0
        _OuterRadius ("Circle Outer Radius", Range(0, 1)) = 0.5
        _CircleSmoothness ("Circle Smoothness", Range(0, 1)) = 1

        [Header(Dissolve)]
        _Cutoff ("Clipping Amount", Range(0, 1)) = 0.1
        [HDR] _BurnColor ("Burn Effect Color", Color) = (0, 0, 0, 0)
        _BurnSize ("Burn Effect Size", Range(0, 1)) = 0.1

        [Header(Vertex Offset)]
        [Toggle(VERTEX_OFFSET)] _VertexOffset ("Enable Vertex Offset", float) = 0
        _VertexOffsetAmount ("Vertex Offset Amount", float) = 0

        [Header(Displacement)]
        _DisplacementTex ("Displacement Texture", 2D) = "gray" {}
        _DisplacementAmount ("Displacement Amount", float) = 0
    }
    SubShader {
        Tags {"RenderType" = "Opaque"}
        Offset -1, -1
        Cull [_CullMode]

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma require samplelod
            #pragma shader_feature SECONDARY_TEX
            #pragma shader_feature COLOR_BANDING
            #pragma shader_feature POLAR_COORDINATES
            #pragma shader_feature CIRCULAR_MASK
            #pragma shader_feature VERTEX_OFFSET

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

            float _InnerRadius;
            float _OuterRadius;
            float _CircleSmoothness;

            float _Cutoff;
            fixed4 _BurnColor;
            float _BurnSize;

            float _VertexOffsetAmount;

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
                float2 unscaledUV : TEXCOORD4;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };            

            vertexToFragment vert (appdata v) {
                vertexToFragment o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.unscaledUV = v.uv;
                o.uv2 = TRANSFORM_TEX(v.uv, _SecondaryTex);

                #ifdef VERTEX_OFFSET
                float vertexOffset = tex2Dlod(_MainTex, float4(o.uv + _Time.y * _PanningSpeed.xy, 1, 1)).x;
                #ifdef SECONDARY_TEX
                vertexOffset *= tex2Dlod(_SecondaryTex, float4(o.uv2 + _Time.y * _SecondaryPanningSpeed.xy, 1, 1)).x * 2;
                #endif
                vertexOffset = ((vertexOffset * 2) - 1) * _VertexOffsetAmount;
                v.vertex.xyz += vertexOffset * v.normal;
                #endif

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.displacementUV = TRANSFORM_TEX(v.uv, _DisplacementTex);
                o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (vertexToFragment i) : SV_Target {
                float2 uv = i.uv;
                float2 uv2 = i.uv2;
                float2 displacementUV = i.displacementUV;

                #ifdef POLAR_COORDINATES
                float2 mappedUV = uv * 2 - 1;
                uv = (atan2(mappedUV.y, mappedUV.x) / UNITY_PI / 2 + 0.5, length(mappedUV));
                mappedUV = uv2 * 2 - 1;
                uv2 = (atan2(mappedUV.y, mappedUV.x) / UNITY_PI / 2 + 0.5, length(mappedUV));
                mappedUV = displacementUV * 2 - 1;
                displacementUV = (atan2(mappedUV.y, mappedUV.x) / UNITY_PI / 2 + 0.5, length(mappedUV));
                #endif

                uv += _Time.y * _PanningSpeed.xy;
                displacementUV += _Time.y * _PanningSpeed.zw;
                uv2 += _Time.y * _SecondaryPanningSpeed.xy;

                float2 displacement = tex2D(_DisplacementTex, displacementUV).xy;
                displacement = ((displacement * 2) - 1) * _DisplacementAmount;


                float col = pow(saturate(lerp(0.5, tex2D(_MainTex, uv + displacement).x, _Contrast)), _Intensity);
                #ifdef SECONDARY_TEX
                col *= pow(saturate(lerp(0.5, tex2D(_SecondaryTex, uv2 + displacement).x, _Contrast)), _Intensity) * 2;
                #endif

                #ifdef CIRCULAR_MASK
                float positionRelativeToMask = distance(i.unscaledUV, float2(0.5, 0.5));
                col *= 1 - smoothstep(_OuterRadius, _OuterRadius + _CircleSmoothness, positionRelativeToMask);
                col *= smoothstep(_InnerRadius, _InnerRadius + _CircleSmoothness, positionRelativeToMask);
                #endif

                float originalCol = col;

                #ifdef COLOR_BANDING
                col = round(col * _NumberOfBands) / _NumberOfBands;
                #endif

                half cutoff = saturate(_Cutoff + (1 - i.color.a));
                half clipTest = originalCol - _Cutoff;
                clip(clipTest);

                float4 finalColor = tex2D(_GradientMap, float2(col, 0));
                finalColor.rgb *= _Color * finalColor.a + (_BurnColor * step(clipTest, _BurnSize) * smoothstep(0.001, 0.1, _Cutoff));
                finalColor.a = 1;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                return finalColor;
            }
            ENDCG
        }
    }
}
