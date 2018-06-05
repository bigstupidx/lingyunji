// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32837,y:32684,varname:node_4795,prsc:2|emission-2393-OUT,alpha-798-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:32079,y:32455,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2393,x:32621,y:32750,varname:node_2393,prsc:2|A-717-OUT,B-2053-RGB,C-797-RGB,D-9248-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:32235,y:32772,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:32235,y:32930,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:9248,x:32235,y:33081,varname:node_9248,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:798,x:32641,y:32971,varname:node_798,prsc:2|A-6074-A,B-2053-A,C-797-A;n:type:ShaderForge.SFN_Tex2d,id:236,x:31683,y:32641,ptovrint:False,ptlb:LiuGuang,ptin:_LiuGuang,varname:node_236,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8432-OUT;n:type:ShaderForge.SFN_Color,id:4457,x:31683,y:32833,ptovrint:False,ptlb:LiuGuang_Color,ptin:_LiuGuang_Color,varname:node_4457,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:6933,x:32079,y:32653,varname:node_6933,prsc:2|A-236-RGB,B-4457-RGB,C-2024-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2024,x:31893,y:32899,ptovrint:False,ptlb:LiuGuang_QiangDu,ptin:_LiuGuang_QiangDu,varname:node_2024,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_ChannelBlend,id:717,x:32453,y:32444,varname:node_717,prsc:2,chbt:1|M-7335-OUT,R-6933-OUT,BTM-6074-RGB;n:type:ShaderForge.SFN_TexCoord,id:6796,x:30674,y:32545,varname:node_6796,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:3321,x:31102,y:32548,varname:node_3321,prsc:2|A-6796-UVOUT,B-8450-OUT,C-3381-OUT;n:type:ShaderForge.SFN_Subtract,id:8362,x:30837,y:32690,varname:node_8362,prsc:2|A-6796-UVOUT,B-1254-OUT;n:type:ShaderForge.SFN_Divide,id:8450,x:30972,y:32889,varname:node_8450,prsc:2|A-8362-OUT,B-3370-OUT;n:type:ShaderForge.SFN_Clamp01,id:8432,x:31452,y:32641,varname:node_8432,prsc:2|IN-5211-OUT;n:type:ShaderForge.SFN_Lerp,id:5211,x:31278,y:32641,varname:node_5211,prsc:2|A-3321-OUT,B-1254-OUT,T-3370-OUT;n:type:ShaderForge.SFN_Vector1,id:5662,x:30138,y:32681,varname:node_5662,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Time,id:9400,x:30520,y:33233,varname:node_9400,prsc:2;n:type:ShaderForge.SFN_Fmod,id:1081,x:30984,y:33227,varname:node_1081,prsc:2|A-123-OUT,B-1662-OUT;n:type:ShaderForge.SFN_Vector1,id:1662,x:30774,y:33378,varname:node_1662,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:123,x:30774,y:33227,varname:node_123,prsc:2|A-9400-TSL,B-7478-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7478,x:30520,y:33409,ptovrint:False,ptlb:LiuGuang_Speed,ptin:_LiuGuang_Speed,varname:node_7478,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:5;n:type:ShaderForge.SFN_SwitchProperty,id:7279,x:31924,y:32306,ptovrint:False,ptlb:WuAlpha,ptin:_WuAlpha,varname:node_7279,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-236-A,B-236-R;n:type:ShaderForge.SFN_Multiply,id:7335,x:32223,y:32303,varname:node_7335,prsc:2|A-7279-OUT,B-6074-A,C-6025-R;n:type:ShaderForge.SFN_Tex2d,id:8091,x:30674,y:32368,ptovrint:False,ptlb:UVNiuQu,ptin:_UVNiuQu,varname:node_8091,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3381,x:30948,y:32225,varname:node_3381,prsc:2|A-8091-R,B-7323-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7323,x:30127,y:32425,ptovrint:False,ptlb:NiuQu_QiangDu,ptin:_NiuQu_QiangDu,varname:node_7323,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Add,id:1254,x:30503,y:32660,varname:node_1254,prsc:2|A-9961-OUT,B-5662-OUT;n:type:ShaderForge.SFN_Multiply,id:9961,x:30349,y:32527,varname:node_9961,prsc:2|A-7323-OUT,B-1377-OUT;n:type:ShaderForge.SFN_Vector1,id:1377,x:30138,y:32548,varname:node_1377,prsc:2,v1:0.15;n:type:ShaderForge.SFN_SwitchProperty,id:3370,x:30972,y:33085,ptovrint:False,ptlb:AniUVScale,ptin:_AniUVScale,varname:node_3370,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-5589-OUT,B-1081-OUT;n:type:ShaderForge.SFN_Slider,id:5589,x:30479,y:33093,ptovrint:False,ptlb:UVScale,ptin:_UVScale,varname:node_5589,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3,max:1;n:type:ShaderForge.SFN_Tex2d,id:6025,x:31717,y:32110,ptovrint:False,ptlb:LiuGuang_Mask,ptin:_LiuGuang_Mask,varname:node_6025,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;proporder:6074-797-236-7279-4457-2024-5589-3370-7478-6025-8091-7323;pass:END;sub:END;*/

