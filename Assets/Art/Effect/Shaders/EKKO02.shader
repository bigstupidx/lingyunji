// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.13 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.13;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,nrsp:0,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:False,dith:0,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:33676,y:32612,varname:node_1,prsc:2|normal-133-RGB,emission-6166-OUT,custl-105-OUT,alpha-82-OUT,clip-7917-OUT,refract-255-OUT;n:type:ShaderForge.SFN_Color,id:81,x:32399,y:32276,ptovrint:False,ptlb:Me_81,ptin:_Me_81,varname:node_898,prsc:2,glob:False,c1:1,c2:0.1764706,c3:0.1764706,c4:1;n:type:ShaderForge.SFN_Slider,id:82,x:33097,y:32956,ptovrint:False,ptlb:node_82,ptin:_node_82,varname:node_9794,prsc:2,min:0,cur:1,max:1;n:type:ShaderForge.SFN_NormalVector,id:83,x:31811,y:32637,prsc:2,pt:True;n:type:ShaderForge.SFN_LightVector,id:93,x:32222,y:32740,varname:node_93,prsc:2;n:type:ShaderForge.SFN_Dot,id:98,x:32376,y:32541,varname:node_98,prsc:2,dt:0|A-83-OUT,B-93-OUT;n:type:ShaderForge.SFN_Vector1,id:99,x:32480,y:32957,varname:node_99,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Add,id:100,x:32719,y:32668,varname:node_100,prsc:2|A-103-OUT,B-99-OUT;n:type:ShaderForge.SFN_Multiply,id:101,x:32860,y:32590,varname:node_101,prsc:2|A-100-OUT,B-100-OUT;n:type:ShaderForge.SFN_Multiply,id:102,x:33015,y:32304,varname:node_102,prsc:2|A-81-RGB,B-101-OUT;n:type:ShaderForge.SFN_Multiply,id:103,x:32585,y:32644,varname:node_103,prsc:2|A-104-OUT,B-98-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:104,x:32554,y:32404,varname:node_104,prsc:2;n:type:ShaderForge.SFN_Multiply,id:105,x:33264,y:32687,varname:node_105,prsc:2|A-132-OUT,B-106-RGB;n:type:ShaderForge.SFN_LightColor,id:106,x:33004,y:32814,varname:node_106,prsc:2;n:type:ShaderForge.SFN_ViewReflectionVector,id:120,x:32052,y:32927,varname:node_120,prsc:2;n:type:ShaderForge.SFN_Dot,id:121,x:32269,y:32905,varname:node_121,prsc:2,dt:1|A-93-OUT,B-120-OUT;n:type:ShaderForge.SFN_Power,id:126,x:32490,y:33093,varname:node_126,prsc:2|VAL-121-OUT,EXP-127-OUT;n:type:ShaderForge.SFN_Exp,id:127,x:32312,y:33158,varname:node_127,prsc:2,et:0|IN-129-OUT;n:type:ShaderForge.SFN_Slider,id:129,x:31830,y:33219,ptovrint:False,ptlb:node_129,ptin:_node_129,varname:node_4298,prsc:2,min:1,cur:3.329159,max:8;n:type:ShaderForge.SFN_Slider,id:130,x:32545,y:33185,ptovrint:False,ptlb:node_130,ptin:_node_130,varname:node_776,prsc:2,min:0,cur:0.488266,max:1;n:type:ShaderForge.SFN_Multiply,id:131,x:32743,y:33033,varname:node_131,prsc:2|A-130-OUT,B-126-OUT;n:type:ShaderForge.SFN_Add,id:132,x:32743,y:32777,varname:node_132,prsc:2|A-102-OUT,B-131-OUT,C-344-OUT;n:type:ShaderForge.SFN_Tex2d,id:133,x:33185,y:32852,ptovrint:False,ptlb:node_133,ptin:_node_133,varname:node_2321,prsc:2,tex:bbab0a6f7bae9cf42bf057d8ee2755f6,ntxv:3,isnm:True;n:type:ShaderForge.SFN_NormalVector,id:228,x:32773,y:33427,prsc:2,pt:False;n:type:ShaderForge.SFN_Transform,id:232,x:33051,y:33446,varname:node_232,prsc:2,tffrom:0,tfto:3|IN-228-OUT;n:type:ShaderForge.SFN_ComponentMask,id:237,x:33191,y:33584,varname:node_237,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-232-XYZ;n:type:ShaderForge.SFN_Multiply,id:255,x:33719,y:33268,varname:node_255,prsc:2|A-291-OUT,B-256-OUT;n:type:ShaderForge.SFN_Slider,id:256,x:33343,y:33516,ptovrint:False,ptlb:node_256,ptin:_node_256,varname:node_7589,prsc:2,min:-0.5,cur:-0.06562531,max:0;n:type:ShaderForge.SFN_ComponentMask,id:290,x:33719,y:33182,varname:node_290,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-133-RGB;n:type:ShaderForge.SFN_Add,id:291,x:33465,y:33349,varname:node_291,prsc:2|A-290-OUT,B-237-OUT;n:type:ShaderForge.SFN_Fresnel,id:343,x:32137,y:33441,varname:node_343,prsc:2|NRM-83-OUT,EXP-345-OUT;n:type:ShaderForge.SFN_Multiply,id:344,x:32410,y:33301,varname:node_344,prsc:2|A-343-OUT,B-101-OUT;n:type:ShaderForge.SFN_Slider,id:345,x:31961,y:33613,ptovrint:False,ptlb:node_345,ptin:_node_345,varname:node_9244,prsc:2,min:0,cur:0.5340255,max:1;n:type:ShaderForge.SFN_Multiply,id:7917,x:33474,y:33068,varname:node_7917,prsc:2|A-7993-OUT,B-9047-OUT;n:type:ShaderForge.SFN_Slider,id:9047,x:33051,y:33466,ptovrint:False,ptlb:s,ptin:_s,varname:node_9047,prsc:2,min:0,cur:15,max:15;n:type:ShaderForge.SFN_Tex2d,id:2176,x:32976,y:33063,ptovrint:False,ptlb:node_2176,ptin:_node_2176,varname:node_2176,prsc:2,tex:00c2bb747d65cd04898933bf5cb3385b,ntxv:2,isnm:False|UVIN-9012-UVOUT;n:type:ShaderForge.SFN_Panner,id:9012,x:32859,y:33033,varname:node_9012,prsc:2,spu:-0.2,spv:0.3;n:type:ShaderForge.SFN_Vector1,id:9520,x:33185,y:33038,varname:node_9520,prsc:2,v1:5;n:type:ShaderForge.SFN_Multiply,id:6166,x:33330,y:33068,varname:node_6166,prsc:2|A-9520-OUT,B-9693-OUT;n:type:ShaderForge.SFN_Tex2d,id:926,x:32813,y:33254,ptovrint:False,ptlb:node_926,ptin:_node_926,varname:node_926,prsc:2,tex:00c2bb747d65cd04898933bf5cb3385b,ntxv:2,isnm:False|UVIN-3940-UVOUT;n:type:ShaderForge.SFN_Panner,id:3940,x:32557,y:33301,varname:node_3940,prsc:2,spu:0.1,spv:-0.2;n:type:ShaderForge.SFN_Multiply,id:9693,x:33070,y:33131,varname:node_9693,prsc:2|A-2176-RGB,B-926-RGB;n:type:ShaderForge.SFN_Multiply,id:7993,x:33278,y:33202,varname:node_7993,prsc:2|A-2176-B,B-1202-OUT;n:type:ShaderForge.SFN_Vector1,id:3661,x:33172,y:33353,varname:node_3661,prsc:2,v1:3;n:type:ShaderForge.SFN_Multiply,id:1202,x:33008,y:33287,varname:node_1202,prsc:2|A-3661-OUT,B-926-B;proporder:81-82-129-130-133-256-345-9047-2176-926;pass:END;sub:END;*/

