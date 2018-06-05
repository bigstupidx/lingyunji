// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.7464632,fgcg:0.7065312,fgcb:0.8897059,fgca:1,fgde:0.01,fgrn:24,fgrf:225.01,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:9691,x:32838,y:32709,varname:node_9691,prsc:2|emission-2897-OUT,alpha-2651-OUT;n:type:ShaderForge.SFN_Tex2d,id:1035,x:31789,y:32473,ptovrint:False,ptlb:Texture_1,ptin:_Texture_1,varname:node_1035,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:792560af6171eb645ac47c090d0e6e51,ntxv:3,isnm:False|UVIN-3748-OUT;n:type:ShaderForge.SFN_Tex2d,id:2995,x:32078,y:33498,ptovrint:False,ptlb:Texture_2,ptin:_Texture_2,varname:node_2995,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3bdae506da5b2a940a8a85b016cbd1bd,ntxv:0,isnm:False|UVIN-8313-OUT;n:type:ShaderForge.SFN_Tex2d,id:2250,x:30978,y:32345,ptovrint:False,ptlb:Noise_1,ptin:_Noise_1,varname:node_2250,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1a4c68f1f7e7db540abc6591a35676ca,ntxv:0,isnm:False|UVIN-6660-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:5992,x:30954,y:32669,ptovrint:False,ptlb:Noise_2,ptin:_Noise_2,varname:node_5992,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6b1bb81f02147f6419e047ede14a5038,ntxv:0,isnm:False|UVIN-4707-UVOUT;n:type:ShaderForge.SFN_Multiply,id:2309,x:32337,y:32719,varname:node_2309,prsc:2|A-6752-OUT,B-4173-OUT;n:type:ShaderForge.SFN_Vector1,id:9086,x:32094,y:32753,varname:node_9086,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:6752,x:32094,y:32606,varname:node_6752,prsc:2|A-1035-R,B-2994-RGB;n:type:ShaderForge.SFN_Multiply,id:9730,x:31161,y:32492,varname:node_9730,prsc:2|A-2250-R,B-5992-R;n:type:ShaderForge.SFN_TexCoord,id:2327,x:31360,y:32331,varname:node_2327,prsc:2,uv:1;n:type:ShaderForge.SFN_Add,id:3748,x:31573,y:32473,varname:node_3748,prsc:2|A-2327-UVOUT,B-2322-OUT;n:type:ShaderForge.SFN_Multiply,id:2322,x:31388,y:32545,varname:node_2322,prsc:2|A-9730-OUT,B-8466-OUT;n:type:ShaderForge.SFN_TexCoord,id:1751,x:30335,y:32328,varname:node_1751,prsc:2,uv:0;n:type:ShaderForge.SFN_Panner,id:6660,x:30687,y:32323,varname:node_6660,prsc:2,spu:-0.2,spv:0|UVIN-1751-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:2003,x:30333,y:32629,varname:node_2003,prsc:2,uv:0;n:type:ShaderForge.SFN_Panner,id:4707,x:30666,y:32635,varname:node_4707,prsc:2,spu:0,spv:0.5|UVIN-2003-UVOUT;n:type:ShaderForge.SFN_Multiply,id:6040,x:31135,y:33388,varname:node_6040,prsc:2|A-2250-R,B-5992-R;n:type:ShaderForge.SFN_TexCoord,id:3182,x:31464,y:33420,varname:node_3182,prsc:2,uv:0;n:type:ShaderForge.SFN_Add,id:8313,x:31768,y:33541,varname:node_8313,prsc:2|A-3182-UVOUT,B-4678-OUT;n:type:ShaderForge.SFN_Multiply,id:4678,x:31491,y:33727,varname:node_4678,prsc:2|A-6040-OUT,B-99-OUT;n:type:ShaderForge.SFN_If,id:2991,x:31942,y:32952,varname:node_2991,prsc:2|A-2392-A,B-695-A,GT-111-OUT,EQ-111-OUT,LT-3429-OUT;n:type:ShaderForge.SFN_VertexColor,id:2392,x:31273,y:32854,varname:node_2392,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:695,x:31273,y:33028,ptovrint:False,ptlb:Dissolve,ptin:_Dissolve,varname:node_695,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a057290370bcf624cbb26357645a1f12,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2651,x:32458,y:33512,varname:node_2651,prsc:2|A-2995-R,B-2991-OUT;n:type:ShaderForge.SFN_Multiply,id:2897,x:32583,y:32771,varname:node_2897,prsc:2|A-2309-OUT,B-2991-OUT;n:type:ShaderForge.SFN_Color,id:2994,x:31789,y:32664,ptovrint:False,ptlb:Texture_1_color,ptin:_Texture_1_color,varname:node_2994,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:99,x:31018,y:33690,ptovrint:False,ptlb:Texture_2_glow,ptin:_Texture_2_glow,varname:node_99,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Slider,id:8466,x:31116,y:32762,ptovrint:False,ptlb:Texture_1_glow,ptin:_Texture_1_glow,varname:node_8466,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2.580734,max:10;n:type:ShaderForge.SFN_Slider,id:4173,x:31963,y:32837,ptovrint:False,ptlb:glow,ptin:_glow,varname:node_4173,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3.590895,max:10;n:type:ShaderForge.SFN_Slider,id:111,x:31474,y:33202,ptovrint:False,ptlb:Dissolve_1,ptin:_Dissolve_1,varname:node_111,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Slider,id:3429,x:31474,y:33298,ptovrint:False,ptlb:Dissolve_2,ptin:_Dissolve_2,varname:node_3429,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;proporder:1035-2995-2250-5992-695-2994-99-8466-4173-111-3429;pass:END;sub:END;*/

