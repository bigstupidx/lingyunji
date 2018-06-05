// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:Mobile/Diffuse,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:32719,y:32712,varname:node_4013,prsc:2|emission-6327-OUT,alpha-9501-OUT,clip-9501-OUT;n:type:ShaderForge.SFN_Tex2d,id:8162,x:30233,y:32517,ptovrint:False,ptlb:T2,ptin:_T2,varname:node_2254,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:70387129d7d1ebf43a33d07b53c858ae,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Color,id:9018,x:30230,y:32004,ptovrint:False,ptlb:T1_color,ptin:_T1_color,varname:node_4392,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Panner,id:367,x:29921,y:32087,varname:node_367,prsc:2,spu:0.3,spv:0|UVIN-7795-UVOUT;n:type:ShaderForge.SFN_Multiply,id:4623,x:31386,y:32334,varname:node_4623,prsc:2|A-211-OUT,B-5120-OUT;n:type:ShaderForge.SFN_Vector1,id:211,x:31187,y:32226,varname:node_211,prsc:2,v1:3;n:type:ShaderForge.SFN_Tex2d,id:8031,x:31080,y:32926,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_2377,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:4f4519c8dadf0a5478a89bac42233550,ntxv:2,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:7795,x:29723,y:32087,varname:node_7795,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:3964,x:30206,y:32291,ptovrint:False,ptlb:T1,ptin:_T1,varname:node_9846,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3266,x:30771,y:32424,varname:node_3266,prsc:2|A-1335-OUT,B-8106-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8188,x:30808,y:32323,ptovrint:False,ptlb:glow,ptin:_glow,varname:node_7929,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:5120,x:30963,y:32326,varname:node_5120,prsc:2|A-8188-OUT,B-3266-OUT;n:type:ShaderForge.SFN_Panner,id:2215,x:29902,y:31922,varname:node_2215,prsc:2,spu:0.1,spv:0|UVIN-1139-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:1139,x:29704,y:31922,varname:node_1139,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:5382,x:32163,y:32913,varname:node_5382,prsc:2|A-3989-OUT,B-2999-OUT;n:type:ShaderForge.SFN_Multiply,id:3989,x:31402,y:32702,varname:node_3989,prsc:2|A-3964-A,B-8162-A;n:type:ShaderForge.SFN_Multiply,id:2999,x:31681,y:33046,varname:node_2999,prsc:2|A-8031-A,B-1395-OUT;n:type:ShaderForge.SFN_Slider,id:1395,x:31242,y:33140,ptovrint:False,ptlb:msk_size,ptin:_msk_size,varname:node_2755,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Multiply,id:6327,x:32039,y:32740,varname:node_6327,prsc:2|A-4623-OUT,B-8031-RGB;n:type:ShaderForge.SFN_Multiply,id:1335,x:30526,y:32155,varname:node_1335,prsc:2|A-9018-RGB,B-3964-RGB;n:type:ShaderForge.SFN_Color,id:7938,x:30186,y:32742,ptovrint:False,ptlb:T2_color,ptin:_T2_color,varname:node_3453,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:8106,x:30571,y:32685,varname:node_8106,prsc:2|A-8162-RGB,B-7938-RGB;n:type:ShaderForge.SFN_Multiply,id:9501,x:32487,y:32998,varname:node_9501,prsc:2|A-5382-OUT,B-1610-OUT;n:type:ShaderForge.SFN_Slider,id:1610,x:32109,y:33030,ptovrint:False,ptlb:D,ptin:_D,varname:node_1610,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;proporder:8162-9018-8031-3964-8188-7938-1395-1610;pass:END;sub:END;*/

Shader "my shader/vfx/alphatest_anim_add" {
    Properties {
        _T2 ("T2", 2D) = "black" {}
        _T1_color ("T1_color", Color) = (0.5,0.5,0.5,1)
        _Mask ("Mask", 2D) = "black" {}
        _T1 ("T1", 2D) = "white" {}
        _glow ("glow", Float ) = 0
        _T2_color ("T2_color", Color) = (0.5,0.5,0.5,1)
        _msk_size ("msk_size", Range(0, 5)) = 0
        _D ("D", Range(0, 5)) = 0
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
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 2.0
            uniform sampler2D _T2; uniform float4 _T2_ST;
            uniform float4 _T1_color;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform sampler2D _T1; uniform float4 _T1_ST;
            uniform float _glow;
            uniform float _msk_size;
            uniform float4 _T2_color;
            uniform float _D;
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
                float node_9501 = (((_T1_var.a*_T2_var.a)*(_Mask_var.a*_msk_size))*_D);
                clip(node_9501 - 0.5);
////// Lighting:
////// Emissive:
                float3 emissive = ((3.0*(_glow*((_T1_color.rgb*_T1_var.rgb)*(_T2_var.rgb*_T2_color.rgb))))*_Mask_var.rgb);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,node_9501);
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
            #pragma target 2.0
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
                float node_9501 = (((_T1_var.a*_T2_var.a)*(_Mask_var.a*_msk_size))*_D);
                clip(node_9501 - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
