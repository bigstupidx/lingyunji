// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:Mobile/Diffuse,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4795,x:33527,y:32917,varname:node_4795,prsc:2|emission-1279-OUT;n:type:ShaderForge.SFN_Color,id:8113,x:32451,y:33434,ptovrint:False,ptlb:Mian_color,ptin:_Mian_color,varname:node_4392,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Panner,id:2385,x:31735,y:32364,varname:node_2385,prsc:2,spu:0,spv:0.2|UVIN-683-UVOUT,DIST-1206-OUT;n:type:ShaderForge.SFN_Multiply,id:613,x:32851,y:32777,varname:node_613,prsc:2|A-3810-OUT,B-6803-OUT;n:type:ShaderForge.SFN_Vector1,id:3810,x:32673,y:32652,varname:node_3810,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Tex2d,id:6603,x:32233,y:32872,ptovrint:False,ptlb:Mask_map,ptin:_Mask_map,varname:node_2377,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:4f4519c8dadf0a5478a89bac42233550,ntxv:2,isnm:False|UVIN-8607-OUT;n:type:ShaderForge.SFN_TexCoord,id:7622,x:31510,y:32933,cmnt:uv0,varname:node_7622,prsc:2,uv:1;n:type:ShaderForge.SFN_ValueProperty,id:3836,x:32871,y:32683,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_9304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:1279,x:33316,y:32869,varname:node_1279,prsc:2|A-3836-OUT,B-613-OUT,C-8113-RGB;n:type:ShaderForge.SFN_TexCoord,id:683,x:31331,y:32396,cmnt:uv1,varname:node_683,prsc:2,uv:0;n:type:ShaderForge.SFN_Time,id:4776,x:31238,y:32702,varname:node_4776,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:9351,x:31314,y:32904,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_9351,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:1206,x:31490,y:32665,varname:node_1206,prsc:2|A-4776-T,B-9351-OUT;n:type:ShaderForge.SFN_Lerp,id:8607,x:31977,y:32889,varname:node_8607,prsc:2|A-9667-R,B-7622-UVOUT,T-8844-OUT;n:type:ShaderForge.SFN_Tex2d,id:9667,x:32114,y:32314,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_9667,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2385-UVOUT;n:type:ShaderForge.SFN_Slider,id:8844,x:31643,y:33184,ptovrint:False,ptlb:Noise_int,ptin:_Noise_int,varname:node_8844,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Tex2d,id:9886,x:32322,y:32707,ptovrint:False,ptlb:M_mask,ptin:_M_mask,varname:node_9886,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8607-OUT;n:type:ShaderForge.SFN_Multiply,id:6570,x:32562,y:32884,varname:node_6570,prsc:2|A-9886-RGB,B-6603-RGB;n:type:ShaderForge.SFN_Tex2d,id:1099,x:32292,y:33177,ptovrint:False,ptlb:M_noise,ptin:_M_noise,varname:node_1099,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4349-UVOUT;n:type:ShaderForge.SFN_Multiply,id:6803,x:32743,y:32981,varname:node_6803,prsc:2|A-6570-OUT,B-1099-RGB;n:type:ShaderForge.SFN_Panner,id:4349,x:31950,y:33056,varname:node_4349,prsc:2,spu:0,spv:0.1|UVIN-2385-UVOUT;proporder:8113-6603-3836-9351-9667-8844-9886-1099;pass:END;sub:END;*/

Shader "my shader/vfx/UVoffset_animB" {
    Properties {
        _Mian_color ("Mian_color", Color) = (0.5,0.5,0.5,1)
        _Mask_map ("Mask_map", 2D) = "black" {}
        _Alpha ("Alpha", Float ) = 0
        _Speed ("Speed", Float ) = 0
        _Noise ("Noise", 2D) = "white" {}
        _Noise_int ("Noise_int", Range(0, 1)) = 0
        _M_mask ("M_mask", 2D) = "white" {}
        _M_noise ("M_noise", 2D) = "white" {}
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
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 2.0
            uniform float4 _TimeEditor;
            uniform float4 _Mian_color;
            uniform sampler2D _Mask_map; uniform float4 _Mask_map_ST;
            uniform float _Alpha;
            uniform float _Speed;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Noise_int;
            uniform sampler2D _M_mask; uniform float4 _M_mask_ST;
            uniform sampler2D _M_noise; uniform float4 _M_noise_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_4776 = _Time + _TimeEditor;
                float2 node_2385 = (i.uv0+(node_4776.g*_Speed)*float2(0,0.2));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_2385, _Noise));
                float2 node_8607 = lerp(float2(_Noise_var.r,_Noise_var.r),i.uv1,_Noise_int);
                float4 _M_mask_var = tex2D(_M_mask,TRANSFORM_TEX(node_8607, _M_mask));
                float4 _Mask_map_var = tex2D(_Mask_map,TRANSFORM_TEX(node_8607, _Mask_map));
                float4 node_6896 = _Time + _TimeEditor;
                float2 node_4349 = (node_2385+node_6896.g*float2(0,0.1));
                float4 _M_noise_var = tex2D(_M_noise,TRANSFORM_TEX(node_4349, _M_noise));
                float3 emissive = (_Alpha*(1.5*((_M_mask_var.rgb*_Mask_map_var.rgb)*_M_noise_var.rgb))*_Mian_color.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