Shader "my shader/vfx/my_shuimo" {
    Properties {
        _Texture_1 ("Texture_1", 2D) = "bump" {}
        _Texture_2 ("Texture_2", 2D) = "white" {}
        _Noise_1 ("Noise_1", 2D) = "white" {}
        _Noise_2 ("Noise_2", 2D) = "white" {}
        _Dissolve ("Dissolve", 2D) = "white" {}
        _Texture_1_color ("Texture_1_color", Color) = (0.5,0.5,0.5,1)
        _Texture_2_glow ("Texture_2_glow", Range(0, 10)) = 1
        _Texture_1_glow ("Texture_1_glow", Range(0, 10)) = 2.580734
        _glow ("glow", Range(0, 10)) = 3.590895
        _Dissolve_1 ("Dissolve_1", Range(0, 10)) = 1
        _Dissolve_2 ("Dissolve_2", Range(0, 10)) = 0
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
            #pragma only_renderers d3d9 d3d11 glcore gles n3ds wiiu 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Texture_1; uniform float4 _Texture_1_ST;
            uniform sampler2D _Texture_2; uniform float4 _Texture_2_ST;
            uniform sampler2D _Noise_1; uniform float4 _Noise_1_ST;
            uniform sampler2D _Noise_2; uniform float4 _Noise_2_ST;
            uniform sampler2D _Dissolve; uniform float4 _Dissolve_ST;
            uniform float4 _Texture_1_color;
            uniform float _Texture_2_glow;
            uniform float _Texture_1_glow;
            uniform float _glow;
            uniform float _Dissolve_1;
            uniform float _Dissolve_2;
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
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 node_7506 = _Time + _TimeEditor;
                float2 node_6660 = (i.uv0+node_7506.g*float2(-0.2,0));
                float4 _Noise_1_var = tex2D(_Noise_1,TRANSFORM_TEX(node_6660, _Noise_1));
                float2 node_4707 = (i.uv0+node_7506.g*float2(0,0.5));
                float4 _Noise_2_var = tex2D(_Noise_2,TRANSFORM_TEX(node_4707, _Noise_2));
                float2 node_3748 = (i.uv1+((_Noise_1_var.r*_Noise_2_var.r)*_Texture_1_glow));
                float4 _Texture_1_var = tex2D(_Texture_1,TRANSFORM_TEX(node_3748, _Texture_1));
                float4 _Dissolve_var = tex2D(_Dissolve,TRANSFORM_TEX(i.uv0, _Dissolve));
                float node_2991_if_leA = step(i.vertexColor.a,_Dissolve_var.a);
                float node_2991_if_leB = step(_Dissolve_var.a,i.vertexColor.a);
                float node_2991 = lerp((node_2991_if_leA*_Dissolve_2)+(node_2991_if_leB*_Dissolve_1),_Dissolve_1,node_2991_if_leA*node_2991_if_leB);
                float3 emissive = (((_Texture_1_var.r*_Texture_1_color.rgb)*_glow)*node_2991);
                float3 finalColor = emissive;
                float2 node_8313 = (i.uv0+((_Noise_1_var.r*_Noise_2_var.r)*_Texture_2_glow));
                float4 _Texture_2_var = tex2D(_Texture_2,TRANSFORM_TEX(node_8313, _Texture_2));
                fixed4 finalRGBA = fixed4(finalColor,(_Texture_2_var.r*node_2991));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
