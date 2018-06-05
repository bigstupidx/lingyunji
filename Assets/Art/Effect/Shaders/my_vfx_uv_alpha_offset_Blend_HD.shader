// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:Mobile/Particles/Alpha Blended,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:33813,y:33024,varname:node_4795,prsc:2|emission-2393-OUT,custl-291-OUT,alpha-8184-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31248,y:32015,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2393,x:31895,y:32114,varname:node_2393,prsc:2|A-6074-RGB,B-2053-RGB,C-797-RGB,D-9248-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:30470,y:32968,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:30924,y:32329,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:9248,x:31136,y:32507,varname:node_9248,prsc:2,v1:2;n:type:ShaderForge.SFN_TexCoord,id:733,x:30463,y:32566,varname:node_733,prsc:2,uv:0;n:type:ShaderForge.SFN_ValueProperty,id:1480,x:31393,y:32718,ptovrint:False,ptlb:Alpha_offset_start,ptin:_Alpha_offset_start,varname:node_1480,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_RemapRange,id:3250,x:30723,y:32581,varname:node_3250,prsc:2,frmn:-1,frmx:1,tomn:-1,tomx:15|IN-733-V;n:type:ShaderForge.SFN_Add,id:471,x:31542,y:32539,varname:node_471,prsc:2|A-3250-OUT,B-1480-OUT,C-6074-A;n:type:ShaderForge.SFN_OneMinus,id:9247,x:31185,y:32894,varname:node_9247,prsc:2|IN-3250-OUT;n:type:ShaderForge.SFN_ValueProperty,id:123,x:31269,y:33086,ptovrint:False,ptlb:Alpha_offset_end,ptin:_Alpha_offset_end,varname:_Alpha_offset_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:8586,x:31767,y:32608,varname:node_8586,prsc:2|A-471-OUT,B-4052-OUT;n:type:ShaderForge.SFN_Add,id:4052,x:31520,y:32922,varname:node_4052,prsc:2|A-9247-OUT,B-123-OUT;n:type:ShaderForge.SFN_Tex2d,id:7259,x:30633,y:33431,ptovrint:False,ptlb:mask_map,ptin:_mask_map,varname:node_7259,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_If,id:4080,x:31411,y:33525,varname:node_4080,prsc:2|A-1035-OUT,B-7259-R,GT-4029-OUT,EQ-4029-OUT,LT-5498-OUT;n:type:ShaderForge.SFN_Vector1,id:4029,x:31150,y:33670,varname:node_4029,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:5498,x:31150,y:33795,varname:node_5498,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:759,x:32360,y:33416,varname:node_759,prsc:2|A-6074-A,B-1968-OUT;n:type:ShaderForge.SFN_Multiply,id:8184,x:33057,y:33116,varname:node_8184,prsc:2|A-797-A,B-759-OUT,C-8586-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4332,x:30915,y:33420,ptovrint:False,ptlb:N_mask,ptin:_N_mask,varname:node_329,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.3;n:type:ShaderForge.SFN_If,id:8864,x:31431,y:33779,varname:node_8864,prsc:2|A-1035-OUT,B-8510-OUT,GT-4029-OUT,EQ-4029-OUT,LT-5498-OUT;n:type:ShaderForge.SFN_Add,id:8510,x:31012,y:33733,varname:node_8510,prsc:2|A-7259-R,B-4598-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4598,x:30781,y:33831,ptovrint:False,ptlb:N_BY_KD,ptin:_N_BY_KD,varname:node_5828,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.01;n:type:ShaderForge.SFN_Subtract,id:585,x:31641,y:33676,varname:node_585,prsc:2|A-4080-OUT,B-8864-OUT;n:type:ShaderForge.SFN_Color,id:135,x:31578,y:33850,ptovrint:False,ptlb:C_BYcolor,ptin:_C_BYcolor,varname:node_9508,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:1609,x:31835,y:33752,varname:node_1609,prsc:2|A-585-OUT,B-135-RGB;n:type:ShaderForge.SFN_ValueProperty,id:4822,x:31641,y:34024,ptovrint:False,ptlb:N_BY_QD,ptin:_N_BY_QD,varname:node_8447,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_Multiply,id:291,x:32708,y:33553,varname:node_291,prsc:2|A-1609-OUT,B-4822-OUT;n:type:ShaderForge.SFN_Add,id:1968,x:31873,y:33605,varname:node_1968,prsc:2|A-4080-OUT,B-585-OUT;n:type:ShaderForge.SFN_Multiply,id:1035,x:31153,y:33376,varname:node_1035,prsc:2|A-2053-A,B-4332-OUT;proporder:6074-797-1480-123-7259-4332-4598-135-4822;pass:END;sub:END;*/

Shader "my shader/vfx/uv_alpha_offset_Blend_HD" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _Alpha_offset_start ("Alpha_offset_start", Float ) = 0
        _Alpha_offset_end ("Alpha_offset_end", Float ) = 0
        _mask_map ("mask_map", 2D) = "white" {}
        _N_mask ("N_mask", Float ) = 0.3
        _N_BY_KD ("N_BY_KD", Float ) = 0.01
        _C_BYcolor ("C_BYcolor", Color) = (1,0,0,1)
        _N_BY_QD ("N_BY_QD", Float ) = 3
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
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform float _Alpha_offset_start;
            uniform float _Alpha_offset_end;
            uniform sampler2D _mask_map; uniform float4 _mask_map_ST;
            uniform float _N_mask;
            uniform float _N_BY_KD;
            uniform float4 _C_BYcolor;
            uniform float _N_BY_QD;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = (_MainTex_var.rgb*i.vertexColor.rgb*_TintColor.rgb*2.0);
                float node_1035 = (i.vertexColor.a*_N_mask);
                float4 _mask_map_var = tex2D(_mask_map,TRANSFORM_TEX(i.uv0, _mask_map));
                float node_4080_if_leA = step(node_1035,_mask_map_var.r);
                float node_4080_if_leB = step(_mask_map_var.r,node_1035);
                float node_5498 = 0.0;
                float node_4029 = 1.0;
                float node_4080 = lerp((node_4080_if_leA*node_5498)+(node_4080_if_leB*node_4029),node_4029,node_4080_if_leA*node_4080_if_leB);
                float node_8864_if_leA = step(node_1035,(_mask_map_var.r+_N_BY_KD));
                float node_8864_if_leB = step((_mask_map_var.r+_N_BY_KD),node_1035);
                float node_585 = (node_4080-lerp((node_8864_if_leA*node_5498)+(node_8864_if_leB*node_4029),node_4029,node_8864_if_leA*node_8864_if_leB));
                float3 finalColor = emissive + ((node_585*_C_BYcolor.rgb)*_N_BY_QD);
                float node_3250 = (i.uv0.g*8.0+7.0);
                fixed4 finalRGBA = fixed4(finalColor,(_TintColor.a*(_MainTex_var.a*(node_4080+node_585))*((node_3250+_Alpha_offset_start+_MainTex_var.a)*((1.0 - node_3250)+_Alpha_offset_end))));
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
