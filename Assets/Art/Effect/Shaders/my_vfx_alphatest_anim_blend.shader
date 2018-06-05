// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:Mobile/Diffuse,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:32858,y:32067,varname:node_4013,prsc:2|emission-5804-OUT,alpha-9114-OUT,clip-634-OUT;n:type:ShaderForge.SFN_Tex2d,id:5121,x:30293,y:32249,ptovrint:False,ptlb:T2,ptin:_T2,varname:node_2254,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:70387129d7d1ebf43a33d07b53c858ae,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Color,id:8370,x:30343,y:31766,ptovrint:False,ptlb:T1_color,ptin:_T1_color,varname:node_4392,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:1226,x:31800,y:32296,varname:node_1226,prsc:2|A-724-OUT,B-2395-OUT;n:type:ShaderForge.SFN_Tex2d,id:6298,x:31504,y:32849,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_2377,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:4f4519c8dadf0a5478a89bac42233550,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8991,x:30339,y:31974,ptovrint:False,ptlb:T1,ptin:_T1,varname:node_9846,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:6530,x:30884,y:32186,varname:node_6530,prsc:2|A-5729-OUT,B-1053-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2352,x:30921,y:32085,ptovrint:False,ptlb:glow,ptin:_glow,varname:node_7929,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:2395,x:31076,y:32088,varname:node_2395,prsc:2|A-2352-OUT,B-6530-OUT;n:type:ShaderForge.SFN_Multiply,id:9114,x:32512,y:32968,varname:node_9114,prsc:2|A-4601-OUT,B-9179-OUT;n:type:ShaderForge.SFN_Multiply,id:4601,x:32233,y:32953,varname:node_4601,prsc:2|A-4334-OUT,B-3650-OUT;n:type:ShaderForge.SFN_Slider,id:9179,x:31738,y:33180,ptovrint:False,ptlb:alpha_size,ptin:_alpha_size,varname:node_3897,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Multiply,id:4334,x:31450,y:32597,varname:node_4334,prsc:2|A-8991-A,B-5121-A;n:type:ShaderForge.SFN_Multiply,id:3650,x:31729,y:32941,varname:node_3650,prsc:2|A-6298-A,B-9139-OUT;n:type:ShaderForge.SFN_Slider,id:9139,x:31290,y:33035,ptovrint:False,ptlb:msk_size,ptin:_msk_size,varname:node_2755,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Color,id:8718,x:31800,y:32643,ptovrint:False,ptlb:maskColor,ptin:_maskColor,varname:node_686,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:9443,x:32078,y:32772,varname:node_9443,prsc:2|A-8718-RGB,B-6298-RGB;n:type:ShaderForge.SFN_Multiply,id:5804,x:32443,y:32742,varname:node_5804,prsc:2|A-1226-OUT,B-9443-OUT;n:type:ShaderForge.SFN_Multiply,id:5729,x:30639,y:31917,varname:node_5729,prsc:2|A-8370-RGB,B-8991-RGB;n:type:ShaderForge.SFN_Color,id:9927,x:30299,y:32504,ptovrint:False,ptlb:T2_color,ptin:_T2_color,varname:node_3453,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:1053,x:30684,y:32447,varname:node_1053,prsc:2|A-5121-RGB,B-9927-RGB;n:type:ShaderForge.SFN_Multiply,id:634,x:32659,y:33228,varname:node_634,prsc:2|A-4601-OUT,B-771-OUT;n:type:ShaderForge.SFN_Slider,id:771,x:32277,y:33290,ptovrint:False,ptlb:D,ptin:_D,varname:node_1610,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_ValueProperty,id:724,x:31558,y:32221,ptovrint:False,ptlb:size,ptin:_size,varname:node_724,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;proporder:5121-8370-6298-8991-2352-8718-9927-9179-9139-771-724;pass:END;sub:END;*/

Shader "my shader/vfx/alphatest_anim_blend" {
    Properties {
        _T2 ("T2", 2D) = "black" {}
        _T1_color ("T1_color", Color) = (0.5,0.5,0.5,1)
        _Mask ("Mask", 2D) = "black" {}
        _T1 ("T1", 2D) = "white" {}
        _glow ("glow", Float ) = 0
        _maskColor ("maskColor", Color) = (0.5,0.5,0.5,1)
        _T2_color ("T2_color", Color) = (0.5,0.5,0.5,1)
        _alpha_size ("alpha_size", Range(0, 5)) = 0
        _msk_size ("msk_size", Range(0, 5)) = 0
        _D ("D", Range(0, 5)) = 0
        _size ("size", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _T2; uniform float4 _T2_ST;
            uniform float4 _T1_color;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform sampler2D _T1; uniform float4 _T1_ST;
            uniform float _glow;
            uniform float _alpha_size;
            uniform float _msk_size;
            uniform float4 _maskColor;
            uniform float4 _T2_color;
            uniform float _D;
            uniform float _size;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 _T1_var = tex2D(_T1,TRANSFORM_TEX(i.uv0, _T1));
                float4 _T2_var = tex2D(_T2,TRANSFORM_TEX(i.uv0, _T2));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float node_4601 = ((_T1_var.a*_T2_var.a)*(_Mask_var.a*_msk_size));
                clip((node_4601*_D) - 0.5);
////// Lighting:
////// Emissive:
                float3 emissive = ((_size*(_glow*((_T1_color.rgb*_T1_var.rgb)*(_T2_var.rgb*_T2_color.rgb))))*(_maskColor.rgb*_Mask_var.rgb));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,(node_4601*_alpha_size));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _T2; uniform float4 _T2_ST;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform sampler2D _T1; uniform float4 _T1_ST;
            uniform float _msk_size;
            uniform float _D;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 _T1_var = tex2D(_T1,TRANSFORM_TEX(i.uv0, _T1));
                float4 _T2_var = tex2D(_T2,TRANSFORM_TEX(i.uv0, _T2));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float node_4601 = ((_T1_var.a*_T2_var.a)*(_Mask_var.a*_msk_size));
                clip((node_4601*_D) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
