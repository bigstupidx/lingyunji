// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.35 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.35;sub:START;pass:START;ps:flbk:Mobile/Diffuse,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.684854,fgcg:0.597,fgcb:1,fgca:1,fgde:0.01,fgrn:12.2,fgrf:224.1,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4795,x:33527,y:32917,varname:node_4795,prsc:2|emission-1279-OUT;n:type:ShaderForge.SFN_Tex2d,id:5565,x:32438,y:32792,ptovrint:False,ptlb:Offset_map,ptin:_Offset_map,varname:node_2254,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False|UVIN-8607-OUT;n:type:ShaderForge.SFN_Color,id:8113,x:32451,y:33434,ptovrint:False,ptlb:Mian_color,ptin:_Mian_color,varname:node_4392,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:7549,x:32682,y:32809,varname:node_7549,prsc:2|A-5565-RGB,B-6603-RGB;n:type:ShaderForge.SFN_Panner,id:2385,x:31916,y:32729,varname:node_2385,prsc:2,spu:0,spv:0.2|UVIN-7622-UVOUT,DIST-3759-OUT;n:type:ShaderForge.SFN_Multiply,id:613,x:32881,y:32809,varname:node_613,prsc:2|A-3836-OUT,B-7549-OUT;n:type:ShaderForge.SFN_Tex2d,id:6603,x:32378,y:33114,ptovrint:False,ptlb:Mask_map,ptin:_Mask_map,varname:node_2377,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False|UVIN-2385-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:7622,x:31484,y:32689,cmnt:uv0,varname:node_7622,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ValueProperty,id:3836,x:32871,y:32683,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_9304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:1279,x:33212,y:32962,varname:node_1279,prsc:2|A-613-OUT,B-5298-R,C-8113-RGB,D-2494-RGB;n:type:ShaderForge.SFN_Time,id:4776,x:31296,y:32958,varname:node_4776,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:9351,x:31314,y:33158,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_9351,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:1206,x:31518,y:33012,varname:node_1206,prsc:2|A-4776-T,B-9351-OUT;n:type:ShaderForge.SFN_Lerp,id:8607,x:32191,y:32913,varname:node_8607,prsc:2|A-9667-R,B-7622-UVOUT,T-8844-OUT;n:type:ShaderForge.SFN_Tex2d,id:9667,x:32171,y:32631,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_9667,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2385-UVOUT;n:type:ShaderForge.SFN_Slider,id:8844,x:31808,y:33290,ptovrint:False,ptlb:Noise_int,ptin:_Noise_int,varname:node_8844,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Tex2d,id:5298,x:32682,y:32999,ptovrint:False,ptlb:mask,ptin:_mask,varname:node_5298,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_SwitchProperty,id:3759,x:31777,y:33018,ptovrint:False,ptlb:Aoto_offset,ptin:_Aoto_offset,varname:node_3759,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-9079-OUT,B-1206-OUT;n:type:ShaderForge.SFN_Slider,id:9079,x:31476,y:33258,ptovrint:False,ptlb:Offset,ptin:_Offset,varname:node_9079,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;n:type:ShaderForge.SFN_VertexColor,id:2494,x:32915,y:33241,varname:node_2494,prsc:2;proporder:5565-8113-6603-3836-9351-9667-8844-5298-3759-9079;pass:END;sub:END;*/

Shader "my shader/vfx/UVoffset_anim_add_mask" {
    Properties {
        _Offset_map ("Offset_map", 2D) = "black" {}
        _Mian_color ("Mian_color", Color) = (0.5,0.5,0.5,1)
        _Mask_map ("Mask_map", 2D) = "black" {}
        _Alpha ("Alpha", Float ) = 0
        _Speed ("Speed", Float ) = 0
        _Noise ("Noise", 2D) = "white" {}
        _Noise_int ("Noise_int", Range(0, 1)) = 0
        _mask ("mask", 2D) = "white" {}
        [MaterialToggle] _Aoto_offset ("Aoto_offset", Float ) = 0
        _Offset ("Offset", Range(0, 10)) = 0
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
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal n3ds wiiu 
            #pragma target 2.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Offset_map; uniform float4 _Offset_map_ST;
            uniform float4 _Mian_color;
            uniform sampler2D _Mask_map; uniform float4 _Mask_map_ST;
            uniform float _Alpha;
            uniform float _Speed;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Noise_int;
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform fixed _Aoto_offset;
            uniform float _Offset;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
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
                float2 node_2385 = (i.uv0+lerp( _Offset, (node_4776.g*_Speed), _Aoto_offset )*float2(0,0.2));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_2385, _Noise));
                float2 node_8607 = lerp(float2(_Noise_var.r,_Noise_var.r),i.uv0,_Noise_int);
                float4 _Offset_map_var = tex2D(_Offset_map,TRANSFORM_TEX(node_8607, _Offset_map));
                float4 _Mask_map_var = tex2D(_Mask_map,TRANSFORM_TEX(node_2385, _Mask_map));
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(i.uv0, _mask));
                float3 emissive = ((_Alpha*(_Offset_map_var.rgb*_Mask_map_var.rgb))*_mask_var.r*_Mian_color.rgb*i.vertexColor.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
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
