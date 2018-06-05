// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.27 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.27;sub:START;pass:START;ps:flbk:Mobile/Diffuse,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:33016,y:32773,varname:node_4013,prsc:2|normal-9651-OUT,custl-5089-OUT,alpha-5680-OUT,refract-5-OUT;n:type:ShaderForge.SFN_Tex2d,id:9867,x:31169,y:32812,ptovrint:False,ptlb:Refraction_map,ptin:_Refraction_map,varname:node_9867,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:9651,x:31540,y:32653,varname:node_9651,prsc:2|A-7922-OUT,B-9867-RGB,T-9261-OUT;n:type:ShaderForge.SFN_Slider,id:9261,x:31092,y:33315,ptovrint:False,ptlb:refracsion_int,ptin:_refracsion_int,varname:node_9261,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Vector3,id:7922,x:31187,y:32334,varname:node_7922,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Color,id:6375,x:31602,y:32899,ptovrint:False,ptlb:Shield Color,ptin:_ShieldColor,varname:node_6375,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:0,c4:0.1;n:type:ShaderForge.SFN_Slider,id:5269,x:31725,y:32822,ptovrint:False,ptlb:Glow Power,ptin:_GlowPower,varname:node_5269,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:2;n:type:ShaderForge.SFN_Multiply,id:2875,x:32146,y:32835,varname:node_2875,prsc:2|A-5269-OUT,B-6375-RGB;n:type:ShaderForge.SFN_Slider,id:3662,x:30805,y:33009,ptovrint:False,ptlb:Fresnel Factor,ptin:_FresnelFactor,varname:node_3662,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3808599,max:1;n:type:ShaderForge.SFN_Multiply,id:5680,x:32253,y:33138,varname:node_5680,prsc:2|A-6375-A,B-8556-OUT;n:type:ShaderForge.SFN_ComponentMask,id:6220,x:31868,y:33207,varname:node_6220,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-9867-RGB;n:type:ShaderForge.SFN_Multiply,id:9085,x:32200,y:33367,varname:node_9085,prsc:2|A-9261-OUT,B-2838-OUT;n:type:ShaderForge.SFN_Vector1,id:2838,x:31948,y:33440,varname:node_2838,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Multiply,id:5,x:32397,y:33253,varname:node_5,prsc:2|A-6220-OUT,B-9085-OUT;n:type:ShaderForge.SFN_NormalVector,id:1152,x:30542,y:33072,prsc:2,pt:False;n:type:ShaderForge.SFN_ViewVector,id:398,x:30592,y:33369,varname:node_398,prsc:2;n:type:ShaderForge.SFN_OneMinus,id:9844,x:30938,y:33180,varname:node_9844,prsc:2|IN-1216-OUT;n:type:ShaderForge.SFN_Dot,id:1216,x:30847,y:33404,varname:node_1216,prsc:2,dt:0|A-1152-OUT,B-398-OUT;n:type:ShaderForge.SFN_Multiply,id:8556,x:31249,y:33051,varname:node_8556,prsc:2|A-3662-OUT,B-9844-OUT;n:type:ShaderForge.SFN_RemapRange,id:5089,x:32317,y:32900,varname:node_5089,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-2875-OUT;proporder:9867-9261-6375-5269-3662;pass:END;sub:END;*/

Shader "my shader/role/my_char_refraction" {
    Properties {
        _Refraction_map ("Refraction_map", 2D) = "bump" {}
        _refracsion_int ("refracsion_int", Range(0, 1)) = 0
        _ShieldColor ("Shield Color", Color) = (1,1,0,0.1)
        _GlowPower ("Glow Power", Range(1, 2)) = 1
        _FresnelFactor ("Fresnel Factor", Range(0, 1)) = 0.3808599
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
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
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _Refraction_map; uniform float4 _Refraction_map_ST;
            uniform float _refracsion_int;
            uniform float4 _ShieldColor;
            uniform float _GlowPower;
            uniform float _FresnelFactor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Refraction_map_var = UnpackNormal(tex2D(_Refraction_map,TRANSFORM_TEX(i.uv0, _Refraction_map)));
                float3 normalLocal = lerp(float3(0,0,1),_Refraction_map_var.rgb,_refracsion_int);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (_Refraction_map_var.rgb.rg*(_refracsion_int*0.2));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
                float3 finalColor = ((_GlowPower*_ShieldColor.rgb)*2.0+-1.0);
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,(_ShieldColor.a*(_FresnelFactor*(1.0 - dot(i.normalDir,viewDirection))))),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
