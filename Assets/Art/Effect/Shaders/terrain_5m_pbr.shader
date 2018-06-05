// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.27 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.27;sub:START;pass:START;ps:flbk:Mobile/Diffuse,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:2,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:34879,y:32716,varname:node_4013,prsc:2|diff-9216-OUT,spec-5118-OUT,gloss-6859-OUT,normal-3883-OUT;n:type:ShaderForge.SFN_Tex2d,id:6350,x:32305,y:31342,ptovrint:False,ptlb:1_Layer(R),ptin:_1_LayerR,varname:node_6240,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:5726eb6ca27c5ed47bc66cfcfafedb38,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8790,x:32305,y:31511,ptovrint:False,ptlb:1_Layer(G),ptin:_1_LayerG,varname:node_5562,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:05650c1cd55f7d1418a0088ef5b9a917,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9381,x:32305,y:31677,ptovrint:False,ptlb:1_Layer(B),ptin:_1_LayerB,varname:node_7342,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:c5296741e820d374aa65670c0bcc6987,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:595,x:32305,y:31838,ptovrint:False,ptlb:1_Layer(A),ptin:_1_LayerA,varname:node_1725,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:21e7ae8ff9e868a4dabfa9efad5b11a1,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8664,x:32276,y:33015,ptovrint:False,ptlb:mask1,ptin:_mask1,varname:node_349,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1b1c209bb16909e4c955d49d478d9cdc,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8105,x:32495,y:33430,ptovrint:False,ptlb:1_Layer(A)_N,ptin:_1_LayerA_N,varname:node_802,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a93150e8fd4a5734089dd1072d514421,ntxv:3,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1920,x:32309,y:32423,ptovrint:False,ptlb:2_Layer(G),ptin:_2_LayerG,varname:node_1920,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:24e911958ee56f1459a732d9b61c46ca,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9953,x:32322,y:32775,ptovrint:False,ptlb:mask2,ptin:_mask2,varname:node_9953,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:d4140ebc982a2244ab3e2aad7e11ff73,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3047,x:32337,y:32131,ptovrint:False,ptlb:2_Layer(R),ptin:_2_LayerR,varname:node_3047,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:749edca901c3d2e44a2688bf6f022431,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1878,x:32309,y:32588,ptovrint:False,ptlb:2_Layer(B),ptin:_2_LayerB,varname:node_1878,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:dea70348005ccc44c8309d2aef396f91,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1151,x:33363,y:31563,varname:node_1151,prsc:2|A-6350-RGB,B-8664-R;n:type:ShaderForge.SFN_Multiply,id:4210,x:33398,y:31714,varname:node_4210,prsc:2|A-8790-RGB,B-8664-G;n:type:ShaderForge.SFN_Multiply,id:9641,x:33491,y:31889,varname:node_9641,prsc:2|A-9381-RGB,B-8664-B;n:type:ShaderForge.SFN_Multiply,id:4075,x:33514,y:32117,varname:node_4075,prsc:2|A-595-RGB,B-8664-A;n:type:ShaderForge.SFN_Multiply,id:8281,x:33514,y:32269,varname:node_8281,prsc:2|A-3047-RGB,B-9953-R;n:type:ShaderForge.SFN_Multiply,id:494,x:33514,y:32421,varname:node_494,prsc:2|A-1920-RGB,B-9953-G;n:type:ShaderForge.SFN_Multiply,id:8075,x:33540,y:32563,varname:node_8075,prsc:2|A-1878-RGB,B-9953-B;n:type:ShaderForge.SFN_Add,id:8017,x:34320,y:32127,varname:node_8017,prsc:2|A-1151-OUT,B-4210-OUT,C-9641-OUT,D-4075-OUT,E-8215-OUT;n:type:ShaderForge.SFN_Add,id:8215,x:34009,y:32309,varname:node_8215,prsc:2|A-8281-OUT,B-494-OUT,C-8075-OUT;n:type:ShaderForge.SFN_Multiply,id:9216,x:34506,y:32521,varname:node_9216,prsc:2|A-8017-OUT,B-1924-RGB;n:type:ShaderForge.SFN_Color,id:1924,x:34203,y:32562,ptovrint:False,ptlb:Main_color,ptin:_Main_color,varname:node_1924,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:1967,x:33145,y:32567,varname:node_1967,prsc:2|A-8790-A,B-8664-A,C-4259-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6859,x:33908,y:32985,ptovrint:False,ptlb:Gloss_int,ptin:_Gloss_int,varname:node_6859,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:4259,x:33088,y:32957,ptovrint:False,ptlb:Spc_int,ptin:_Spc_int,varname:node_4259,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:356,x:33012,y:33253,varname:node_356,prsc:2|A-8664-A,B-8105-RGB;n:type:ShaderForge.SFN_Add,id:3883,x:33605,y:33349,varname:node_3883,prsc:2|A-3778-OUT,B-7808-OUT;n:type:ShaderForge.SFN_Multiply,id:7808,x:33366,y:33438,varname:node_7808,prsc:2|A-2322-OUT,B-9177-OUT;n:type:ShaderForge.SFN_Vector4,id:9177,x:32987,y:33612,varname:node_9177,prsc:2,v1:0.5,v2:0.5,v3:1,v4:1;n:type:ShaderForge.SFN_OneMinus,id:2322,x:33012,y:33418,varname:node_2322,prsc:2|IN-8664-A;n:type:ShaderForge.SFN_Add,id:5118,x:33617,y:32868,varname:node_5118,prsc:2|A-2545-OUT,B-1967-OUT;n:type:ShaderForge.SFN_Tex2d,id:1366,x:32266,y:33412,ptovrint:False,ptlb:2_Layer(R)_N,ptin:_2_LayerR_N,varname:node_1366,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:False;n:type:ShaderForge.SFN_Multiply,id:9005,x:32829,y:33040,varname:node_9005,prsc:2|A-9953-R,B-1366-RGB;n:type:ShaderForge.SFN_Add,id:3778,x:33181,y:33101,varname:node_3778,prsc:2|A-9005-OUT,B-356-OUT;n:type:ShaderForge.SFN_Multiply,id:2545,x:33197,y:32699,varname:node_2545,prsc:2|A-3047-A,B-9953-R,C-4259-OUT;proporder:1924-8664-6350-8790-9381-595-8105-9953-3047-1920-1878-6859-4259-1366;pass:END;sub:END;*/

