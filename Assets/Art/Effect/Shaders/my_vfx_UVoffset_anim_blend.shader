// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.35 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.35;sub:START;pass:START;ps:flbk:Mobile/Diffuse,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5198783,fgcg:0.375865,fgcb:0.6470588,fgca:1,fgde:0.01,fgrn:13.7,fgrf:320.5,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4795,x:33527,y:32917,varname:node_4795,prsc:2|emission-1279-OUT,alpha-402-OUT;n:type:ShaderForge.SFN_Tex2d,id:5565,x:32373,y:32743,ptovrint:False,ptlb:Offset_map,ptin:_Offset_map,varname:node_2254,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False|UVIN-8607-OUT;n:type:ShaderForge.SFN_Color,id:8113,x:32162,y:33267,ptovrint:False,ptlb:Mian_color,ptin:_Mian_color,varname:node_4392,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:7549,x:32710,y:32808,varname:node_7549,prsc:2|A-5565-RGB,B-6603-RGB;n:type:ShaderForge.SFN_Panner,id:2385,x:31674,y:32580,varname:node_2385,prsc:2,spu:0,spv:0.2|UVIN-7622-UVOUT,DIST-1206-OUT;n:type:ShaderForge.SFN_Multiply,id:613,x:32997,y:32805,varname:node_613,prsc:2|A-3810-OUT,B-7549-OUT,C-9282-OUT;n:type:ShaderForge.SFN_Vector1,id:3810,x:32682,y:32716,varname:node_3810,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Tex2d,id:6603,x:32310,y:33061,ptovrint:False,ptlb:Mask_map,ptin:_Mask_map,varname:node_2377,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False|UVIN-6547-OUT;n:type:ShaderForge.SFN_TexCoord,id:7622,x:31371,y:32728,cmnt:uv0,varname:node_7622,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:1279,x:33247,y:32896,varname:node_1279,prsc:2|A-1702-OUT,B-613-OUT,C-7797-RGB;n:type:ShaderForge.SFN_TexCoord,id:683,x:31864,y:32921,cmnt:uv1,varname:node_683,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_Time,id:4776,x:31219,y:32882,varname:node_4776,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:9351,x:31237,y:33082,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_9351,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:1206,x:31403,y:32882,varname:node_1206,prsc:2|A-4776-T,B-9351-OUT;n:type:ShaderForge.SFN_Lerp,id:8607,x:32191,y:32877,varname:node_8607,prsc:2|A-9667-R,B-7622-UVOUT,T-8844-OUT;n:type:ShaderForge.SFN_Tex2d,id:9667,x:32033,y:32753,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_9667,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2385-UVOUT;n:type:ShaderForge.SFN_Slider,id:8844,x:31805,y:33418,ptovrint:False,ptlb:Noise_int,ptin:_Noise_int,varname:node_8844,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:402,x:33285,y:33187,varname:node_402,prsc:2|A-5565-A,B-6603-A,C-8113-A,D-7797-A;n:type:ShaderForge.SFN_Color,id:2180,x:32143,y:33454,ptovrint:False,ptlb:Mu_color,ptin:_Mu_color,varname:_Mian_color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:1702,x:32544,y:33524,varname:node_1702,prsc:2|A-8113-RGB,B-2180-RGB,T-2375-RGB;n:type:ShaderForge.SFN_Tex2d,id:2375,x:32143,y:33622,ptovrint:False,ptlb:Multiiy_map,ptin:_Multiiy_map,varname:node_2375,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:9282,x:32385,y:33239,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:node_9282,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:5,max:5;n:type:ShaderForge.SFN_VertexColor,id:7797,x:32982,y:33340,varname:node_7797,prsc:2;n:type:ShaderForge.SFN_SwitchProperty,id:6547,x:31942,y:33125,ptovrint:False,ptlb:aoto,ptin:_aoto,varname:node_6547,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-683-UVOUT,B-3293-UVOUT;n:type:ShaderForge.SFN_Panner,id:3293,x:31528,y:33119,varname:node_3293,prsc:2,spu:0,spv:0.2|UVIN-8715-UVOUT,DIST-995-OUT;n:type:ShaderForge.SFN_TexCoord,id:8715,x:31225,y:33267,cmnt:uv0,varname:node_8715,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Time,id:7973,x:31073,y:33421,varname:node_7973,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:9445,x:31091,y:33621,ptovrint:False,ptlb:offset_speed,ptin:_offset_speed,varname:_Speed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:995,x:31257,y:33421,varname:node_995,prsc:2|A-7973-T,B-9445-OUT;proporder:5565-8113-6603-9351-9667-8844-2180-2375-9282-6547-9445;pass:END;sub:END;*/

Shader "my shader/vfx/UVoffset_anim_blend" {
    Properties {
        _Offset_map ("Offset_map", 2D) = "black" {}
        _Mian_color ("Mian_color", Color) = (0.5,0.5,0.5,1)
        _Mask_map ("Mask_map", 2D) = "black" {}
        _Speed ("Speed", Float ) = 0
        _Noise ("Noise", 2D) = "white" {}
        _Noise_int ("Noise_int", Range(0, 1)) = 0
        _Mu_color ("Mu_color", Color) = (0.5,0.5,0.5,1)
        _Multiiy_map ("Multiiy_map", 2D) = "white" {}
        _Emission ("Emission", Range(-5, 5)) = 5
        [MaterialToggle] _aoto ("aoto", Float ) = 0
        _offset_speed ("offset_speed", Float ) = 0
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
            uniform sampler2D _Offset_map; uniform float4 _Offset_map_ST;
            uniform float4 _Mian_color;
            uniform sampler2D _Mask_map; uniform float4 _Mask_map_ST;
            uniform float _Speed;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Noise_int;
            uniform float4 _Mu_color;
            uniform sampler2D _Multiiy_map; uniform float4 _Multiiy_map_ST;
            uniform float _Emission;
            uniform fixed _aoto;
            uniform float _offset_speed;
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
                float4 _Multiiy_map_var = tex2D(_Multiiy_map,TRANSFORM_TEX(i.uv0, _Multiiy_map));
                float4 node_4776 = _Time + _TimeEditor;
                float2 node_2385 = (i.uv0+(node_4776.g*_Speed)*float2(0,0.2));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_2385, _Noise));
                float2 node_8607 = lerp(float2(_Noise_var.r,_Noise_var.r),i.uv0,_Noise_int);
                float4 _Offset_map_var = tex2D(_Offset_map,TRANSFORM_TEX(node_8607, _Offset_map));
                float4 node_7973 = _Time + _TimeEditor;
                float2 _aoto_var = lerp( i.uv1, (i.uv0+(node_7973.g*_offset_speed)*float2(0,0.2)), _aoto );
                float4 _Mask_map_var = tex2D(_Mask_map,TRANSFORM_TEX(_aoto_var, _Mask_map));
                float3 emissive = (lerp(_Mian_color.rgb,_Mu_color.rgb,_Multiiy_map_var.rgb)*(1.5*(_Offset_map_var.rgb*_Mask_map_var.rgb)*_Emission)*i.vertexColor.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,(_Offset_map_var.a*_Mask_map_var.a*_Mian_color.a*i.vertexColor.a));
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal n3ds wiiu 
            #pragma target 2.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