Shader "Shader Forge/EKKO02" {
    Properties {
        _Me_81 ("Me_81", Color) = (1,0.1764706,0.1764706,1)
        _node_82 ("node_82", Range(0, 1)) = 1
        _node_129 ("node_129", Range(1, 8)) = 3.329159
        _node_130 ("node_130", Range(0, 1)) = 0.488266
        _node_133 ("node_133", 2D) = "bump" {}
        _node_256 ("node_256", Range(-0.5, 0)) = -0.06562531
        _node_345 ("node_345", Range(0, 1)) = 0.5340255
        _s ("s", Range(0, 15)) = 15
        _node_2176 ("node_2176", 2D) = "black" {}
        _node_926 ("node_926", 2D) = "black" {}
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
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform float4 _Me_81;
            uniform float _node_82;
            uniform float _node_129;
            uniform float _node_130;
            uniform sampler2D _node_133; uniform float4 _node_133_ST;
            uniform float _node_256;
            uniform float _node_345;
            uniform float _s;
            uniform sampler2D _node_2176; uniform float4 _node_2176_ST;
            uniform sampler2D _node_926; uniform float4 _node_926_ST;
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
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex);
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
                float3 _node_133_var = UnpackNormal(tex2D(_node_133,TRANSFORM_TEX(i.uv0, _node_133)));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + ((_node_133_var.rgb.rg+mul( UNITY_MATRIX_V, float4(i.normalDir,0) ).xyz.rgb.rg)*_node_256);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = _node_133_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float4 node_376 = _Time + _TimeEditor;
                float2 node_9012 = (i.uv0+node_376.g*float2(-0.2,0.3));
                float4 _node_2176_var = tex2D(_node_2176,TRANSFORM_TEX(node_9012, _node_2176));
                float2 node_3940 = (i.uv0+node_376.g*float2(0.1,-0.2));
                float4 _node_926_var = tex2D(_node_926,TRANSFORM_TEX(node_3940, _node_926));
                clip(((_node_2176_var.b*(3.0*_node_926_var.b))*_s) - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
////// Emissive:
                float3 emissive = (5.0*(_node_2176_var.rgb*_node_926_var.rgb));
                float node_100 = ((attenuation*dot(normalDirection,lightDirection))+0.5);
                float node_101 = (node_100*node_100);
                float3 finalColor = emissive + (((_Me_81.rgb*node_101)+(_node_130*pow(max(0,dot(lightDirection,viewReflectDirection)),exp(_node_129)))+(pow(1.0-max(0,dot(normalDirection, viewDirection)),_node_345)*node_101))*_LightColor0.rgb);
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,_node_82),1);
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
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform float4 _Me_81;
            uniform float _node_82;
            uniform float _node_129;
            uniform float _node_130;
            uniform sampler2D _node_133; uniform float4 _node_133_ST;
            uniform float _node_256;
            uniform float _node_345;
            uniform float _s;
            uniform sampler2D _node_2176; uniform float4 _node_2176_ST;
            uniform sampler2D _node_926; uniform float4 _node_926_ST;
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
                LIGHTING_COORDS(6,7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
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
                float3 _node_133_var = UnpackNormal(tex2D(_node_133,TRANSFORM_TEX(i.uv0, _node_133)));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + ((_node_133_var.rgb.rg+mul( UNITY_MATRIX_V, float4(i.normalDir,0) ).xyz.rgb.rg)*_node_256);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalLocal = _node_133_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float4 node_6766 = _Time + _TimeEditor;
                float2 node_9012 = (i.uv0+node_6766.g*float2(-0.2,0.3));
                float4 _node_2176_var = tex2D(_node_2176,TRANSFORM_TEX(node_9012, _node_2176));
                float2 node_3940 = (i.uv0+node_6766.g*float2(0.1,-0.2));
                float4 _node_926_var = tex2D(_node_926,TRANSFORM_TEX(node_3940, _node_926));
                clip(((_node_2176_var.b*(3.0*_node_926_var.b))*_s) - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float node_100 = ((attenuation*dot(normalDirection,lightDirection))+0.5);
                float node_101 = (node_100*node_100);
                float3 finalColor = (((_Me_81.rgb*node_101)+(_node_130*pow(max(0,dot(lightDirection,viewReflectDirection)),exp(_node_129)))+(pow(1.0-max(0,dot(normalDirection, viewDirection)),_node_345)*node_101))*_LightColor0.rgb);
                return fixed4(finalColor * _node_82,0);
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
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float _s;
            uniform sampler2D _node_2176; uniform float4 _node_2176_ST;
            uniform sampler2D _node_926; uniform float4 _node_926_ST;
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
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
/////// Vectors:
                float4 node_1487 = _Time + _TimeEditor;
                float2 node_9012 = (i.uv0+node_1487.g*float2(-0.2,0.3));
                float4 _node_2176_var = tex2D(_node_2176,TRANSFORM_TEX(node_9012, _node_2176));
                float2 node_3940 = (i.uv0+node_1487.g*float2(0.1,-0.2));
                float4 _node_926_var = tex2D(_node_926,TRANSFORM_TEX(node_3940, _node_926));
                clip(((_node_2176_var.b*(3.0*_node_926_var.b))*_s) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
