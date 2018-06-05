// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.05 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.05;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:2,dpts:2,wrdp:False,dith:0,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:161,x:33871,y:32739,varname:node_161,prsc:2|emission-7541-RGB,alpha-3500-OUT,clip-7017-OUT;n:type:ShaderForge.SFN_Tex2d,id:7541,x:33166,y:32628,ptovrint:False,ptlb:mainTex,ptin:_mainTex,varname:node_7541,prsc:2,tex:b418ba7883c2a5440bbaeda55bf0946d,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9302,x:33026,y:32865,ptovrint:False,ptlb:mask,ptin:_mask,varname:node_9302,prsc:2,tex:0c7d78ee1d847c6449b0673e2e10f4e4,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:7017,x:33372,y:33057,varname:node_7017,prsc:2|A-9302-R,B-9236-OUT;n:type:ShaderForge.SFN_Slider,id:9236,x:32921,y:33190,ptovrint:False,ptlb:qiangdu,ptin:_qiangdu,varname:node_9236,prsc:2,min:0,cur:1.2,max:10;n:type:ShaderForge.SFN_Multiply,id:3500,x:33476,y:32895,varname:node_3500,prsc:2|A-9302-G,B-7017-OUT;proporder:7541-9302-9236;pass:END;sub:END;*/

Shader "Custom/CreateRoleClipShader" {
    Properties {
        _mainTex ("mainTex", 2D) = "white" {}
        _mask ("mask", 2D) = "white" {}
        _radius ("radius", Range(0, 20)) = 1
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _mainTex; 
            sampler2D _mask; 
			
			float4 _mainTex_ST;
			float4 _mask_ST;
			
            float _radius;

			struct v2f
			{
				float4 pos:POSITION;
				float2 uv:TEXCOORD0;
				
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

			fixed4 frag(v2f i) :COLOR
			{
                float4 _mask_var = tex2D(_mask,i.uv);
				float r = (_mask_var.a * _radius);
                clip(r + 0.2);
                float4 _mainTex_var = tex2D(_mainTex, i.uv);
                float3 finalColor = _mainTex_var.rgb;
				float a = 1 - _mask_var.a * r;
				
                return fixed4(finalColor, a);
				//return fixed4(float3(1,1,1), max((_mask_var.a * r), 0));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
