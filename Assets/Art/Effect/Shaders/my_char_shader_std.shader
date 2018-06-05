// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.27 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.27;sub:START;pass:START;ps:flbk:Mobile/Diffuse,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:2510,x:34539,y:31923,varname:node_2510,prsc:2|normal-2255-RGB,emission-5432-OUT,custl-7404-OUT,clip-4654-A;n:type:ShaderForge.SFN_Tex2d,id:4654,x:31349,y:31164,ptovrint:False,ptlb:Base(RGBA),ptin:_BaseRGBA,varname:_Diffuse,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0820f04d4b48d8843a4e20d240ae5f52,ntxv:0,isnm:False;n:type:ShaderForge.SFN_NormalVector,id:8842,x:30758,y:32148,prsc:2,pt:True;n:type:ShaderForge.SFN_LightVector,id:5561,x:31063,y:32006,varname:node_5561,prsc:2;n:type:ShaderForge.SFN_HalfVector,id:6373,x:30732,y:32421,varname:node_6373,prsc:2;n:type:ShaderForge.SFN_Dot,id:403,x:31295,y:32036,varname:node_403,prsc:2,dt:1|A-5561-OUT,B-8842-OUT;n:type:ShaderForge.SFN_Dot,id:7276,x:31251,y:32304,varname:node_7276,prsc:2,dt:1|A-8842-OUT,B-6373-OUT;n:type:ShaderForge.SFN_Multiply,id:8611,x:32708,y:31648,varname:node_8611,prsc:2|A-4914-OUT,B-403-OUT,C-1038-OUT;n:type:ShaderForge.SFN_Power,id:2822,x:31522,y:32083,varname:node_2822,prsc:2|VAL-7276-OUT,EXP-3722-OUT;n:type:ShaderForge.SFN_Exp,id:3722,x:31482,y:32280,varname:node_3722,prsc:2,et:1|IN-9517-OUT;n:type:ShaderForge.SFN_Slider,id:9517,x:31172,y:32490,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:5.313067,max:11;n:type:ShaderForge.SFN_Multiply,id:4179,x:31882,y:32000,varname:node_4179,prsc:2|A-2822-OUT,B-4084-OUT,C-1222-A;n:type:ShaderForge.SFN_Slider,id:2640,x:31611,y:32354,ptovrint:False,ptlb:Specularinty,ptin:_Specularinty,varname:_Specularinty,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.7473624,max:11;n:type:ShaderForge.SFN_Add,id:6933,x:32874,y:31941,varname:node_6933,prsc:2|A-8611-OUT,B-4179-OUT;n:type:ShaderForge.SFN_Multiply,id:7404,x:33609,y:32202,varname:node_7404,prsc:2|A-6933-OUT,B-9190-RGB,C-5277-OUT,D-1038-OUT,E-4453-OUT;n:type:ShaderForge.SFN_LightColor,id:9190,x:33000,y:32099,varname:node_9190,prsc:2;n:type:ShaderForge.SFN_LightAttenuation,id:5277,x:32953,y:32248,varname:node_5277,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:2255,x:33812,y:32016,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:_Normal,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:132c1abf0e1594f47a24dad65d015101,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Fresnel,id:6936,x:31373,y:33524,varname:node_6936,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:7509,x:32026,y:32962,ptovrint:False,ptlb:RGB mask,ptin:_RGBmask,varname:_RGBmask,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:75a76f0536b8a5e499d892a78bf1d47b,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:5432,x:33713,y:32678,varname:node_5432,prsc:2|A-3263-OUT,B-4480-OUT,C-2947-OUT,D-5973-OUT,E-1801-OUT;n:type:ShaderForge.SFN_Multiply,id:3263,x:32771,y:32357,varname:node_3263,prsc:2|A-5076-RGB,B-1227-OUT,C-2212-OUT;n:type:ShaderForge.SFN_AmbientLight,id:5076,x:32648,y:32237,varname:node_5076,prsc:2;n:type:ShaderForge.SFN_Cubemap,id:4379,x:30900,y:33455,ptovrint:False,ptlb:Cubemap,ptin:_Cubemap,varname:_Cubemap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,cube:9693d50a34bca46c6bbcb5b38ec4213a,pvfc:0;n:type:ShaderForge.SFN_Multiply,id:1542,x:31162,y:33502,varname:node_1542,prsc:2|A-4379-RGB,B-821-OUT;n:type:ShaderForge.SFN_Multiply,id:6916,x:31441,y:33265,varname:node_6916,prsc:2|A-5425-OUT,B-1542-OUT;n:type:ShaderForge.SFN_Multiply,id:3400,x:31902,y:33314,varname:node_3400,prsc:2|A-5853-OUT,B-5395-OUT;n:type:ShaderForge.SFN_Vector1,id:5425,x:31042,y:33254,varname:node_5425,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Vector1,id:821,x:30900,y:33632,varname:node_821,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:5549,x:31180,y:33672,varname:node_5549,prsc:2|A-8842-OUT,B-657-OUT;n:type:ShaderForge.SFN_Lerp,id:5853,x:31622,y:33394,varname:node_5853,prsc:2|A-6916-OUT,B-1542-OUT,T-6936-OUT;n:type:ShaderForge.SFN_Vector1,id:657,x:31006,y:33818,varname:node_657,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Add,id:5395,x:31676,y:33663,varname:node_5395,prsc:2|A-5549-OUT,B-2927-OUT;n:type:ShaderForge.SFN_OneMinus,id:2927,x:31463,y:33885,varname:node_2927,prsc:2|IN-657-OUT;n:type:ShaderForge.SFN_Multiply,id:1523,x:32580,y:33059,varname:node_1523,prsc:2|A-4654-R,B-3400-OUT,C-9647-OUT;n:type:ShaderForge.SFN_Multiply,id:8596,x:32502,y:33354,varname:node_8596,prsc:2|A-4654-B,B-3400-OUT,C-8274-OUT;n:type:ShaderForge.SFN_Slider,id:8274,x:32069,y:33449,ptovrint:False,ptlb:B_CM_Str,ptin:_B_CM_Str,varname:_B_CM_Str,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2.048157,max:5;n:type:ShaderForge.SFN_Slider,id:9647,x:31726,y:33160,ptovrint:False,ptlb:R_CM_Str,ptin:_R_CM_Str,varname:_R_CM_Str,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.6773703,max:5;n:type:ShaderForge.SFN_Multiply,id:2947,x:32826,y:33114,varname:node_2947,prsc:2|A-7509-R,B-1523-OUT;n:type:ShaderForge.SFN_Multiply,id:5973,x:32850,y:33261,varname:node_5973,prsc:2|A-7509-B,B-8596-OUT;n:type:ShaderForge.SFN_Slider,id:3732,x:31197,y:32940,ptovrint:False,ptlb:skin_int,ptin:_skin_int,varname:node_3732,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.174269,max:5;n:type:ShaderForge.SFN_Multiply,id:4480,x:32805,y:32939,varname:node_4480,prsc:2|A-208-OUT,B-7509-G,C-1227-OUT;n:type:ShaderForge.SFN_ViewVector,id:9563,x:30888,y:32791,varname:node_9563,prsc:2;n:type:ShaderForge.SFN_Dot,id:4996,x:31197,y:32729,varname:node_4996,prsc:2,dt:1|A-8842-OUT,B-9563-OUT;n:type:ShaderForge.SFN_OneMinus,id:4774,x:31443,y:32684,varname:node_4774,prsc:2|IN-4996-OUT;n:type:ShaderForge.SFN_Multiply,id:208,x:31929,y:32787,varname:node_208,prsc:2|A-2102-OUT,B-3732-OUT,C-7365-RGB;n:type:ShaderForge.SFN_Color,id:7365,x:31388,y:33025,ptovrint:False,ptlb:skin_color,ptin:_skin_color,varname:node_7365,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Power,id:2102,x:31652,y:32735,varname:node_2102,prsc:2|VAL-4774-OUT,EXP-6838-OUT;n:type:ShaderForge.SFN_Vector1,id:6838,x:31477,y:32847,varname:node_6838,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:2212,x:32540,y:32517,ptovrint:False,ptlb:Amb_light,ptin:_Amb_light,varname:node_2212,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;n:type:ShaderForge.SFN_Tex2d,id:1222,x:31232,y:31797,ptovrint:False,ptlb:RGB(Specular)A(Gloss),ptin:_RGBSpecularAGloss,varname:node_1222,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:4084,x:31782,y:32176,varname:node_4084,prsc:2|A-1222-RGB,B-2640-OUT;n:type:ShaderForge.SFN_Color,id:5457,x:31884,y:31098,ptovrint:False,ptlb:Main_color,ptin:_Main_color,varname:node_5457,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:4914,x:32160,y:31012,varname:node_4914,prsc:2|A-1227-OUT,B-5457-RGB;n:type:ShaderForge.SFN_Multiply,id:1801,x:33035,y:32619,varname:node_1801,prsc:2|A-6599-OUT,B-7509-G,C-1227-OUT;n:type:ShaderForge.SFN_Slider,id:6599,x:32439,y:32673,ptovrint:False,ptlb:skin_Amb,ptin:_skin_Amb,varname:node_6599,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Color,id:8170,x:32772,y:32759,ptovrint:False,ptlb:skin_color_offset,ptin:_skin_color_offset,varname:node_8170,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:9241,x:32201,y:31736,ptovrint:False,ptlb:AO_map,ptin:_AO_map,varname:node_9241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:02b23288066c3344986cc093c3ba7a9c,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Power,id:1038,x:32493,y:31797,varname:node_1038,prsc:2|VAL-9241-RGB,EXP-1026-OUT;n:type:ShaderForge.SFN_Slider,id:1026,x:32196,y:31942,ptovrint:False,ptlb:AO_int,ptin:_AO_int,varname:node_1026,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:5290,x:33263,y:32356,varname:node_5290,prsc:2|A-7509-G,B-4151-OUT;n:type:ShaderForge.SFN_Slider,id:4151,x:32918,y:32537,ptovrint:False,ptlb:skin_light,ptin:_skin_light,varname:node_4151,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.7397742,max:1;n:type:ShaderForge.SFN_OneMinus,id:4453,x:33464,y:32391,varname:node_4453,prsc:2|IN-5290-OUT;n:type:ShaderForge.SFN_Color,id:5695,x:31186,y:31462,ptovrint:False,ptlb:Cloth_color,ptin:_Cloth_color,varname:node_5695,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:1227,x:31441,y:31423,varname:node_1227,prsc:2|A-4654-RGB,B-5695-RGB,T-7509-A;proporder:5457-4654-1222-9517-2640-2255-7509-9241-1026-4379-8274-9647-3732-7365-2212-6599-8170-4151-5695;pass:END;sub:END;*/

