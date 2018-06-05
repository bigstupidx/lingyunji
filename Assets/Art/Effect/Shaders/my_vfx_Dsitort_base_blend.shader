// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:Mobile/Diffuse,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4795,x:33558,y:32909,varname:node_4795,prsc:2|emission-3871-OUT,alpha-6810-OUT;n:type:ShaderForge.SFN_Tex2d,id:5565,x:32576,y:32832,ptovrint:False,ptlb:Main_map,ptin:_Main_map,varname:node_2254,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False|UVIN-8607-OUT;n:type:ShaderForge.SFN_TexCoord,id:7622,x:31808,y:33094,cmnt:uv0,varname:node_7622,prsc:2,uv:1;n:type:ShaderForge.SFN_Time,id:4776,x:31443,y:32924,varname:node_4776,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:9351,x:31461,y:33124,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_9351,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:1206,x:31621,y:32924,varname:node_1206,prsc:2|A-4776-T,B-9351-OUT;n:type:ShaderForge.SFN_Lerp,id:8607,x:32225,y:32911,varname:node_8607,prsc:2|A-9667-R,B-3592-OUT,T-8844-OUT;n:type:ShaderForge.SFN_Tex2d,id:9667,x:32033,y:32763,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_9667,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7469-UVOUT;n:type:ShaderForge.SFN_Slider,id:8844,x:31808,y:33290,ptovrint:False,ptlb:Noise_int,ptin:_Noise_int,varname:node_8844,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Panner,id:7469,x:31814,y:32718,varname:node_7469,prsc:2,spu:0,spv:0.1|UVIN-5759-UVOUT,DIST-1206-OUT;n:type:ShaderForge.SFN_TexCoord,id:5759,x:31456,y:32718,cmnt:uv0,varname:node_5759,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:3871,x:33204,y:32790,varname:node_3871,prsc:2|A-5565-RGB,B-5034-RGB,C-2440-RGB,D-2953-OUT,E-5565-A;n:type:ShaderForge.SFN_Color,id:5034,x:32442,y:33060,ptovrint:False,ptlb:Main_color,ptin:_Main_color,varname:node_5034,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_VertexColor,id:2440,x:32539,y:33352,varname:node_2440,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6810,x:33161,y:33226,varname:node_6810,prsc:2|A-5565-A,B-2440-A,C-5034-A;n:type:ShaderForge.SFN_SwitchProperty,id:3592,x:31999,y:33005,ptovrint:False,ptlb:UVS,ptin:_UVS,varname:node_3592,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-7622-UVOUT,B-5759-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:2953,x:32960,y:33069,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:node_2953,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:5034-5565-9351-9667-8844-3592-2953;pass:END;sub:END;*/

Shader "my shader/vfx/2UV_Distort_base_blend" {
    Properties {
        _Main_color ("Main_color", Color) = (0.5,0.5,0.5,1)
        _Main_map ("Main_map", 2D) = "black" {}
        _Speed ("Speed", Float ) = 0
        _Noise ("Noise", 2D) = "white" {}
        _Noise_int ("Noise_int", Range(0, 1)) = 0
        [MaterialToggle] _UVS ("UVS", Float ) = 0
        _Emission ("Emission", Float ) = 1
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
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal n3ds wiiu 
            #pragma target 2.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Main_map; uniform float4 _Main_map_ST;
            uniform float _Speed;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Noise_int;
            uniform float4 _Main_color;
            uniform fixed _UVS;
            uniform float _Emission;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 node_4776 = _Time + _TimeEditor;
                float2 node_7469 = (i.uv0+(node_4776.g*_Speed)*float2(0,0.1));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_7469, _Noise));
                float2 node_8607 = lerp(float2(_Noise_var.r,_Noise_var.r),lerp( i.uv1, i.uv0, _UVS ),_Noise_int);
                float4 _Main_map_var = tex2D(_Main_map,TRANSFORM_TEX(node_8607, _Main_map));
                float3 emissive = (_Main_map_var.rgb*_Main_color.rgb*i.vertexColor.rgb*_Emission*_Main_map_var.a);
                float3 finalColor = emissive;
                return fixed4(finalColor,(_Main_map_var.a*i.vertexColor.a*_Main_color.a));
            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
