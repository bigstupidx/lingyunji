// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:2,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:7952,x:32719,y:32712,varname:node_7952,prsc:2|emission-7528-OUT,alpha-9718-OUT;n:type:ShaderForge.SFN_Tex2d,id:2260,x:31557,y:33115,ptovrint:False,ptlb:4,ptin:_4,varname:node_2260,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:83a844108ca51c645aab07b81b04e992,ntxv:0,isnm:False|UVIN-841-OUT;n:type:ShaderForge.SFN_Tex2d,id:2980,x:31062,y:32967,ptovrint:False,ptlb:2,ptin:_2,varname:node_2980,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:12dfff9a82f56f3448193ca777dece57,ntxv:0,isnm:False|UVIN-6488-UVOUT;n:type:ShaderForge.SFN_Panner,id:6488,x:30741,y:32842,varname:node_6488,prsc:2,spu:0,spv:0.1|UVIN-195-OUT,DIST-7424-OUT;n:type:ShaderForge.SFN_TexCoord,id:2216,x:30342,y:32922,varname:node_2216,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:2829,x:31083,y:33317,ptovrint:False,ptlb:3,ptin:_3,varname:node_2829,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:f781c1490a9c1a147be5d51c26cd6b93,ntxv:0,isnm:False|UVIN-8500-UVOUT;n:type:ShaderForge.SFN_Panner,id:8500,x:30887,y:33337,varname:node_8500,prsc:2,spu:0,spv:0.1|UVIN-692-UVOUT,DIST-3985-OUT;n:type:ShaderForge.SFN_TexCoord,id:692,x:30672,y:33254,varname:node_692,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:620,x:31239,y:33219,varname:node_620,prsc:2|A-2980-R,B-2829-R;n:type:ShaderForge.SFN_Add,id:841,x:31358,y:33102,varname:node_841,prsc:2|A-8018-UVOUT,B-620-OUT;n:type:ShaderForge.SFN_TexCoord,id:8018,x:31156,y:32688,varname:node_8018,prsc:2,uv:0;n:type:ShaderForge.SFN_OneMinus,id:6986,x:31824,y:32917,varname:node_6986,prsc:2|IN-2260-R;n:type:ShaderForge.SFN_Multiply,id:9718,x:32034,y:32698,varname:node_9718,prsc:2|A-3763-R,B-6986-OUT,C-3882-A;n:type:ShaderForge.SFN_Tex2d,id:3763,x:31598,y:32402,ptovrint:False,ptlb:1,ptin:_1,varname:node_3763,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:073a5f1e4f0abd541b1c9f09887a9edb,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:7528,x:32141,y:32849,varname:node_7528,prsc:2|A-9718-OUT,B-3882-RGB;n:type:ShaderForge.SFN_Color,id:3882,x:31883,y:33123,ptovrint:False,ptlb:1color,ptin:_1color,varname:node_3882,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.6029412,c2:0.6549695,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:195,x:30579,y:32793,varname:node_195,prsc:2|A-2303-OUT,B-2216-UVOUT;n:type:ShaderForge.SFN_Vector2,id:2303,x:30373,y:32811,varname:node_2303,prsc:2,v1:2,v2:1;n:type:ShaderForge.SFN_Multiply,id:7424,x:30874,y:33015,varname:node_7424,prsc:2|A-7613-T,B-5085-OUT;n:type:ShaderForge.SFN_Time,id:7613,x:30537,y:33031,varname:node_7613,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:5085,x:30726,y:33182,ptovrint:False,ptlb:2 sleep,ptin:_2sleep,varname:node_5085,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:8040,x:30628,y:33637,ptovrint:False,ptlb:1 sleep,ptin:_1sleep,varname:_2sleep_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:3985,x:30646,y:33463,varname:node_3985,prsc:2|A-1014-T,B-8040-OUT;n:type:ShaderForge.SFN_Time,id:1014,x:30350,y:33516,varname:node_1014,prsc:2;proporder:2260-2980-2829-3763-3882-5085-8040;pass:END;sub:END;*/

Shader "HJY/jianqi" {
    Properties {
        _4 ("4", 2D) = "white" {}
        _2 ("2", 2D) = "white" {}
        _3 ("3", 2D) = "white" {}
        _1 ("1", 2D) = "white" {}
        _1color ("1color", Color) = (0.6029412,0.6549695,1,1)
        _2sleep ("2 sleep", Float ) = 1
        _1sleep ("1 sleep", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        Pass {
            Name "FORWARD"
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
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _4; uniform float4 _4_ST;
            uniform sampler2D _2; uniform float4 _2_ST;
            uniform sampler2D _3; uniform float4 _3_ST;
            uniform sampler2D _1; uniform float4 _1_ST;
            uniform float4 _1color;
            uniform float _2sleep;
            uniform float _1sleep;
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
////// Lighting:
////// Emissive:
                float4 _1_var = tex2D(_1,TRANSFORM_TEX(i.uv0, _1));
                float4 node_7613 = _Time + _TimeEditor;
                float2 node_6488 = ((float2(2,1)*i.uv0)+(node_7613.g*_2sleep)*float2(0,0.1));
                float4 _2_var = tex2D(_2,TRANSFORM_TEX(node_6488, _2));
                float4 node_1014 = _Time + _TimeEditor;
                float2 node_8500 = (i.uv0+(node_1014.g*_1sleep)*float2(0,0.1));
                float4 _3_var = tex2D(_3,TRANSFORM_TEX(node_8500, _3));
                float2 node_841 = (i.uv0+(_2_var.r*_3_var.r));
                float4 _4_var = tex2D(_4,TRANSFORM_TEX(node_841, _4));
                float node_9718 = (_1_var.r*(1.0 - _4_var.r)*_1color.a);
                float3 emissive = (node_9718*_1color.rgb);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,node_9718);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
