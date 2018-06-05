// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.27 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.27;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4285,x:33528,y:32645,varname:node_4285,prsc:2|emission-8707-OUT,clip-1011-A;n:type:ShaderForge.SFN_Tex2d,id:1011,x:31991,y:32758,ptovrint:False,ptlb:node_1011,ptin:_node_1011,varname:node_1011,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:8395,x:31991,y:32958,ptovrint:False,ptlb:node_8395,ptin:_node_8395,varname:node_8395,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:1361,x:32571,y:32902,varname:node_1361,prsc:2|A-1011-RGB,B-8395-RGB,C-8395-A;n:type:ShaderForge.SFN_Vector1,id:1696,x:32372,y:32752,varname:node_1696,prsc:2,v1:5;n:type:ShaderForge.SFN_Multiply,id:7589,x:32927,y:32709,varname:node_7589,prsc:2|A-2484-OUT,B-1361-OUT;n:type:ShaderForge.SFN_Slider,id:956,x:32271,y:32669,ptovrint:False,ptlb:juese_liangdu,ptin:_juese_liangdu,varname:node_956,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3333333,max:1;n:type:ShaderForge.SFN_Multiply,id:2484,x:32634,y:32670,varname:node_2484,prsc:2|A-956-OUT,B-1696-OUT;n:type:ShaderForge.SFN_Tex2d,id:1029,x:32159,y:33085,ptovrint:False,ptlb:tex_bing,ptin:_tex_bing,varname:node_1029,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ffb6ca48a6c248f46a3636c4baffaf88,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:8707,x:33195,y:32705,varname:node_8707,prsc:2|A-7589-OUT,B-9901-OUT,T-1596-OUT;n:type:ShaderForge.SFN_Slider,id:1596,x:32712,y:33096,ptovrint:False,ptlb:shuihuojiaohuan,ptin:_shuihuojiaohuan,varname:node_1596,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Color,id:2533,x:32114,y:33280,ptovrint:False,ptlb:bing_yanse,ptin:_bing_yanse,varname:node_2533,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:4910,x:32597,y:33173,varname:node_4910,prsc:2|A-1029-RGB,B-2533-RGB,C-2533-A;n:type:ShaderForge.SFN_Fresnel,id:3154,x:32344,y:33777,varname:node_3154,prsc:2|NRM-5130-OUT,EXP-9898-OUT;n:type:ShaderForge.SFN_Slider,id:9898,x:31804,y:33926,ptovrint:False,ptlb:waifaguang_liangdu,ptin:_waifaguang_liangdu,varname:node_9898,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Add,id:9901,x:33215,y:33350,varname:node_9901,prsc:2|A-6984-OUT,B-7294-OUT;n:type:ShaderForge.SFN_NormalVector,id:5130,x:32072,y:33777,prsc:2,pt:False;n:type:ShaderForge.SFN_Color,id:672,x:32302,y:33963,ptovrint:False,ptlb:waifaguang_yase,ptin:_waifaguang_yase,varname:node_672,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:7294,x:32497,y:33891,varname:node_7294,prsc:2|A-3154-OUT,B-672-RGB;n:type:ShaderForge.SFN_Tex2d,id:6782,x:32114,y:33452,ptovrint:False,ptlb:tex_huoyan,ptin:_tex_huoyan,varname:node_6782,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7128-UVOUT;n:type:ShaderForge.SFN_Multiply,id:800,x:32538,y:33393,varname:node_800,prsc:2|A-6782-RGB,B-4289-RGB,C-4289-A;n:type:ShaderForge.SFN_Color,id:4289,x:32114,y:33637,ptovrint:False,ptlb:huoyan_yanse,ptin:_huoyan_yanse,varname:node_4289,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_SwitchProperty,id:6984,x:32779,y:33370,ptovrint:False,ptlb:binghuoqiehuan,ptin:_binghuoqiehuan,varname:node_6984,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-4910-OUT,B-800-OUT;n:type:ShaderForge.SFN_Panner,id:7128,x:31769,y:33426,varname:node_7128,prsc:2,spu:0,spv:1|UVIN-4241-UVOUT,DIST-3509-OUT;n:type:ShaderForge.SFN_TexCoord,id:4241,x:31357,y:33409,varname:node_4241,prsc:2,uv:0;n:type:ShaderForge.SFN_Time,id:1270,x:31252,y:33569,varname:node_1270,prsc:2;n:type:ShaderForge.SFN_Slider,id:5970,x:31198,y:33739,ptovrint:False,ptlb:huoyanliudong,ptin:_huoyanliudong,varname:node_5970,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:3509,x:31600,y:33570,varname:node_3509,prsc:2|A-1270-T,B-5434-OUT;n:type:ShaderForge.SFN_Clamp,id:5434,x:31512,y:33822,varname:node_5434,prsc:2|IN-5970-OUT,MIN-1425-OUT,MAX-3259-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1425,x:31103,y:33839,ptovrint:False,ptlb:node_1425,ptin:_node_1425,varname:node_1425,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:3259,x:31103,y:33922,ptovrint:False,ptlb:node_3259,ptin:_node_3259,varname:node_3259,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;proporder:1011-8395-956-1029-1596-2533-9898-672-6782-4289-6984-5970-1425-3259;pass:END;sub:END;*/

