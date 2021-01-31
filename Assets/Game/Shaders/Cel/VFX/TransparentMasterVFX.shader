Shader "Cel/VFX/Transparent Master VFX" {
    Properties {
        _MainTex ("Main (Grayscale) Texture", 2D) = "gray" {}
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
        _SecondaryTex ("Secondary (Grayscale) Texture", 2D) = "white" {}
        _SecondaryTertiaryPanningSpeed ("Secondary + Tertiary Texture Panning Speed", Vector) = (0, 0, 0, 0)
        /*
        note: in _SecondaryTertiaryPanningSpeed, the X and Y values refer
        to the secondary texture's panning speed, and the Z and W values
        refer to the tertiary texture's panning speed
        */

        [Header(Tertiary Texture)]
        [Toggle(TERTIARY_TEX)] _HasTertiaryTex ("Enable Tertiary Texture", float) = 0
        _TertiaryTex ("Tertiary (Grayscale) Texture", 2D) = "white" {}

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

        [Header(Rectangular Mask)]
        [Toggle(RECTANGULAR_MASK)] _RectangularMask ("Enable Rectangular Mask", float) = 0
        _RectangleWidth ("Rectangle Width", float) = 1
        _RectangleHeight ("Rectangle Height", float) = 1
        _RectangleCutoff ("Rectangle Cutoff", Range(0, 1)) = 1
        _RectangleSmoothness ("Rectangle Smoothness", Range(0, 1)) = 1

        [Header(Dissolve)]
        _Cutoff ("Clipping Amount", Range(0, 1)) = 0.1
        _CutoffSoftness ("Clipping Softness", Range(0, 1)) = 0
        [HDR] _BurnColor ("Burn Effect Color", Color) = (0, 0, 0, 0)
        _BurnSize ("Burn Effect Size", Range(0, 1)) = 0.1

        [Header(Soft Blend)]
        [Toggle(SOFT_BLEND)] _SoftBlend ("Enable Soft Blend", float) = 0
        _IntersectionMaxThreshold ("Intersection Max Threshold", float) = 1

        [Header(Vertex Offset)]
        [Toggle(VERTEX_OFFSET)] _VertexOffset ("Enable Vertex Offset", float) = 0
        _VertexOffsetAmount ("Vertex Offset Amount", float) = 0

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
            #pragma require samplelod
            #pragma shader_feature VERTEX_COLOR
            #pragma shader_feature SECONDARY_TEX
            #pragma shader_feature TERTIARY_TEX
            #pragma shader_feature COLOR_BANDING
            #pragma shader_feature POLAR_COORDINATES
            #pragma shader_feature CIRCULAR_MASK
            #pragma shader_feature RECTANGULAR_MASK
            #pragma shader_feature SOFT_BLEND
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
            float4 _SecondaryTertiaryPanningSpeed;

            sampler2D _TertiaryTex;
            float4 _TertiaryTex_ST;

            float _NumberOfBands;

            float _InnerRadius;
            float _OuterRadius;
            float _CircleSmoothness;

            float _RectangleWidth;
            float _RectangleHeight;
            float _RectangleCutoff;
            float _RectangleSmoothness;

            float _Cutoff;
            float _CutoffSoftness;
            fixed4 _BurnColor;
            float _BurnSize;

            sampler2D _CameraDepthTexture;
            float _IntersectionMaxThreshold;

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
                float2 uv3 : TEXCOORD4;
                float2 unscaledUV : TEXCOORD5;
                float4 screenSpaceUV : TEXCOORD6;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };            

            vertexToFragment vert (appdata v) {
                vertexToFragment o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv, _SecondaryTex);
                o.uv3 = TRANSFORM_TEX(v.uv, _TertiaryTex);
                
                o.unscaledUV = v.uv;

                #ifdef VERTEX_OFFSET
                float vertexOffset = tex2Dlod(_MainTex, float4(o.uv + _Time.y * _PanningSpeed.xy, 1, 1)).x;
                #ifdef SECONDARY_TEX
                vertexOffset *= tex2Dlod(_SecondaryTex, float4(o.uv2 + _Time.y * _SecondaryTertiaryPanningSpeed.xy, 1, 1)).x * 2;
                #endif
                #ifdef TERTIARY_TEX
                vertexOffset *= tex2Dlod(_TertiaryTex, float4(o.uv3 + _Time.y * _SecondaryTertiaryPanningSpeed.zw, 1, 1)).x * 2;
                #endif
                vertexOffset = ((vertexOffset * 2) - 1) * _VertexOffsetAmount;
                v.vertex.xyz += vertexOffset * v.normal;
                #endif

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
                float2 uv3 = i.uv3;
                float2 displacementUV = i.displacementUV;

                #ifdef POLAR_COORDINATES
                float2 mappedUV = uv * 2 - 1;
                uv = (atan2(mappedUV.y, mappedUV.x) / UNITY_PI / 2 + 0.5, length(mappedUV));
                mappedUV = uv2 * 2 - 1;
                uv2 = (atan2(mappedUV.y, mappedUV.x) / UNITY_PI / 2 + 0.5, length(mappedUV));
                mappedUV = uv3 * 2 - 1;
                uv3 = (atan2(mappedUV.y, mappedUV.x) / UNITY_PI / 2 + 0.5, length(mappedUV));
                mappedUV = displacementUV * 2 - 1;
                displacementUV = (atan2(mappedUV.y, mappedUV.x) / UNITY_PI / 2 + 0.5, length(mappedUV));
                #endif

                uv += _Time.y * _PanningSpeed.xy;
                displacementUV += _Time.y * _PanningSpeed.zw;
                uv2 += _Time.y * _SecondaryTertiaryPanningSpeed.xy;
                uv3 += _Time.y * _SecondaryTertiaryPanningSpeed.zw;

                float2 displacement = tex2D(_DisplacementTex, displacementUV).xy;
                displacement = ((displacement * 2) - 1) * _DisplacementAmount;


                float col = pow(saturate(lerp(0.5, tex2D(_MainTex, uv + displacement).x, _Contrast)), _Intensity);
                #ifdef SECONDARY_TEX
                col *= pow(saturate(lerp(0.5, tex2D(_SecondaryTex, uv2 + displacement).x, _Contrast)), _Intensity) * 2;
                #endif
                #ifdef TERTIARY_TEX
                col *= pow(saturate(lerp(0.5, tex2D(_TertiaryTex, uv3 + displacement).x, _Contrast)), _Intensity) * 2;
                #endif

                #ifdef CIRCULAR_MASK
                float positionRelativeToCircle = distance(i.unscaledUV, float2(0.5, 0.5));
                col *= 1 - smoothstep(_OuterRadius, _OuterRadius + _CircleSmoothness, positionRelativeToCircle);
                col *= smoothstep(_InnerRadius, _InnerRadius + _CircleSmoothness, positionRelativeToCircle);
                #endif

                #ifdef RECTANGULAR_MASK
                float2 uvInSDF = i.unscaledUV * 2 - 1;
                float positionRelativeToRectangle = max(abs(uvInSDF.x / _RectangleWidth), abs(uvInSDF.y / _RectangleHeight));
                col *= 1 - smoothstep(_RectangleCutoff, _RectangleCutoff + _RectangleSmoothness, positionRelativeToRectangle);
                #endif

                float originalCol = col;

                #ifdef COLOR_BANDING
                col = round(col * _NumberOfBands) / _NumberOfBands;
                #endif

                half cutoff = saturate(_Cutoff + (1 - i.color.a));
                float alpha = smoothstep(cutoff, cutoff + _CutoffSoftness, col);

                half clipTest = col - _Cutoff;
                clip(clipTest);

                float4 finalColor = tex2D(_GradientMap, float2(col, 0)) + _BurnColor * smoothstep(originalCol - cutoff, originalCol - cutoff + _CutoffSoftness, _BurnSize) * smoothstep(0.001, 0.1, _Cutoff);
                finalColor.rgb *= _Color * finalColor.a;
                
                #ifdef VERTEX_COLOR
                finalColor.rgb *= i.color.rgb;
                #endif

                finalColor.a = alpha * tex2D(_MainTex, uv + displacement).a * _Color.a * i.color.a;
                
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