Shader "my shader/role/my_role_shader_std" {
    Properties {
        _Main_color ("Main_color", Color) = (0.5,0.5,0.5,1)
        _BaseRGBA ("Base(RGBA)", 2D) = "white" {}
        _RGBSpecularAGloss ("RGB(Specular)A(Gloss)", 2D) = "white" {}
        _Gloss ("Gloss", Range(0, 11)) = 5.313067
        _Specularinty ("Specularinty", Range(0, 11)) = 0.7473624
        _Normal ("Normal", 2D) = "bump" {}
        _RGBmask ("RGB mask", 2D) = "white" {}
        _AO_map ("AO_map", 2D) = "white" {}
        _AO_int ("AO_int", Range(0, 1)) = 0
        _Cubemap ("Cubemap", Cube) = "_Skybox" {}
        _B_CM_Str ("B_CM_Str", Range(0, 5)) = 2.048157
        _R_CM_Str ("R_CM_Str", Range(0, 5)) = 0.6773703
        _skin_int ("skin_int", Range(0, 5)) = 1.174269
        _skin_color ("skin_color", Color) = (0.5,0.5,0.5,1)
        _Amb_light ("Amb_light", Range(0, 10)) = 0
        _skin_Amb ("skin_Amb", Range(0, 1)) = 0
        _skin_color_offset ("skin_color_offset", Color) = (0.5,0.5,0.5,1)
        _skin_light ("skin_light", Range(0, 1)) = 0.7397742
        _Cloth_color ("Cloth_color", Color) = (0.5,0.5,0.5,1)
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
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _BaseRGBA; uniform float4 _BaseRGBA_ST;
            uniform float _Gloss;
            uniform float _Specularinty;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform sampler2D _RGBmask; uniform float4 _RGBmask_ST;
            uniform samplerCUBE _Cubemap;
            uniform float _B_CM_Str;
            uniform float _R_CM_Str;
            uniform float _skin_int;
            uniform float4 _skin_color;
            uniform float _Amb_light;
            uniform sampler2D _RGBSpecularAGloss; uniform float4 _RGBSpecularAGloss_ST;
            uniform float4 _Main_color;
            uniform float _skin_Amb;
            uniform sampler2D _AO_map; uniform float4 _AO_map_ST;
            uniform float _AO_int;
            uniform float _skin_light;
            uniform float4 _Cloth_color;
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
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalLocal = _Normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float4 _BaseRGBA_var = tex2D(_BaseRGBA,TRANSFORM_TEX(i.uv0, _BaseRGBA));
                clip(_BaseRGBA_var.a - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
////// Emissive:
                float4 _RGBmask_var = tex2D(_RGBmask,TRANSFORM_TEX(i.uv0, _RGBmask));
                float3 node_1227 = lerp(_BaseRGBA_var.rgb,_Cloth_color.rgb,_RGBmask_var.a);
                float3 node_1542 = (texCUBE(_Cubemap,viewReflectDirection).rgb*1.0);
                float node_657 = 0.3;
                float3 node_3400 = (lerp((0.5*node_1542),node_1542,(1.0-max(0,dot(normalDirection, viewDirection))))*((normalDirection*node_657)+(1.0 - node_657)));
                float3 emissive = ((UNITY_LIGHTMODEL_AMBIENT.rgb*node_1227*_Amb_light)+((pow((1.0 - max(0,dot(normalDirection,viewDirection))),1.0)*_skin_int*_skin_color.rgb)*_RGBmask_var.g*node_1227)+(_RGBmask_var.r*(_BaseRGBA_var.r*node_3400*_R_CM_Str))+(_RGBmask_var.b*(_BaseRGBA_var.b*node_3400*_B_CM_Str))+(_skin_Amb*_RGBmask_var.g*node_1227));
                float4 _AO_map_var = tex2D(_AO_map,TRANSFORM_TEX(i.uv0, _AO_map));
                float3 node_1038 = pow(_AO_map_var.rgb,_AO_int);
                float4 _RGBSpecularAGloss_var = tex2D(_RGBSpecularAGloss,TRANSFORM_TEX(i.uv0, _RGBSpecularAGloss));
                float3 finalColor = emissive + ((((node_1227*_Main_color.rgb)*max(0,dot(lightDirection,normalDirection))*node_1038)+(pow(max(0,dot(normalDirection,halfDirection)),exp2(_Gloss))*(_RGBSpecularAGloss_var.rgb*_Specularinty)*_RGBSpecularAGloss_var.a))*_LightColor0.rgb*attenuation*node_1038*(1.0 - (_RGBmask_var.g*_skin_light)));
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _BaseRGBA; uniform float4 _BaseRGBA_ST;
            uniform float _Gloss;
            uniform float _Specularinty;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform sampler2D _RGBmask; uniform float4 _RGBmask_ST;
            uniform samplerCUBE _Cubemap;
            uniform float _B_CM_Str;
            uniform float _R_CM_Str;
            uniform float _skin_int;
            uniform float4 _skin_color;
            uniform float _Amb_light;
            uniform sampler2D _RGBSpecularAGloss; uniform float4 _RGBSpecularAGloss_ST;
            uniform float4 _Main_color;
            uniform float _skin_Amb;
            uniform sampler2D _AO_map; uniform float4 _AO_map_ST;
            uniform float _AO_int;
            uniform float _skin_light;
            uniform float4 _Cloth_color;
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
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalLocal = _Normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float4 _BaseRGBA_var = tex2D(_BaseRGBA,TRANSFORM_TEX(i.uv0, _BaseRGBA));
                clip(_BaseRGBA_var.a - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float4 _RGBmask_var = tex2D(_RGBmask,TRANSFORM_TEX(i.uv0, _RGBmask));
                float3 node_1227 = lerp(_BaseRGBA_var.rgb,_Cloth_color.rgb,_RGBmask_var.a);
                float4 _AO_map_var = tex2D(_AO_map,TRANSFORM_TEX(i.uv0, _AO_map));
                float3 node_1038 = pow(_AO_map_var.rgb,_AO_int);
                float4 _RGBSpecularAGloss_var = tex2D(_RGBSpecularAGloss,TRANSFORM_TEX(i.uv0, _RGBSpecularAGloss));
                float3 finalColor = ((((node_1227*_Main_color.rgb)*max(0,dot(lightDirection,normalDirection))*node_1038)+(pow(max(0,dot(normalDirection,halfDirection)),exp2(_Gloss))*(_RGBSpecularAGloss_var.rgb*_Specularinty)*_RGBSpecularAGloss_var.a))*_LightColor0.rgb*attenuation*node_1038*(1.0 - (_RGBmask_var.g*_skin_light)));
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
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
            uniform sampler2D _BaseRGBA; uniform float4 _BaseRGBA_ST;
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
                float4 _BaseRGBA_var = tex2D(_BaseRGBA,TRANSFORM_TEX(i.uv0, _BaseRGBA));
                clip(_BaseRGBA_var.a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
