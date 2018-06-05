// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.35 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.35;sub:START;pass:START;ps:flbk:Mobile/Diffuse,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:37464,y:33737,varname:node_3138,prsc:2|emission-198-OUT;n:type:ShaderForge.SFN_Tex2d,id:254,x:35113,y:33749,ptovrint:False,ptlb:Main_map,ptin:_Main_map,varname:node_254,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5164-UVOUT;n:type:ShaderForge.SFN_Color,id:9878,x:35498,y:33705,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_9878,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:198,x:36861,y:33811,varname:node_198,prsc:2|A-9878-RGB,B-9617-OUT,C-1522-OUT;n:type:ShaderForge.SFN_Power,id:9617,x:35396,y:33896,varname:node_9617,prsc:2|VAL-254-RGB,EXP-8192-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8192,x:35095,y:34031,ptovrint:False,ptlb:Main_map_Pow,ptin:_Main_map_Pow,varname:node_8192,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Slider,id:9364,x:33782,y:33443,ptovrint:False,ptlb:u,ptin:_u,varname:node_9364,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Slider,id:2822,x:33754,y:33602,ptovrint:False,ptlb:v,ptin:_v,varname:node_2822,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_TexCoord,id:8285,x:33209,y:33623,varname:node_8285,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ComponentMask,id:2786,x:33427,y:33661,varname:node_2786,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-8285-UVOUT;n:type:ShaderForge.SFN_Multiply,id:6475,x:34207,y:33440,varname:node_6475,prsc:2|A-9364-OUT,B-2786-R;n:type:ShaderForge.SFN_Multiply,id:6753,x:34207,y:33620,varname:node_6753,prsc:2|A-2822-OUT,B-2786-G;n:type:ShaderForge.SFN_Tex2d,id:4414,x:33617,y:33850,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_4414,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1759-UVOUT;n:type:ShaderForge.SFN_Multiply,id:3225,x:34469,y:33459,varname:node_3225,prsc:2|A-6475-OUT,B-8568-OUT;n:type:ShaderForge.SFN_Panner,id:1759,x:33342,y:34014,varname:node_1759,prsc:2,spu:-0.1,spv:0|UVIN-8285-UVOUT;n:type:ShaderForge.SFN_Add,id:2548,x:33844,y:33955,varname:node_2548,prsc:2|A-4414-R,B-7206-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7206,x:33570,y:34099,ptovrint:False,ptlb:Mask_int,ptin:_Mask_int,varname:node_7206,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Clamp01,id:8568,x:34069,y:33973,varname:node_8568,prsc:2|IN-2548-OUT;n:type:ShaderForge.SFN_Multiply,id:5878,x:34491,y:33636,varname:node_5878,prsc:2|A-6753-OUT,B-8568-OUT;n:type:ShaderForge.SFN_Append,id:7419,x:34681,y:33581,varname:node_7419,prsc:2|A-3225-OUT,B-5878-OUT;n:type:ShaderForge.SFN_Panner,id:5164,x:34880,y:33803,varname:node_5164,prsc:2,spu:-0.2,spv:0|UVIN-7419-OUT,DIST-1053-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1522,x:36663,y:34061,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:node_1522,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Slider,id:1053,x:34517,y:34051,ptovrint:False,ptlb:scale,ptin:_scale,varname:node_1053,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-3,cur:0,max:2.5;proporder:9878-254-8192-9364-2822-4414-7206-1522-1053;pass:END;sub:END;*/

Shader "my shader/vfx/my_distortionUV" {
    Properties {
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Main_map ("Main_map", 2D) = "white" {}
        _Main_map_Pow ("Main_map_Pow", Float ) = 0
        _u ("u", Range(0, 2)) = 0
        _v ("v", Range(0, 2)) = 0
        _Mask ("Mask", 2D) = "white" {}
        _Mask_int ("Mask_int", Float ) = 0.5
        _Emission ("Emission", Float ) = 0
        _scale ("scale", Range(-3, 2.5)) = 0
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
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 gles gles3 metal 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Main_map; uniform float4 _Main_map_ST;
            uniform float4 _Color;
            uniform float _Main_map_Pow;
            uniform float _u;
            uniform float _v;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _Mask_int;
            uniform float _Emission;
            uniform float _scale;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float2 node_2786 = i.uv0.rg;
                float4 node_6547 = _Time + _TimeEditor;
                float2 node_1759 = (i.uv0+node_6547.g*float2(-0.1,0));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(node_1759, _Mask));
                float node_8568 = saturate((_Mask_var.r+_Mask_int));
                float2 node_5164 = (float2(((_u*node_2786.r)*node_8568),((_v*node_2786.g)*node_8568))+_scale*float2(-0.2,0));
                float4 _Main_map_var = tex2D(_Main_map,TRANSFORM_TEX(node_5164, _Main_map));
                float3 emissive = (_Color.rgb*pow(_Main_map_var.rgb,_Main_map_Pow)*_Emission);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
