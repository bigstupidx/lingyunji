// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:Mobile/Diffuse,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:False,qofs:0,qpre:3,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:9361,x:34243,y:32033,varname:node_9361,prsc:2|diff-5684-OUT,emission-9873-RGB,clip-4213-OUT;n:type:ShaderForge.SFN_Tex2d,id:9873,x:33618,y:32220,varname:node_9873,prsc:2,tex:a4a5223756dc97e42b42ac2359b40434,ntxv:0,isnm:False|UVIN-5534-OUT,TEX-7445-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:7445,x:32418,y:32460,ptovrint:False,ptlb:dissolve_ram,ptin:_dissolve_ram,varname:node_7445,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a4a5223756dc97e42b42ac2359b40434,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Append,id:5534,x:32579,y:32324,varname:node_5534,prsc:2|A-9762-OUT,B-8343-OUT;n:type:ShaderForge.SFN_Vector1,id:8343,x:32208,y:32441,varname:node_8343,prsc:2,v1:0;n:type:ShaderForge.SFN_Tex2d,id:796,x:31603,y:33483,ptovrint:False,ptlb:noise,ptin:_noise,varname:node_796,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:6882,x:31210,y:32845,ptovrint:False,ptlb:dissolve_amount,ptin:_dissolve_amount,varname:node_6882,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5412629,max:1;n:type:ShaderForge.SFN_Add,id:4213,x:32741,y:32879,varname:node_4213,prsc:2|A-6148-OUT,B-796-R;n:type:ShaderForge.SFN_RemapRange,id:6148,x:32170,y:32894,varname:node_6148,prsc:2,frmn:0,frmx:1,tomn:-0.75,tomx:0.6|IN-3413-OUT;n:type:ShaderForge.SFN_OneMinus,id:3413,x:31702,y:32873,varname:node_3413,prsc:2|IN-6882-OUT;n:type:ShaderForge.SFN_RemapRange,id:6549,x:31780,y:32311,varname:node_6549,prsc:2,frmn:0,frmx:1,tomn:-4,tomx:4|IN-4213-OUT;n:type:ShaderForge.SFN_Clamp01,id:7997,x:31963,y:32392,varname:node_7997,prsc:2|IN-6549-OUT;n:type:ShaderForge.SFN_OneMinus,id:9762,x:32196,y:32237,varname:node_9762,prsc:2|IN-7997-OUT;n:type:ShaderForge.SFN_Fresnel,id:670,x:32849,y:31871,varname:node_670,prsc:2|EXP-2207-OUT;n:type:ShaderForge.SFN_Color,id:1733,x:33242,y:31780,ptovrint:False,ptlb:Main_color,ptin:_Main_color,varname:node_1733,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.05033519,c2:0.6708714,c3:0.9779412,c4:1;n:type:ShaderForge.SFN_Multiply,id:5684,x:33789,y:31852,varname:node_5684,prsc:2|A-1733-RGB,B-3111-OUT,C-7175-OUT;n:type:ShaderForge.SFN_Slider,id:2207,x:32422,y:32111,ptovrint:False,ptlb:fresnel_amount,ptin:_fresnel_amount,varname:node_2207,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.931624,max:10;n:type:ShaderForge.SFN_Add,id:3111,x:33242,y:31937,varname:node_3111,prsc:2|A-670-OUT,B-670-OUT,C-1889-RGB;n:type:ShaderForge.SFN_Slider,id:7175,x:33497,y:32023,ptovrint:False,ptlb:self_Illumin,ptin:_self_Illumin,varname:node_7175,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:4;n:type:ShaderForge.SFN_Tex2d,id:1889,x:32856,y:32132,ptovrint:False,ptlb:Base,ptin:_Base,varname:node_1889,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;proporder:1733-2207-6882-796-7445-7175-1889;pass:END;sub:END;*/

Shader "my shader/vfx/my_dissolveC" {
    Properties {
        _Main_color ("Main_color", Color) = (0.05033519,0.6708714,0.9779412,1)
        _fresnel_amount ("fresnel_amount", Range(0, 10)) = 1.931624
        _dissolve_amount ("dissolve_amount", Range(0, 1)) = 0.5412629
        _noise ("noise", 2D) = "white" {}
        _dissolve_ram ("dissolve_ram", 2D) = "white" {}
        _self_Illumin ("self_Illumin", Range(0, 4)) = 1
        _Base ("Base", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="TransparentCutout"
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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 2.0
            uniform float4 _LightColor0;
            uniform sampler2D _dissolve_ram; uniform float4 _dissolve_ram_ST;
            uniform sampler2D _noise; uniform float4 _noise_ST;
            uniform float _dissolve_amount;
            uniform float4 _Main_color;
            uniform float _fresnel_amount;
            uniform float _self_Illumin;
            uniform sampler2D _Base; uniform float4 _Base_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 _noise_var = tex2D(_noise,TRANSFORM_TEX(i.uv0, _noise));
                float node_4213 = (((1.0 - _dissolve_amount)*1.35+-0.75)+_noise_var.r);
                clip(node_4213 - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float node_670 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_fresnel_amount);
                float4 _Base_var = tex2D(_Base,TRANSFORM_TEX(i.uv0, _Base));
                float3 diffuseColor = (_Main_color.rgb*(node_670+node_670+_Base_var.rgb)*_self_Illumin);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float2 node_5534 = float2((1.0 - saturate((node_4213*8.0+-4.0))),0.0);
                float4 node_9873 = tex2D(_dissolve_ram,TRANSFORM_TEX(node_5534, _dissolve_ram));
                float3 emissive = node_9873.rgb;
/// Final Color:
                float3 finalColor = diffuse + emissive;
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
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers xbox360 xboxone ps3 ps4 psp2 
            #pragma target 2.0
            uniform sampler2D _noise; uniform float4 _noise_ST;
            uniform float _dissolve_amount;
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
            float4 frag(VertexOutput i) : COLOR {
                float4 _noise_var = tex2D(_noise,TRANSFORM_TEX(i.uv0, _noise));
                float node_4213 = (((1.0 - _dissolve_amount)*1.35+-0.75)+_noise_var.r);
                clip(node_4213 - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
