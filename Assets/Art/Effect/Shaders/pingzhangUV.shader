// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.05 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.05;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:2,bsrc:0,bdst:0,culm:2,dpts:2,wrdp:False,dith:0,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:7908,x:32789,y:32755,varname:node_7908,prsc:2|emission-9024-OUT;n:type:ShaderForge.SFN_Tex2d,id:6271,x:31785,y:32705,ptovrint:False,ptlb:Diffuse_Tex2,ptin:_Diffuse_Tex2,varname:node_6271,prsc:2,tex:006d598cc33b8ea45873f90e83492682,ntxv:0,isnm:False|UVIN-8686-UVOUT;n:type:ShaderForge.SFN_Panner,id:8686,x:31622,y:32694,varname:node_8686,prsc:2,spu:0.2,spv:-0.15;n:type:ShaderForge.SFN_Panner,id:1876,x:31565,y:32996,varname:node_1876,prsc:2,spu:0,spv:0.1;n:type:ShaderForge.SFN_Tex2d,id:9928,x:31786,y:32996,ptovrint:False,ptlb:Diffuse_Tex,ptin:_Diffuse_Tex,varname:node_9928,prsc:2,tex:0d178d668b46f66498f8041fc187b2b6,ntxv:0,isnm:False|UVIN-1876-UVOUT;n:type:ShaderForge.SFN_Multiply,id:9391,x:32120,y:32777,varname:node_9391,prsc:2|A-5135-OUT,B-9928-RGB;n:type:ShaderForge.SFN_Add,id:7493,x:32145,y:33024,varname:node_7493,prsc:2|A-6271-RGB,B-9928-RGB;n:type:ShaderForge.SFN_Multiply,id:4228,x:32359,y:32749,varname:node_4228,prsc:2|A-948-OUT,B-9391-OUT;n:type:ShaderForge.SFN_Multiply,id:7101,x:32395,y:33034,varname:node_7101,prsc:2|A-9715-RGB,B-7493-OUT;n:type:ShaderForge.SFN_Multiply,id:9024,x:32589,y:32837,varname:node_9024,prsc:2|A-7101-OUT,B-4228-OUT;n:type:ShaderForge.SFN_ValueProperty,id:948,x:32093,y:32685,ptovrint:False,ptlb:node_948,ptin:_node_948,varname:node_948,prsc:2,glob:False,v1:0;n:type:ShaderForge.SFN_Color,id:9715,x:32226,y:33177,ptovrint:False,ptlb:node_9715,ptin:_node_9715,varname:node_9715,prsc:2,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Add,id:5135,x:32021,y:32470,varname:node_5135,prsc:2|A-6977-RGB,B-6271-RGB;n:type:ShaderForge.SFN_Panner,id:7868,x:31604,y:32415,varname:node_7868,prsc:2,spu:-0.15,spv:0.2;n:type:ShaderForge.SFN_Tex2d,id:6977,x:31791,y:32434,ptovrint:False,ptlb:Diffuse_Tex1,ptin:_Diffuse_Tex1,varname:_node_6271_copy,prsc:2,tex:006d598cc33b8ea45873f90e83492682,ntxv:0,isnm:False|UVIN-7868-UVOUT;proporder:6271-9928-948-9715-6977;pass:END;sub:END;*/

Shader "Star_Shader/FX/PingzhangqiangUV" {
    Properties {
        _Diffuse_Tex2 ("Diffuse_Tex2", 2D) = "white" {}
        _Diffuse_Tex ("Diffuse_Tex", 2D) = "white" {}
        _node_948 ("node_948", Float ) = 0
        _node_9715 ("node_9715", Color) = (0.5,0.5,0.5,1)
        _Diffuse_Tex1 ("Diffuse_Tex1", 2D) = "white" {}
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
            uniform sampler2D _Diffuse_Tex2; uniform float4 _Diffuse_Tex2_ST;
            uniform sampler2D _Diffuse_Tex; uniform float4 _Diffuse_Tex_ST;
            uniform float _node_948;
            uniform float4 _node_9715;
            uniform sampler2D _Diffuse_Tex1; uniform float4 _Diffuse_Tex1_ST;
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
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
////// Emissive:
                float4 node_6348 = _Time + _TimeEditor;
                float2 node_8686 = (i.uv0+node_6348.g*float2(0.2,-0.15));
                float4 _Diffuse_Tex2_var = tex2D(_Diffuse_Tex2,TRANSFORM_TEX(node_8686, _Diffuse_Tex2));
                float2 node_1876 = (i.uv0+node_6348.g*float2(0,0.1));
                float4 _Diffuse_Tex_var = tex2D(_Diffuse_Tex,TRANSFORM_TEX(node_1876, _Diffuse_Tex));
                float2 node_7868 = (i.uv0+node_6348.g*float2(-0.15,0.2));
                float4 _Diffuse_Tex1_var = tex2D(_Diffuse_Tex1,TRANSFORM_TEX(node_7868, _Diffuse_Tex1));
                float3 emissive = ((_node_9715.rgb*(_Diffuse_Tex2_var.rgb+_Diffuse_Tex_var.rgb))*(_node_948*((_Diffuse_Tex1_var.rgb+_Diffuse_Tex2_var.rgb)*_Diffuse_Tex_var.rgb)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
