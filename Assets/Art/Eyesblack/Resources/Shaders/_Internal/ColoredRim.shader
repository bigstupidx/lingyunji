// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Eyesblack/ColoredRim" {
	Properties {
		_RimColor("Rim Color", Color) = (0, 0.8, 1, 1)
		_RimWidth("Rim Width", Float) = 3

		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	fixed4 _RimColor;
	fixed _RimWidth;

	struct appdata_t {
		float4 vertex : POSITION;
		fixed3 normal : NORMAL;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		fixed rim : TEXCOORD1;
	};

	v2f vert(appdata_t v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);

		o.uv = v.texcoord;

		float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
		fixed rim = 1 - abs(dot(viewDir, v.normal));
		rim *= _RimWidth;
		rim *= rim;
		o.rim = rim;
		return o;
	}

	fixed4 frag(v2f i) : COLOR{
		fixed4 c = _RimColor;
		c.a *= i.rim;
		return c;
	}
	ENDCG


	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		
		ZWrite Off
		Blend [_SrcBlend] [_DstBlend]

		
		Pass {
			ZWrite On
			ColorMask 0
		}
		
		
		Pass {
			CGPROGRAM
			#pragma vertex vert 
			#pragma fragment frag 
			ENDCG
		}
	}
}