Shader "wangrui/bing_huo" {
    Properties {
        _node_1011 ("node_1011", 2D) = "white" {}
        _node_8395 ("node_8395", Color) = (0.5,0.5,0.5,1)
        _juese_liangdu ("juese_liangdu", Range(0, 1)) = 0.3333333
        _tex_bing ("tex_bing", 2D) = "white" {}
        _shuihuojiaohuan ("shuihuojiaohuan", Range(0, 1)) = 0
        _bing_yanse ("bing_yanse", Color) = (0.5,0.5,0.5,1)
        _waifaguang_liangdu ("waifaguang_liangdu", Range(0, 5)) = 1
        _waifaguang_yase ("waifaguang_yase", Color) = (0.5,0.5,0.5,1)
        _tex_huoyan ("tex_huoyan", 2D) = "white" {}
        _huoyan_yanse ("huoyan_yanse", Color) = (0.5,0.5,0.5,1)
        [MaterialToggle] _binghuoqiehuan ("binghuoqiehuan", Float ) = 0
        _huoyanliudong ("huoyanliudong", Range(0, 1)) = 0
        _node_1425 ("node_1425", Float ) = 0
        _node_3259 ("node_3259", Float ) = 0.5
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _node_1011; uniform float4 _node_1011_ST;
            uniform float4 _node_8395;
            uniform float _juese_liangdu;
            uniform sampler2D _tex_bing; uniform float4 _tex_bing_ST;
            uniform float _shuihuojiaohuan;
            uniform float4 _bing_yanse;
            uniform float _waifaguang_liangdu;
            uniform float4 _waifaguang_yase;
            uniform sampler2D _tex_huoyan; uniform float4 _tex_huoyan_ST;
            uniform float4 _huoyan_yanse;
            uniform fixed _binghuoqiehuan;
            uniform float _huoyanliudong;
            uniform float _node_1425;
            uniform float _node_3259;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 _node_1011_var = tex2D(_node_1011,TRANSFORM_TEX(i.uv0, _node_1011));
                clip(_node_1011_var.a - 0.5);
////// Lighting:
////// Emissive:
                float4 _tex_bing_var = tex2D(_tex_bing,TRANSFORM_TEX(i.uv0, _tex_bing));
                float4 node_1270 = _Time + _TimeEditor;
                float2 node_7128 = (i.uv0+(node_1270.g*clamp(_huoyanliudong,_node_1425,_node_3259))*float2(0,1));
                float4 _tex_huoyan_var = tex2D(_tex_huoyan,TRANSFORM_TEX(node_7128, _tex_huoyan));
                float3 emissive = lerp(((_juese_liangdu*5.0)*(_node_1011_var.rgb*_node_8395.rgb*_node_8395.a)),(lerp( (_tex_bing_var.rgb*_bing_yanse.rgb*_bing_yanse.a), (_tex_huoyan_var.rgb*_huoyan_yanse.rgb*_huoyan_yanse.a), _binghuoqiehuan )+(pow(1.0-max(0,dot(i.normalDir, viewDirection)),_waifaguang_liangdu)*_waifaguang_yase.rgb)),_shuihuojiaohuan);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
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
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _node_1011; uniform float4 _node_1011_ST;
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
                float4 _node_1011_var = tex2D(_node_1011,TRANSFORM_TEX(i.uv0, _node_1011));
                clip(_node_1011_var.a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