Shader "Effects/AniScaleUV_BF" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _LiuGuang ("LiuGuang", 2D) = "white" {}
        [MaterialToggle] _WuAlpha ("WuAlpha", Float ) = 0
        _LiuGuang_Color ("LiuGuang_Color", Color) = (0.5,0.5,0.5,1)
        _LiuGuang_QiangDu ("LiuGuang_QiangDu", Float ) = 3
        _UVScale ("UVScale", Range(0, 1)) = 0.3
        [MaterialToggle] _AniUVScale ("AniUVScale", Float ) = 0.3
        _LiuGuang_Speed ("LiuGuang_Speed", Float ) = 5
        _LiuGuang_Mask ("LiuGuang_Mask", 2D) = "white" {}
        _UVNiuQu ("UVNiuQu", 2D) = "white" {}
        _NiuQu_QiangDu ("NiuQu_QiangDu", Float ) = 0
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
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _LiuGuang; uniform float4 _LiuGuang_ST;
            uniform float4 _LiuGuang_Color;
            uniform float _LiuGuang_QiangDu;
            uniform float _LiuGuang_Speed;
            uniform fixed _WuAlpha;
            uniform sampler2D _UVNiuQu; uniform float4 _UVNiuQu_ST;
            uniform float _NiuQu_QiangDu;
            uniform fixed _AniUVScale;
            uniform float _UVScale;
            uniform sampler2D _LiuGuang_Mask; uniform float4 _LiuGuang_Mask_ST;
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
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float node_1254 = ((_NiuQu_QiangDu*0.15)+0.5);
                float4 node_9400 = _Time + _TimeEditor;
                float _AniUVScale_var = lerp( _UVScale, fmod((node_9400.r*_LiuGuang_Speed),1.0), _AniUVScale );
                float4 _UVNiuQu_var = tex2D(_UVNiuQu,TRANSFORM_TEX(i.uv0, _UVNiuQu));
                float2 node_8432 = saturate(lerp((i.uv0+((i.uv0-node_1254)/_AniUVScale_var)+(_UVNiuQu_var.r*_NiuQu_QiangDu)),float2(node_1254,node_1254),_AniUVScale_var));
                float4 _LiuGuang_var = tex2D(_LiuGuang,TRANSFORM_TEX(node_8432, _LiuGuang));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 _LiuGuang_Mask_var = tex2D(_LiuGuang_Mask,TRANSFORM_TEX(i.uv0, _LiuGuang_Mask));
                float node_7335 = (lerp( _LiuGuang_var.a, _LiuGuang_var.r, _WuAlpha )*_MainTex_var.a*_LiuGuang_Mask_var.r);
                float3 emissive = ((lerp( _MainTex_var.rgb, (_LiuGuang_var.rgb*_LiuGuang_Color.rgb*_LiuGuang_QiangDu), node_7335.r ))*i.vertexColor.rgb*_TintColor.rgb*2.0);
                float3 finalColor = emissive;
                return fixed4(finalColor,(_MainTex_var.a*i.vertexColor.a*_TintColor.a));
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
