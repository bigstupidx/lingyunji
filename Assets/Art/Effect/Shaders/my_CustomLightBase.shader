// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.27 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.27;sub:START;pass:START;ps:flbk:Mobile/Diffuse,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:2510,x:33831,y:32017,varname:node_2510,prsc:2|normal-2255-RGB,emission-8329-OUT,custl-7404-OUT;n:type:ShaderForge.SFN_Tex2d,id:4654,x:31552,y:31635,ptovrint:False,ptlb:Base,ptin:_Base,varname:_Diffuse,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0820f04d4b48d8843a4e20d240ae5f52,ntxv:0,isnm:False;n:type:ShaderForge.SFN_NormalVector,id:8842,x:30758,y:32148,prsc:2,pt:True;n:type:ShaderForge.SFN_LightVector,id:5561,x:30834,y:31895,varname:node_5561,prsc:2;n:type:ShaderForge.SFN_HalfVector,id:6373,x:30732,y:32421,varname:node_6373,prsc:2;n:type:ShaderForge.SFN_Dot,id:403,x:31295,y:32036,varname:node_403,prsc:2,dt:1|A-5561-OUT,B-8842-OUT;n:type:ShaderForge.SFN_Dot,id:7276,x:31156,y:32199,varname:node_7276,prsc:2,dt:1|A-8842-OUT,B-6373-OUT;n:type:ShaderForge.SFN_Multiply,id:8611,x:32366,y:31752,varname:node_8611,prsc:2|A-4654-RGB,B-403-OUT;n:type:ShaderForge.SFN_Power,id:2822,x:31813,y:32072,varname:node_2822,prsc:2|VAL-7276-OUT,EXP-3722-OUT;n:type:ShaderForge.SFN_Exp,id:3722,x:31519,y:32406,varname:node_3722,prsc:2,et:1|IN-9517-OUT;n:type:ShaderForge.SFN_Slider,id:9517,x:31156,y:32420,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:5.313067,max:11;n:type:ShaderForge.SFN_Multiply,id:4179,x:32366,y:31866,varname:node_4179,prsc:2|A-2822-OUT,B-6006-OUT,C-652-RGB;n:type:ShaderForge.SFN_Slider,id:2640,x:31727,y:32550,ptovrint:False,ptlb:Specularinty,ptin:_Specularinty,varname:_Specularinty,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.7473624,max:11;n:type:ShaderForge.SFN_Add,id:6933,x:32578,y:31936,varname:node_6933,prsc:2|A-8611-OUT,B-4179-OUT;n:type:ShaderForge.SFN_Multiply,id:7404,x:33094,y:31981,varname:node_7404,prsc:2|A-6933-OUT,B-9190-RGB,C-5277-OUT;n:type:ShaderForge.SFN_LightColor,id:9190,x:32578,y:32053,varname:node_9190,prsc:2;n:type:ShaderForge.SFN_LightAttenuation,id:5277,x:32578,y:32163,varname:node_5277,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:2255,x:30683,y:32680,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:_Normal,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:132c1abf0e1594f47a24dad65d015101,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Multiply,id:3263,x:33288,y:32373,varname:node_3263,prsc:2|A-5076-RGB,B-4654-RGB,C-2212-OUT;n:type:ShaderForge.SFN_AmbientLight,id:5076,x:32831,y:32234,varname:node_5076,prsc:2;n:type:ShaderForge.SFN_Slider,id:2212,x:32506,y:32484,ptovrint:False,ptlb:Amb_light,ptin:_Amb_light,varname:node_2212,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Tex2d,id:6818,x:31780,y:32336,ptovrint:False,ptlb:Spe_map,ptin:_Spe_map,varname:node_6818,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:6006,x:32093,y:32097,varname:node_6006,prsc:2|A-6818-RGB,B-2640-OUT;n:type:ShaderForge.SFN_Tex2d,id:652,x:31562,y:31835,ptovrint:False,ptlb:Gloss_map,ptin:_Gloss_map,varname:node_652,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Fresnel,id:7440,x:32739,y:32952,varname:node_7440,prsc:2|EXP-9915-OUT;n:type:ShaderForge.SFN_Color,id:4961,x:32794,y:32599,ptovrint:False,ptlb:Fresenel_color,ptin:_Fresenel_color,varname:node_1733,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.05033519,c2:0.6708714,c3:0.9779412,c4:1;n:type:ShaderForge.SFN_Multiply,id:6461,x:33242,y:32592,varname:node_6461,prsc:2|A-4961-RGB,B-786-OUT,C-6538-OUT;n:type:ShaderForge.SFN_Slider,id:9915,x:32300,y:33085,ptovrint:False,ptlb:fresnel_amount,ptin:_fresnel_amount,varname:node_2207,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.931624,max:10;n:type:ShaderForge.SFN_Add,id:786,x:32955,y:32913,varname:node_786,prsc:2|A-7440-OUT,B-7440-OUT;n:type:ShaderForge.SFN_Slider,id:6538,x:33020,y:33256,ptovrint:False,ptlb:self_Illumin,ptin:_self_Illumin,varname:node_7175,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4.112586,max:10;n:type:ShaderForge.SFN_Add,id:8329,x:33472,y:32434,varname:node_8329,prsc:2|A-3263-OUT,B-6461-OUT;proporder:4654-9517-2640-2255-2212-6818-652-4961-9915-6538;pass:END;sub:END;*/

