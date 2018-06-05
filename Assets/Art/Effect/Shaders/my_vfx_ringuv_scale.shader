// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:Mobile/Particles/Additive,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4795,x:33694,y:32848,varname:node_4795,prsc:2|emission-4163-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:32488,y:32779,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False|UVIN-8602-OUT;n:type:ShaderForge.SFN_Multiply,id:9543,x:32721,y:33028,varname:node_9543,prsc:2|A-6074-RGB,B-9104-RGB,C-8864-OUT;n:type:ShaderForge.SFN_Tex2d,id:9104,x:31778,y:33140,ptovrint:False,ptlb:mask_map,ptin:_mask_map,varname:node_9104,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6915-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8864,x:32387,y:33372,ptovrint:False,ptlb:Emission_int,ptin:_Emission_int,varname:node_8864,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:1892,x:30879,y:32840,ptovrint:False,ptlb:T_distor_map,ptin:_T_distor_map,varname:node_1892,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:2397,x:31255,y:32774,varname:node_2397,prsc:2|A-1126-OUT,B-1892-R,T-9463-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9463,x:31020,y:33302,ptovrint:False,ptlb:T_distort_int,ptin:_T_distort_int,varname:node_9463,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_TexCoord,id:5331,x:30011,y:31854,varname:node_5331,prsc:2,uv:0;n:type:ShaderForge.SFN_Slider,id:2901,x:29745,y:32374,ptovrint:False,ptlb:UV_scale,ptin:_UV_scale,varname:node_2901,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Color,id:3122,x:32971,y:32804,ptovrint:False,ptlb:Main_color,ptin:_Main_color,varname:node_3122,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:4163,x:33373,y:32914,varname:node_4163,prsc:2|A-9543-OUT,B-3122-A,C-1897-OUT,D-3122-RGB;n:type:ShaderForge.SFN_SwitchProperty,id:1897,x:30155,y:32393,ptovrint:False,ptlb:Aoto_scale,ptin:_Aoto_scale,varname:node_1897,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-2901-OUT,B-9736-OUT;n:type:ShaderForge.SFN_Frac,id:9736,x:30082,y:32719,varname:node_9736,prsc:2|IN-6823-OUT;n:type:ShaderForge.SFN_Time,id:6520,x:29262,y:32670,varname:node_6520,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6823,x:29735,y:32823,varname:node_6823,prsc:2|A-6520-T,B-572-OUT;n:type:ShaderForge.SFN_Add,id:2554,x:30695,y:31782,varname:node_2554,prsc:2|A-5331-UVOUT,B-1415-OUT;n:type:ShaderForge.SFN_Subtract,id:5039,x:29912,y:32032,varname:node_5039,prsc:2|A-5331-UVOUT,B-1448-OUT;n:type:ShaderForge.SFN_Vector1,id:7880,x:29662,y:32194,varname:node_7880,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Divide,id:1415,x:30303,y:32021,varname:node_1415,prsc:2|A-5039-OUT,B-1897-OUT;n:type:ShaderForge.SFN_Lerp,id:1126,x:31183,y:32007,varname:node_1126,prsc:2|A-2554-OUT,B-7880-OUT,T-1897-OUT;n:type:ShaderForge.SFN_Clamp01,id:6915,x:31553,y:32924,varname:node_6915,prsc:2|IN-2397-OUT;n:type:ShaderForge.SFN_Lerp,id:8602,x:32150,y:32700,varname:node_8602,prsc:2|A-5331-UVOUT,B-5585-R,T-6914-OUT;n:type:ShaderForge.SFN_Tex2d,id:5585,x:31856,y:32528,ptovrint:False,ptlb:M_distort_map,ptin:_M_distort_map,varname:node_5585,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-755-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6914,x:31856,y:32788,ptovrint:False,ptlb:M_distort_int,ptin:_M_distort_int,varname:node_6914,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Panner,id:2461,x:31344,y:32245,varname:node_2461,prsc:2,spu:0.1,spv:0.1|UVIN-5331-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:572,x:29436,y:33035,ptovrint:False,ptlb:UVspeed,ptin:_UVspeed,varname:node_572,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.2;n:type:ShaderForge.SFN_Multiply,id:755,x:31648,y:32316,varname:node_755,prsc:2|A-2461-UVOUT,B-232-OUT;n:type:ShaderForge.SFN_ValueProperty,id:232,x:31422,y:32469,ptovrint:False,ptlb:M_distort_speed,ptin:_M_distort_speed,varname:node_232,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_ValueProperty,id:1448,x:29603,y:31979,ptovrint:False,ptlb:Scale_offset,ptin:_Scale_offset,varname:node_1448,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;proporder:3122-6074-5585-6914-232-8864-1892-9463-9104-2901-1897-572-1448;pass:END;sub:END;*/

