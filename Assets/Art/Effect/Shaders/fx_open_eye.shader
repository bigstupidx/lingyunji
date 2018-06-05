// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:Mobile/Diffuse,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.684854,fgcg:0.597,fgcb:1,fgca:1,fgde:0.01,fgrn:12.2,fgrf:224.1,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:42419,y:35932,varname:node_3138,prsc:2|alpha-6610-OUT;n:type:ShaderForge.SFN_ComponentMask,id:8681,x:31124,y:32838,varname:node_8681,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1;n:type:ShaderForge.SFN_TexCoord,id:7673,x:40808,y:35824,varname:node_7673,prsc:2,uv:0;n:type:ShaderForge.SFN_Slider,id:342,x:40617,y:36042,ptovrint:False,ptlb:Num_wave,ptin:_Num_wave,varname:node_342,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2.651328,max:10;n:type:ShaderForge.SFN_Multiply,id:1432,x:41136,y:36121,varname:node_1432,prsc:2|A-6694-OUT,B-7822-OUT;n:type:ShaderForge.SFN_Slider,id:7822,x:40727,y:36311,ptovrint:False,ptlb:Wave_h,ptin:_Wave_h,varname:node_7822,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:10,max:10;n:type:ShaderForge.SFN_Add,id:4227,x:41547,y:36016,varname:node_4227,prsc:2|A-6995-OUT,B-1432-OUT;n:type:ShaderForge.SFN_RemapRange,id:6694,x:40953,y:36047,varname:node_6694,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-7673-V;n:type:ShaderForge.SFN_Multiply,id:1496,x:41028,y:35868,varname:node_1496,prsc:2|A-7673-U,B-342-OUT;n:type:ShaderForge.SFN_Sin,id:6995,x:41547,y:35824,varname:node_6995,prsc:2|IN-6816-OUT;n:type:ShaderForge.SFN_Add,id:6816,x:41196,y:35849,varname:node_6816,prsc:2|A-1496-OUT,B-1209-OUT;n:type:ShaderForge.SFN_Slider,id:1209,x:40892,y:35755,ptovrint:False,ptlb:X_Offset,ptin:_X_Offset,varname:node_1209,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:6.5,max:10;n:type:ShaderForge.SFN_Subtract,id:321,x:41547,y:36158,varname:node_321,prsc:2|A-6995-OUT,B-1432-OUT;n:type:ShaderForge.SFN_Multiply,id:7499,x:41728,y:36062,varname:node_7499,prsc:2|A-4227-OUT,B-321-OUT;n:type:ShaderForge.SFN_OneMinus,id:6610,x:42088,y:36173,varname:node_6610,prsc:2|IN-6984-OUT;n:type:ShaderForge.SFN_Add,id:6984,x:41748,y:36257,varname:node_6984,prsc:2|A-7499-OUT,B-1094-OUT;n:type:ShaderForge.SFN_Slider,id:1094,x:40917,y:36725,ptovrint:False,ptlb:Open,ptin:_Open,varname:node_1094,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:1,max:1;proporder:342-7822-1209-1094;pass:END;sub:END;*/

Shader "my shader/fx_open_eye" {
    Properties {
        _Num_wave ("Num_wave", Range(0, 10)) = 2.651328
        _Wave_h ("Wave_h", Range(0, 10)) = 10
        _X_Offset ("X_Offset", Range(0, 10)) = 6.5
        _Open ("Open", Range(-1, 1)) = 1
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
            #pragma only_renderers d3d9 d3d11 gles gles3 metal 
            #pragma target 2.0
            uniform float _Num_wave;
            uniform float _Wave_h;
            uniform float _X_Offset;
            uniform float _Open;
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
                float3 finalColor = 0;
                float node_6816 = ((i.uv0.r*_Num_wave)+_X_Offset);
                float node_6995 = sin(node_6816);
                float node_1432 = ((i.uv0.g*2.0+-1.0)*_Wave_h);
                return fixed4(finalColor,(1.0 - (((node_6995+node_1432)*(node_6995-node_1432))+_Open)));
            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