Shader "my shader/scene/terrain_5m_pbr" {
    Properties {
        _Main_color ("Main_color", Color) = (0.5,0.5,0.5,1)
        _mask1 ("mask1", 2D) = "white" {}
        _1_LayerR ("1_Layer(R)", 2D) = "white" {}
        _1_LayerG ("1_Layer(G)", 2D) = "white" {}
        _1_LayerB ("1_Layer(B)", 2D) = "white" {}
        _1_LayerA ("1_Layer(A)", 2D) = "white" {}
        _1_LayerA_N ("1_Layer(A)_N", 2D) = "bump" {}
        _mask2 ("mask2", 2D) = "white" {}
        _2_LayerR ("2_Layer(R)", 2D) = "white" {}
        _2_LayerG ("2_Layer(G)", 2D) = "white" {}
        _2_LayerB ("2_Layer(B)", 2D) = "white" {}
        _Gloss_int ("Gloss_int", Float ) = 0
        _Spc_int ("Spc_int", Float ) = 0
        _2_LayerR_N ("2_Layer(R)_N", 2D) = "bump" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _1_LayerR; uniform float4 _1_LayerR_ST;
            uniform sampler2D _1_LayerG; uniform float4 _1_LayerG_ST;
            uniform sampler2D _1_LayerB; uniform float4 _1_LayerB_ST;
            uniform sampler2D _1_LayerA; uniform float4 _1_LayerA_ST;
            uniform sampler2D _mask1; uniform float4 _mask1_ST;
            uniform sampler2D _1_LayerA_N; uniform float4 _1_LayerA_N_ST;
            uniform sampler2D _2_LayerG; uniform float4 _2_LayerG_ST;
            uniform sampler2D _mask2; uniform float4 _mask2_ST;
            uniform sampler2D _2_LayerR; uniform float4 _2_LayerR_ST;
            uniform sampler2D _2_LayerB; uniform float4 _2_LayerB_ST;
            uniform float4 _Main_color;
            uniform float _Gloss_int;
            uniform float _Spc_int;
            uniform sampler2D _2_LayerR_N; uniform float4 _2_LayerR_N_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
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
                float4 _mask2_var = tex2D(_mask2,TRANSFORM_TEX(i.uv0, _mask2));
                float4 _2_LayerR_N_var = tex2D(_2_LayerR_N,TRANSFORM_TEX(i.uv0, _2_LayerR_N));
                float4 _mask1_var = tex2D(_mask1,TRANSFORM_TEX(i.uv0, _mask1));
                float4 _1_LayerA_N_var = tex2D(_1_LayerA_N,TRANSFORM_TEX(i.uv0, _1_LayerA_N));
                float3 normalLocal = (((_mask2_var.r*_2_LayerR_N_var.rgb)+(_mask1_var.a*_1_LayerA_N_var.rgb))+((1.0 - _mask1_var.a)*float4(0.5,0.5,1,1)));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Gloss_int;
                float specPow = exp2( gloss * 10.0+1.0);
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float4 _2_LayerR_var = tex2D(_2_LayerR,TRANSFORM_TEX(i.uv0, _2_LayerR));
                float4 _1_LayerG_var = tex2D(_1_LayerG,TRANSFORM_TEX(i.uv0, _1_LayerG));
                float node_5118 = ((_2_LayerR_var.a*_mask2_var.r*_Spc_int)+(_1_LayerG_var.a*_mask1_var.a*_Spc_int));
                float3 specularColor = float3(node_5118,node_5118,node_5118);
                float3 directSpecular = 1 * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float4 _1_LayerR_var = tex2D(_1_LayerR,TRANSFORM_TEX(i.uv0, _1_LayerR));
                float4 _1_LayerB_var = tex2D(_1_LayerB,TRANSFORM_TEX(i.uv0, _1_LayerB));
                float4 _1_LayerA_var = tex2D(_1_LayerA,TRANSFORM_TEX(i.uv0, _1_LayerA));
                float4 _2_LayerG_var = tex2D(_2_LayerG,TRANSFORM_TEX(i.uv0, _2_LayerG));
                float4 _2_LayerB_var = tex2D(_2_LayerB,TRANSFORM_TEX(i.uv0, _2_LayerB));
                float3 diffuseColor = (((_1_LayerR_var.rgb*_mask1_var.r)+(_1_LayerG_var.rgb*_mask1_var.g)+(_1_LayerB_var.rgb*_mask1_var.b)+(_1_LayerA_var.rgb*_mask1_var.a)+((_2_LayerR_var.rgb*_mask2_var.r)+(_2_LayerG_var.rgb*_mask2_var.g)+(_2_LayerB_var.rgb*_mask2_var.b)))*_Main_color.rgb);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _1_LayerR; uniform float4 _1_LayerR_ST;
            uniform sampler2D _1_LayerG; uniform float4 _1_LayerG_ST;
            uniform sampler2D _1_LayerB; uniform float4 _1_LayerB_ST;
            uniform sampler2D _1_LayerA; uniform float4 _1_LayerA_ST;
            uniform sampler2D _mask1; uniform float4 _mask1_ST;
            uniform sampler2D _1_LayerA_N; uniform float4 _1_LayerA_N_ST;
            uniform sampler2D _2_LayerG; uniform float4 _2_LayerG_ST;
            uniform sampler2D _mask2; uniform float4 _mask2_ST;
            uniform sampler2D _2_LayerR; uniform float4 _2_LayerR_ST;
            uniform sampler2D _2_LayerB; uniform float4 _2_LayerB_ST;
            uniform float4 _Main_color;
            uniform float _Gloss_int;
            uniform float _Spc_int;
            uniform sampler2D _2_LayerR_N; uniform float4 _2_LayerR_N_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
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
                float4 _mask2_var = tex2D(_mask2,TRANSFORM_TEX(i.uv0, _mask2));
                float4 _2_LayerR_N_var = tex2D(_2_LayerR_N,TRANSFORM_TEX(i.uv0, _2_LayerR_N));
                float4 _mask1_var = tex2D(_mask1,TRANSFORM_TEX(i.uv0, _mask1));
                float4 _1_LayerA_N_var = tex2D(_1_LayerA_N,TRANSFORM_TEX(i.uv0, _1_LayerA_N));
                float3 normalLocal = (((_mask2_var.r*_2_LayerR_N_var.rgb)+(_mask1_var.a*_1_LayerA_N_var.rgb))+((1.0 - _mask1_var.a)*float4(0.5,0.5,1,1)));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Gloss_int;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float4 _2_LayerR_var = tex2D(_2_LayerR,TRANSFORM_TEX(i.uv0, _2_LayerR));
                float4 _1_LayerG_var = tex2D(_1_LayerG,TRANSFORM_TEX(i.uv0, _1_LayerG));
                float node_5118 = ((_2_LayerR_var.a*_mask2_var.r*_Spc_int)+(_1_LayerG_var.a*_mask1_var.a*_Spc_int));
                float3 specularColor = float3(node_5118,node_5118,node_5118);
                float3 directSpecular = attenColor * pow(max(0,dot(reflect(-lightDirection, normalDirection),viewDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _1_LayerR_var = tex2D(_1_LayerR,TRANSFORM_TEX(i.uv0, _1_LayerR));
                float4 _1_LayerB_var = tex2D(_1_LayerB,TRANSFORM_TEX(i.uv0, _1_LayerB));
                float4 _1_LayerA_var = tex2D(_1_LayerA,TRANSFORM_TEX(i.uv0, _1_LayerA));
                float4 _2_LayerG_var = tex2D(_2_LayerG,TRANSFORM_TEX(i.uv0, _2_LayerG));
                float4 _2_LayerB_var = tex2D(_2_LayerB,TRANSFORM_TEX(i.uv0, _2_LayerB));
                float3 diffuseColor = (((_1_LayerR_var.rgb*_mask1_var.r)+(_1_LayerG_var.rgb*_mask1_var.g)+(_1_LayerB_var.rgb*_mask1_var.b)+(_1_LayerA_var.rgb*_mask1_var.a)+((_2_LayerR_var.rgb*_mask2_var.r)+(_2_LayerG_var.rgb*_mask2_var.g)+(_2_LayerB_var.rgb*_mask2_var.b)))*_Main_color.rgb);
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _1_LayerR; uniform float4 _1_LayerR_ST;
            uniform sampler2D _1_LayerG; uniform float4 _1_LayerG_ST;
            uniform sampler2D _1_LayerB; uniform float4 _1_LayerB_ST;
            uniform sampler2D _1_LayerA; uniform float4 _1_LayerA_ST;
            uniform sampler2D _mask1; uniform float4 _mask1_ST;
            uniform sampler2D _2_LayerG; uniform float4 _2_LayerG_ST;
            uniform sampler2D _mask2; uniform float4 _mask2_ST;
            uniform sampler2D _2_LayerR; uniform float4 _2_LayerR_ST;
            uniform sampler2D _2_LayerB; uniform float4 _2_LayerB_ST;
            uniform float4 _Main_color;
            uniform float _Gloss_int;
            uniform float _Spc_int;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float4 _1_LayerR_var = tex2D(_1_LayerR,TRANSFORM_TEX(i.uv0, _1_LayerR));
                float4 _mask1_var = tex2D(_mask1,TRANSFORM_TEX(i.uv0, _mask1));
                float4 _1_LayerG_var = tex2D(_1_LayerG,TRANSFORM_TEX(i.uv0, _1_LayerG));
                float4 _1_LayerB_var = tex2D(_1_LayerB,TRANSFORM_TEX(i.uv0, _1_LayerB));
                float4 _1_LayerA_var = tex2D(_1_LayerA,TRANSFORM_TEX(i.uv0, _1_LayerA));
                float4 _2_LayerR_var = tex2D(_2_LayerR,TRANSFORM_TEX(i.uv0, _2_LayerR));
                float4 _mask2_var = tex2D(_mask2,TRANSFORM_TEX(i.uv0, _mask2));
                float4 _2_LayerG_var = tex2D(_2_LayerG,TRANSFORM_TEX(i.uv0, _2_LayerG));
                float4 _2_LayerB_var = tex2D(_2_LayerB,TRANSFORM_TEX(i.uv0, _2_LayerB));
                float3 diffColor = (((_1_LayerR_var.rgb*_mask1_var.r)+(_1_LayerG_var.rgb*_mask1_var.g)+(_1_LayerB_var.rgb*_mask1_var.b)+(_1_LayerA_var.rgb*_mask1_var.a)+((_2_LayerR_var.rgb*_mask2_var.r)+(_2_LayerG_var.rgb*_mask2_var.g)+(_2_LayerB_var.rgb*_mask2_var.b)))*_Main_color.rgb);
                float node_5118 = ((_2_LayerR_var.a*_mask2_var.r*_Spc_int)+(_1_LayerG_var.a*_mask1_var.a*_Spc_int));
                float3 specColor = float3(node_5118,node_5118,node_5118);
                float roughness = 1.0 - _Gloss_int;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