Shader "my shader/vfx/ring_uv_scale" {
    Properties {
        _Main_color ("Main_color", Color) = (0.5,0.5,0.5,1)
        _MainTex ("MainTex", 2D) = "black" {}
        _M_distort_map ("M_distort_map", 2D) = "white" {}
        _M_distort_int ("M_distort_int", Float ) = 0
        _M_distort_speed ("M_distort_speed", Float ) = 0.1
        _Emission_int ("Emission_int", Float ) = 1
        _T_distor_map ("T_distor_map", 2D) = "white" {}
        _T_distort_int ("T_distort_int", Float ) = 0
        _mask_map ("mask_map", 2D) = "white" {}
        _UV_scale ("UV_scale", Range(0, 1)) = 0
        [MaterialToggle] _Aoto_scale ("Aoto_scale", Float ) = 0
        _UVspeed ("UVspeed", Float ) = 0.2
        _Scale_offset ("Scale_offset", Float ) = 0
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
            #pragma multi_compile_fog
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 2.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _mask_map; uniform float4 _mask_map_ST;
            uniform float _Emission_int;
            uniform sampler2D _T_distor_map; uniform float4 _T_distor_map_ST;
            uniform float _T_distort_int;
            uniform float _UV_scale;
            uniform float4 _Main_color;
            uniform fixed _Aoto_scale;
            uniform sampler2D _M_distort_map; uniform float4 _M_distort_map_ST;
            uniform float _M_distort_int;
            uniform float _UVspeed;
            uniform float _M_distort_speed;
            uniform float _Scale_offset;
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
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_1702 = _Time + _TimeEditor;
                float2 node_755 = ((i.uv0+node_1702.g*float2(0.1,0.1))*_M_distort_speed);
                float4 _M_distort_map_var = tex2D(_M_distort_map,TRANSFORM_TEX(node_755, _M_distort_map));
                float2 node_8602 = lerp(i.uv0,float2(_M_distort_map_var.r,_M_distort_map_var.r),_M_distort_int);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_8602, _MainTex));
                float4 node_6520 = _Time + _TimeEditor;
                float _Aoto_scale_var = lerp( _UV_scale, frac((node_6520.g*_UVspeed)), _Aoto_scale );
                float node_7880 = 0.5;
                float4 _T_distor_map_var = tex2D(_T_distor_map,TRANSFORM_TEX(i.uv0, _T_distor_map));
                float2 node_6915 = saturate(lerp(lerp((i.uv0+((i.uv0-_Scale_offset)/_Aoto_scale_var)),float2(node_7880,node_7880),_Aoto_scale_var),float2(_T_distor_map_var.r,_T_distor_map_var.r),_T_distort_int));
                float4 _mask_map_var = tex2D(_mask_map,TRANSFORM_TEX(node_6915, _mask_map));
                float3 emissive = ((_MainTex_var.rgb*_mask_map_var.rgb*_Emission_int)*_Main_color.a*_Aoto_scale_var*_Main_color.rgb);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Mobile/Particles/Additive"
    CustomEditor "ShaderForgeMaterialInspector"
}
