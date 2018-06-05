// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:8854,x:32636,y:32716,varname:node_8854,prsc:2|emission-743-OUT,clip-4684-OUT;n:type:ShaderForge.SFN_Tex2d,id:361,x:32046,y:32618,ptovrint:False,ptlb:node_361,ptin:_node_361,varname:node_361,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:afcc0f0a319bf2e4ebc60558c21eddce,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:9188,x:32046,y:32818,ptovrint:False,ptlb:node_9188,ptin:_node_9188,varname:node_9188,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5019608,c2:0.5019608,c3:0.5019608,c4:0.5019608;n:type:ShaderForge.SFN_Multiply,id:2267,x:32287,y:32750,varname:node_2267,prsc:2|A-361-RGB,B-9188-RGB;n:type:ShaderForge.SFN_Multiply,id:4684,x:32388,y:32964,varname:node_4684,prsc:2|A-6609-RGB,B-2875-OUT;n:type:ShaderForge.SFN_Slider,id:2875,x:31994,y:33239,ptovrint:False,ptlb:node_2875,ptin:_node_2875,varname:node_2875,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Tex2d,id:6609,x:32092,y:33023,ptovrint:False,ptlb:node_6609,ptin:_node_6609,varname:node_6609,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ValueProperty,id:3709,x:32271,y:32618,ptovrint:False,ptlb:node_3709,ptin:_node_3709,varname:node_3709,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_Multiply,id:743,x:32448,y:32680,varname:node_743,prsc:2|A-3709-OUT,B-2267-OUT;proporder:361-9188-2875-6609-3709;pass:END;sub:END;*/

Shader "Shader Forge/rongjie" {
    Properties {
        _node_361 ("node_361", 2D) = "white" {}
        _node_9188 ("node_9188", Color) = (0.5019608,0.5019608,0.5019608,0.5019608)
        _node_2875 ("node_2875", Range(0, 1)) = 1
        _node_6609 ("node_6609", 2D) = "white" {}
        _node_3709 ("node_3709", Float ) = 3
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
            Blend One One
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _node_361; uniform float4 _node_361_ST;
            uniform float4 _node_9188;
            uniform float _node_2875;
            uniform sampler2D _node_6609; uniform float4 _node_6609_ST;
            uniform float _node_3709;
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
                float4 _node_6609_var = tex2D(_node_6609,TRANSFORM_TEX(i.uv0, _node_6609));
                clip((_node_6609_var.rgb*_node_2875) - 0.5);
////// Lighting:
////// Emissive:
                float4 _node_361_var = tex2D(_node_361,TRANSFORM_TEX(i.uv0, _node_361));
                float3 emissive = (_node_3709*(_node_361_var.rgb*_node_9188.rgb));
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
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float _node_2875;
            uniform sampler2D _node_6609; uniform float4 _node_6609_ST;
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
                float4 _node_6609_var = tex2D(_node_6609,TRANSFORM_TEX(i.uv0, _node_6609));
                clip((_node_6609_var.rgb*_node_2875) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
