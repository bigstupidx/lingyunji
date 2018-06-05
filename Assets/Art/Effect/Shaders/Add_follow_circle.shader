// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.05 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge

Shader "Shader Forge/add_follow_circle" {
    Properties {
        _mainTint ("mainTint", Color) = (1,1,1,1)
        _mainTex ("mainTex", 2D) = "white" {}
        _value ("value", Float ) = 1
        _liuGuang_color ("liuGuang_color", Color) = (0.5,0.5,0.5,1)
        _liuGuang_Apha ("liuGuang_Apha", 2D) = "white" {}
        _qiangdu ("qiangdu", Float ) = 2
        [MaterialToggle] _AniUvScale ("AniUvScale", Float ) = 0.5224366
        _uv_scale ("uv_scale", Range(0, 1)) = 0.5224366
        _speedDyn ("speedDyn", Float ) = 5
        _distort_tex ("distort_tex", 2D) = "white" {}
        _qiangdu_N ("qiangdu_N", Float ) = 0.1
        _bias ("bias", Float ) = 0.5
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _liuGuang_Apha; uniform float4 _liuGuang_Apha_ST;
            uniform sampler2D _distort_tex; uniform float4 _distort_tex_ST;
            uniform float4 _liuGuang_color;
            uniform float _qiangdu;
            uniform float _uv_scale;
            uniform float _speedDyn;
            uniform float _qiangdu_N;
            uniform fixed _AniUvScale;
            uniform float _bias;
            uniform sampler2D _mainTex; uniform float4 _mainTex_ST;
            uniform float4 _mainTint;
            uniform float _value;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
////// Emissive:
                float4 _mainTex_var = tex2D(_mainTex,TRANSFORM_TEX(i.uv0, _mainTex));
                float4 node_4948 = _Time + _TimeEditor;
                float _AniUvScale_var = lerp( _uv_scale, fmod((_speedDyn*node_4948.r),1.0), _AniUvScale );
                float4 _distort_tex_var = tex2D(_distort_tex,TRANSFORM_TEX(i.uv0, _distort_tex));
                float2 node_5831 = saturate(lerp((i.uv0+((i.uv0-_bias)/_AniUvScale_var)+(_distort_tex_var.r*_qiangdu_N)),float2(_bias,_bias),_AniUvScale_var));
                float4 _liuGuang_Apha_var = tex2D(_liuGuang_Apha,TRANSFORM_TEX(node_5831, _liuGuang_Apha));
                float3 emissive = ((_mainTint.rgb*_mainTex_var.r*_value)*(_liuGuang_Apha_var.rgb*_liuGuang_color.rgb*_qiangdu));
                float3 finalColor = emissive;
                return fixed4(finalColor,(_mainTint.a*i.vertexColor.a*_liuGuang_color.a));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
