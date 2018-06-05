// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:Mobile/Diffuse,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:33465,y:32809,varname:node_4013,prsc:2|normal-9651-OUT,emission-9005-OUT,alpha-5680-OUT;n:type:ShaderForge.SFN_Tex2d,id:9867,x:30973,y:32674,ptovrint:False,ptlb:Refraction_map,ptin:_Refraction_map,varname:node_9867,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-5048-OUT;n:type:ShaderForge.SFN_Lerp,id:9651,x:31540,y:32653,varname:node_9651,prsc:2|A-7922-OUT,B-9867-RGB,T-9261-OUT;n:type:ShaderForge.SFN_Slider,id:9261,x:31105,y:33290,ptovrint:False,ptlb:refracsion_int,ptin:_refracsion_int,varname:node_9261,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Vector3,id:7922,x:31187,y:32334,varname:node_7922,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Color,id:6375,x:32258,y:32988,ptovrint:False,ptlb:Shield Color,ptin:_ShieldColor,varname:node_6375,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:0,c4:0.1;n:type:ShaderForge.SFN_Multiply,id:5680,x:32656,y:33063,varname:node_5680,prsc:2|A-6375-A,B-1420-A,C-1266-A;n:type:ShaderForge.SFN_ComponentMask,id:6220,x:31983,y:33129,varname:node_6220,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-9867-RGB;n:type:ShaderForge.SFN_Multiply,id:9085,x:32225,y:33368,varname:node_9085,prsc:2|A-9261-OUT,B-2838-OUT,C-9415-OUT;n:type:ShaderForge.SFN_Vector1,id:2838,x:31873,y:33403,varname:node_2838,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Multiply,id:5,x:32490,y:33279,varname:node_5,prsc:2|A-6220-OUT,B-9085-OUT;n:type:ShaderForge.SFN_Tex2d,id:6905,x:30450,y:32812,ptovrint:False,ptlb:noise_map,ptin:_noise_map,varname:node_6905,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-3133-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:1990,x:30114,y:32515,varname:node_1990,prsc:2,uv:0;n:type:ShaderForge.SFN_Panner,id:3133,x:30151,y:32773,varname:node_3133,prsc:2,spu:0,spv:0.05|UVIN-1990-UVOUT,DIST-8939-TSL;n:type:ShaderForge.SFN_Time,id:8939,x:30074,y:32988,varname:node_8939,prsc:2;n:type:ShaderForge.SFN_Lerp,id:5048,x:30740,y:32817,varname:node_5048,prsc:2|A-1990-UVOUT,B-6905-R,T-1053-OUT;n:type:ShaderForge.SFN_Tex2d,id:1420,x:32258,y:32811,ptovrint:False,ptlb:main_color,ptin:_main_color,varname:node_1420,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5048-OUT;n:type:ShaderForge.SFN_Vector3,id:9415,x:31960,y:33503,varname:node_9415,prsc:2,v1:0,v2:2,v3:0;n:type:ShaderForge.SFN_Multiply,id:3982,x:32669,y:32710,varname:node_3982,prsc:2|A-1420-RGB,B-6375-RGB;n:type:ShaderForge.SFN_Vector1,id:1053,x:30392,y:33030,varname:node_1053,prsc:2,v1:0.03;n:type:ShaderForge.SFN_Cubemap,id:3294,x:32774,y:32405,ptovrint:False,ptlb:Cube_map,ptin:_Cube_map,varname:node_3294,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,pvfc:0;n:type:ShaderForge.SFN_Multiply,id:9005,x:33153,y:32541,varname:node_9005,prsc:2|A-3294-RGB,B-3982-OUT,C-8693-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8693,x:32942,y:32781,ptovrint:False,ptlb:Cube_int,ptin:_Cube_int,varname:node_8693,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_VertexColor,id:1266,x:32385,y:33124,varname:node_1266,prsc:2;proporder:9867-9261-6375-6905-1420-3294-8693;pass:END;sub:END;*/

Shader "my shader/scene/scene_refraction" {
    Properties {
        _Refraction_map ("Refraction_map", 2D) = "bump" {}
        _refracsion_int ("refracsion_int", Range(0, 1)) = 0
        _ShieldColor ("Shield Color", Color) = (1,1,0,0.1)
        _noise_map ("noise_map", 2D) = "white" {}
        _main_color ("main_color", 2D) = "white" {}
        _Cube_map ("Cube_map", Cube) = "_Skybox" {}
        _Cube_int ("Cube_int", Float ) = 0
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
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal n3ds wiiu 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Refraction_map; uniform float4 _Refraction_map_ST;
            uniform float _refracsion_int;
            uniform float4 _ShieldColor;
            uniform sampler2D _noise_map; uniform float4 _noise_map_ST;
            uniform sampler2D _main_color; uniform float4 _main_color_ST;
            uniform samplerCUBE _Cube_map;
            uniform float _Cube_int;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_8939 = _Time + _TimeEditor;
                float2 node_3133 = (i.uv0+node_8939.r*float2(0,0.05));
                float4 _noise_map_var = tex2D(_noise_map,TRANSFORM_TEX(node_3133, _noise_map));
                float2 node_5048 = lerp(i.uv0,float2(_noise_map_var.r,_noise_map_var.r),0.03);
                float3 _Refraction_map_var = UnpackNormal(tex2D(_Refraction_map,TRANSFORM_TEX(node_5048, _Refraction_map)));
                float3 normalLocal = lerp(float3(0,0,1),_Refraction_map_var.rgb,_refracsion_int);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
////// Lighting:
////// Emissive:
                float4 _main_color_var = tex2D(_main_color,TRANSFORM_TEX(node_5048, _main_color));
                float3 emissive = (texCUBE(_Cube_map,viewReflectDirection).rgb*(_main_color_var.rgb*_ShieldColor.rgb)*_Cube_int);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,(_ShieldColor.a*_main_color_var.a*i.vertexColor.a));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