Shader "my shader/role/my_CustomLightBase" {
    Properties {
        _Base ("Base", 2D) = "white" {}
        _Gloss ("Gloss", Range(0, 11)) = 5.313067
        _Specularinty ("Specularinty", Range(0, 11)) = 0.7473624
        _Normal ("Normal", 2D) = "bump" {}
        _Amb_light ("Amb_light", Range(0, 2)) = 0
        _Spe_map ("Spe_map", 2D) = "white" {}
        _Gloss_map ("Gloss_map", 2D) = "white" {}
        _Fresenel_color ("Fresenel_color", Color) = (0.05033519,0.6708714,0.9779412,1)
        _fresnel_amount ("fresnel_amount", Range(0, 10)) = 1.931624
        _self_Illumin ("self_Illumin", Range(0, 10)) = 4.112586
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
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
            uniform sampler2D _Base; uniform float4 _Base_ST;
            uniform float _Gloss;
            uniform float _Specularinty;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Amb_light;
            uniform sampler2D _Spe_map; uniform float4 _Spe_map_ST;
            uniform sampler2D _Gloss_map; uniform float4 _Gloss_map_ST;
            uniform float4 _Fresenel_color;
            uniform float _fresnel_amount;
            uniform float _self_Illumin;
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
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalLocal = _Normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
////// Emissive:
                float4 _Base_var = tex2D(_Base,TRANSFORM_TEX(i.uv0, _Base));
                float node_7440 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_fresnel_amount);
                float3 emissive = ((UNITY_LIGHTMODEL_AMBIENT.rgb*_Base_var.rgb*_Amb_light)+(_Fresenel_color.rgb*(node_7440+node_7440)*_self_Illumin));
                float4 _Spe_map_var = tex2D(_Spe_map,TRANSFORM_TEX(i.uv0, _Spe_map));
                float4 _Gloss_map_var = tex2D(_Gloss_map,TRANSFORM_TEX(i.uv0, _Gloss_map));
                float3 finalColor = emissive + (((_Base_var.rgb*max(0,dot(lightDirection,normalDirection)))+(pow(max(0,dot(normalDirection,halfDirection)),exp2(_Gloss))*(_Spe_map_var.rgb*_Specularinty)*_Gloss_map_var.rgb))*_LightColor0.rgb*attenuation);
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
            uniform sampler2D _Base; uniform float4 _Base_ST;
            uniform float _Gloss;
            uniform float _Specularinty;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Amb_light;
            uniform sampler2D _Spe_map; uniform float4 _Spe_map_ST;
            uniform sampler2D _Gloss_map; uniform float4 _Gloss_map_ST;
            uniform float4 _Fresenel_color;
            uniform float _fresnel_amount;
            uniform float _self_Illumin;
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
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalLocal = _Normal_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float4 _Base_var = tex2D(_Base,TRANSFORM_TEX(i.uv0, _Base));
                float4 _Spe_map_var = tex2D(_Spe_map,TRANSFORM_TEX(i.uv0, _Spe_map));
                float4 _Gloss_map_var = tex2D(_Gloss_map,TRANSFORM_TEX(i.uv0, _Gloss_map));
                float3 finalColor = (((_Base_var.rgb*max(0,dot(lightDirection,normalDirection)))+(pow(max(0,dot(normalDirection,halfDirection)),exp2(_Gloss))*(_Spe_map_var.rgb*_Specularinty)*_Gloss_map_var.rgb))*_LightColor0.rgb*attenuation);
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
