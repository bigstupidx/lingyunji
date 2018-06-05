// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:5579,x:34342,y:33070,varname:node_5579,prsc:2|spec-3813-OUT,gloss-5893-OUT,normal-9456-OUT,emission-7235-OUT,voffset-7286-OUT;n:type:ShaderForge.SFN_Tex2d,id:5033,x:32592,y:32881,ptovrint:False,ptlb:BenTi,ptin:_BenTi,varname:_BenTi,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:889b82cd5a082924aa54ac993f5b6f23,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:3725,x:32592,y:32721,ptovrint:False,ptlb:BenTiColor,ptin:_BenTiColor,varname:_BenTiColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:1198,x:32897,y:32872,varname:node_1198,prsc:2|A-3725-RGB,B-5033-RGB;n:type:ShaderForge.SFN_Tex2d,id:5487,x:32368,y:33265,ptovrint:False,ptlb:xiaosan_texture,ptin:_xiaosan_texture,varname:_xiaosan_texture,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1c59fc00753cfa042b87f4284075407a,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Step,id:6246,x:32575,y:33158,varname:node_6246,prsc:2|A-3542-OUT,B-5487-R;n:type:ShaderForge.SFN_Slider,id:3542,x:32211,y:33150,ptovrint:False,ptlb:xiosan,ptin:_xiosan,varname:_xiosan,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3076923,max:1.5;n:type:ShaderForge.SFN_Tex2d,id:4041,x:32843,y:33385,ptovrint:False,ptlb:Bing_wenli,ptin:_Bing_wenli,varname:_Bing_wenli,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1c59fc00753cfa042b87f4284075407a,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:6467,x:33054,y:33022,varname:node_6467,prsc:2|A-274-RGB,B-1198-OUT,T-6246-OUT;n:type:ShaderForge.SFN_Step,id:7507,x:32895,y:33205,varname:node_7507,prsc:2|A-7521-OUT,B-5487-R;n:type:ShaderForge.SFN_Lerp,id:5519,x:33432,y:33010,varname:node_5519,prsc:2|A-1198-OUT,B-6467-OUT,T-7507-OUT;n:type:ShaderForge.SFN_Color,id:274,x:32764,y:33013,ptovrint:False,ptlb:bian_color,ptin:_bian_color,varname:_bian_color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7490196,c2:0.7490196,c3:0.7490196,c4:1;n:type:ShaderForge.SFN_Subtract,id:7521,x:32622,y:33344,varname:node_7521,prsc:2|A-3542-OUT,B-8214-OUT;n:type:ShaderForge.SFN_Slider,id:8214,x:32227,y:33485,ptovrint:False,ptlb:xiaosan_bian,ptin:_xiaosan_bian,varname:_xiaosan_bian,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.03658742,max:0.2;n:type:ShaderForge.SFN_Multiply,id:2312,x:33137,y:33282,varname:node_2312,prsc:2|A-7507-OUT,B-4041-RGB;n:type:ShaderForge.SFN_Color,id:6688,x:33095,y:33440,ptovrint:False,ptlb:xiaosan_color,ptin:_xiaosan_color,varname:_xiaosan_color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.4078432,c2:0.4627451,c3:0.5019608,c4:1;n:type:ShaderForge.SFN_Multiply,id:8441,x:33432,y:33185,varname:node_8441,prsc:2|A-2312-OUT,B-6688-RGB;n:type:ShaderForge.SFN_Add,id:1664,x:33615,y:33010,varname:node_1664,prsc:2|A-5519-OUT,B-8441-OUT;n:type:ShaderForge.SFN_Fresnel,id:4664,x:33186,y:32780,varname:node_4664,prsc:2|EXP-6826-OUT;n:type:ShaderForge.SFN_Slider,id:6826,x:32851,y:32768,ptovrint:False,ptlb:waifaguang_bian,ptin:_waifaguang_bian,varname:_waifaguang_bian,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:2;n:type:ShaderForge.SFN_Add,id:7235,x:33793,y:32950,varname:node_7235,prsc:2|A-5101-OUT,B-1664-OUT;n:type:ShaderForge.SFN_Color,id:1693,x:33234,y:32602,ptovrint:False,ptlb:waifaguang,ptin:_waifaguang,varname:_waifaguang,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.4196079,c2:0.6862745,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:9629,x:33493,y:32649,varname:node_9629,prsc:2|A-1693-RGB,B-7720-OUT;n:type:ShaderForge.SFN_Multiply,id:5101,x:33572,y:32864,varname:node_5101,prsc:2|A-9629-OUT,B-7507-OUT;n:type:ShaderForge.SFN_Tex2d,id:2093,x:33305,y:33540,ptovrint:False,ptlb:Normal_bingwenli,ptin:_Normal_bingwenli,varname:_Normal_bingwenli,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b618472098506f1408e034e4a16da08b,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:7047,x:33148,y:33719,ptovrint:False,ptlb:Normal_bingwenli_qiangdu,ptin:_Normal_bingwenli_qiangdu,varname:_Normal_bingwenli_qiangdu,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:2;n:type:ShaderForge.SFN_Multiply,id:6410,x:33703,y:33303,varname:node_6410,prsc:2|A-7507-OUT,B-5159-OUT;n:type:ShaderForge.SFN_Lerp,id:215,x:33503,y:33519,varname:node_215,prsc:2|A-5321-OUT,B-2093-RGB,T-7047-OUT;n:type:ShaderForge.SFN_Vector3,id:5321,x:33305,y:33413,varname:node_5321,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Tex2d,id:3309,x:33305,y:33876,ptovrint:False,ptlb:BenTi_normal,ptin:_BenTi_normal,varname:_BenTi_normal,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Slider,id:4327,x:33185,y:34068,ptovrint:False,ptlb:normal02,ptin:_normal02,varname:_normal02,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:2;n:type:ShaderForge.SFN_Lerp,id:4123,x:33583,y:33913,varname:node_4123,prsc:2|A-3974-OUT,B-3309-RGB,T-4327-OUT;n:type:ShaderForge.SFN_Vector3,id:3974,x:33474,y:33759,varname:node_3974,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Add,id:5159,x:33709,y:33661,varname:node_5159,prsc:2|A-215-OUT,B-4123-OUT;n:type:ShaderForge.SFN_ConstantLerp,id:7720,x:33352,y:32770,varname:node_7720,prsc:2,a:0,b:1|IN-4664-OUT;n:type:ShaderForge.SFN_Slider,id:3813,x:33727,y:32793,ptovrint:False,ptlb:specular,ptin:_specular,varname:_specular,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.579953,max:2;n:type:ShaderForge.SFN_Slider,id:5893,x:33922,y:32714,ptovrint:False,ptlb:gloss,ptin:_gloss,varname:_gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4330766,max:1;n:type:ShaderForge.SFN_Multiply,id:2054,x:34030,y:33487,varname:node_2054,prsc:2|A-2312-OUT,B-570-OUT;n:type:ShaderForge.SFN_Slider,id:6282,x:34061,y:33998,ptovrint:False,ptlb:tuqi,ptin:_tuqi,varname:_tuqi,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1335529,max:1;n:type:ShaderForge.SFN_NormalVector,id:570,x:33952,y:33674,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:7286,x:34343,y:33618,varname:node_7286,prsc:2|A-2054-OUT,B-6282-OUT;n:type:ShaderForge.SFN_Add,id:9456,x:33943,y:33244,varname:node_9456,prsc:2|A-1198-OUT,B-6410-OUT;proporder:5033-3725-3309-4327-5487-3542-6688-8214-274-4041-6826-1693-2093-7047-3813-5893-6282;pass:END;sub:END;*/

Shader "Shader Forge/dongjie_vogeo" {
    Properties {
        _BenTi ("BenTi", 2D) = "white" {}
        _BenTiColor ("BenTiColor", Color) = (1,1,1,1)
        _BenTi_normal ("BenTi_normal", 2D) = "bump" {}
        _normal02 ("normal02", Range(0, 2)) = 2
        _xiaosan_texture ("xiaosan_texture", 2D) = "white" {}
        _xiosan ("xiosan", Range(0, 1.5)) = 0.3076923
        _xiaosan_color ("xiaosan_color", Color) = (0.4078432,0.4627451,0.5019608,1)
        _xiaosan_bian ("xiaosan_bian", Range(0, 0.2)) = 0.03658742
        _bian_color ("bian_color", Color) = (0.7490196,0.7490196,0.7490196,1)
        _Bing_wenli ("Bing_wenli", 2D) = "white" {}
        _waifaguang_bian ("waifaguang_bian", Range(0, 2)) = 2
        _waifaguang ("waifaguang", Color) = (0.4196079,0.6862745,1,1)
        _Normal_bingwenli ("Normal_bingwenli", 2D) = "bump" {}
        _Normal_bingwenli_qiangdu ("Normal_bingwenli_qiangdu", Range(0, 2)) = 2
        _specular ("specular", Range(0, 2)) = 1.579953
        _gloss ("gloss", Range(0, 1)) = 0.4330766
        _tuqi ("tuqi", Range(0, 1)) = 0.1335529
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
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _LightColor0;
            uniform sampler2D _BenTi; uniform float4 _BenTi_ST;
            uniform float4 _BenTiColor;
            uniform sampler2D _xiaosan_texture; uniform float4 _xiaosan_texture_ST;
            uniform float _xiosan;
            uniform sampler2D _Bing_wenli; uniform float4 _Bing_wenli_ST;
            uniform float4 _bian_color;
            uniform float _xiaosan_bian;
            uniform float4 _xiaosan_color;
            uniform float _waifaguang_bian;
            uniform float4 _waifaguang;
            uniform sampler2D _Normal_bingwenli; uniform float4 _Normal_bingwenli_ST;
            uniform float _Normal_bingwenli_qiangdu;
            uniform sampler2D _BenTi_normal; uniform float4 _BenTi_normal_ST;
            uniform float _normal02;
            uniform float _specular;
            uniform float _gloss;
            uniform float _tuqi;
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
                float4 _xiaosan_texture_var = tex2Dlod(_xiaosan_texture,float4(TRANSFORM_TEX(o.uv0, _xiaosan_texture),0.0,0));
                float node_7507 = step((_xiosan-_xiaosan_bian),_xiaosan_texture_var.r);
                float4 _Bing_wenli_var = tex2Dlod(_Bing_wenli,float4(TRANSFORM_TEX(o.uv0, _Bing_wenli),0.0,0));
                float3 node_2312 = (node_7507*_Bing_wenli_var.rgb);
                v.vertex.xyz += ((node_2312*v.normal)*_tuqi);
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
                float4 _BenTi_var = tex2D(_BenTi,TRANSFORM_TEX(i.uv0, _BenTi));
                float3 node_1198 = (_BenTiColor.rgb*_BenTi_var.rgb);
                float4 _xiaosan_texture_var = tex2D(_xiaosan_texture,TRANSFORM_TEX(i.uv0, _xiaosan_texture));
                float node_7507 = step((_xiosan-_xiaosan_bian),_xiaosan_texture_var.r);
                float3 _Normal_bingwenli_var = UnpackNormal(tex2D(_Normal_bingwenli,TRANSFORM_TEX(i.uv0, _Normal_bingwenli)));
                float3 _BenTi_normal_var = UnpackNormal(tex2D(_BenTi_normal,TRANSFORM_TEX(i.uv0, _BenTi_normal)));
                float3 normalLocal = (node_1198+(node_7507*(lerp(float3(0,0,1),_Normal_bingwenli_var.rgb,_Normal_bingwenli_qiangdu)+lerp(float3(0,0,1),_BenTi_normal_var.rgb,_normal02))));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_specular,_specular,_specular);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
////// Emissive:
                float4 _Bing_wenli_var = tex2D(_Bing_wenli,TRANSFORM_TEX(i.uv0, _Bing_wenli));
                float3 node_2312 = (node_7507*_Bing_wenli_var.rgb);
                float3 emissive = (((_waifaguang.rgb*lerp(0,1,pow(1.0-max(0,dot(normalDirection, viewDirection)),_waifaguang_bian)))*node_7507)+(lerp(node_1198,lerp(_bian_color.rgb,node_1198,step(_xiosan,_xiaosan_texture_var.r)),node_7507)+(node_2312*_xiaosan_color.rgb)));
/// Final Color:
                float3 finalColor = specular + emissive;
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
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _LightColor0;
            uniform sampler2D _BenTi; uniform float4 _BenTi_ST;
            uniform float4 _BenTiColor;
            uniform sampler2D _xiaosan_texture; uniform float4 _xiaosan_texture_ST;
            uniform float _xiosan;
            uniform sampler2D _Bing_wenli; uniform float4 _Bing_wenli_ST;
            uniform float4 _bian_color;
            uniform float _xiaosan_bian;
            uniform float4 _xiaosan_color;
            uniform float _waifaguang_bian;
            uniform float4 _waifaguang;
            uniform sampler2D _Normal_bingwenli; uniform float4 _Normal_bingwenli_ST;
            uniform float _Normal_bingwenli_qiangdu;
            uniform sampler2D _BenTi_normal; uniform float4 _BenTi_normal_ST;
            uniform float _normal02;
            uniform float _specular;
            uniform float _gloss;
            uniform float _tuqi;
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
                float4 _xiaosan_texture_var = tex2Dlod(_xiaosan_texture,float4(TRANSFORM_TEX(o.uv0, _xiaosan_texture),0.0,0));
                float node_7507 = step((_xiosan-_xiaosan_bian),_xiaosan_texture_var.r);
                float4 _Bing_wenli_var = tex2Dlod(_Bing_wenli,float4(TRANSFORM_TEX(o.uv0, _Bing_wenli),0.0,0));
                float3 node_2312 = (node_7507*_Bing_wenli_var.rgb);
                v.vertex.xyz += ((node_2312*v.normal)*_tuqi);
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
                float4 _BenTi_var = tex2D(_BenTi,TRANSFORM_TEX(i.uv0, _BenTi));
                float3 node_1198 = (_BenTiColor.rgb*_BenTi_var.rgb);
                float4 _xiaosan_texture_var = tex2D(_xiaosan_texture,TRANSFORM_TEX(i.uv0, _xiaosan_texture));
                float node_7507 = step((_xiosan-_xiaosan_bian),_xiaosan_texture_var.r);
                float3 _Normal_bingwenli_var = UnpackNormal(tex2D(_Normal_bingwenli,TRANSFORM_TEX(i.uv0, _Normal_bingwenli)));
                float3 _BenTi_normal_var = UnpackNormal(tex2D(_BenTi_normal,TRANSFORM_TEX(i.uv0, _BenTi_normal)));
                float3 normalLocal = (node_1198+(node_7507*(lerp(float3(0,0,1),_Normal_bingwenli_var.rgb,_Normal_bingwenli_qiangdu)+lerp(float3(0,0,1),_BenTi_normal_var.rgb,_normal02))));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_specular,_specular,_specular);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/// Final Color:
                float3 finalColor = specular;
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
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            #pragma glsl
            uniform sampler2D _xiaosan_texture; uniform float4 _xiaosan_texture_ST;
            uniform float _xiosan;
            uniform sampler2D _Bing_wenli; uniform float4 _Bing_wenli_ST;
            uniform float _xiaosan_bian;
            uniform float _tuqi;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 _xiaosan_texture_var = tex2Dlod(_xiaosan_texture,float4(TRANSFORM_TEX(o.uv0, _xiaosan_texture),0.0,0));
                float node_7507 = step((_xiosan-_xiaosan_bian),_xiaosan_texture_var.r);
                float4 _Bing_wenli_var = tex2Dlod(_Bing_wenli,float4(TRANSFORM_TEX(o.uv0, _Bing_wenli),0.0,0));
                float3 node_2312 = (node_7507*_Bing_wenli_var.rgb);
                v.vertex.xyz += ((node_2312*v.normal)*_tuqi);
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
