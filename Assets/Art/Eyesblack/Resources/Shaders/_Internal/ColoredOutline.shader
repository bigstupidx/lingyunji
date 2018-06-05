// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Eyesblack/ColoredOutline" {
	Properties{
		_Color("Main Color", Color) = (1, 1, 1, 1)
		_OutlineWith("Outline width", Range(0, 0.5)) = 0.05
	}


	CGINCLUDE
	#include "UnityCG.cginc"
	fixed4 _Color;
	float _OutlineWith;

	struct appdata_t {
		float4 vertex : POSITION;
		fixed3 normal : NORMAL;
	};

	struct v2f {
		float4 pos : SV_POSITION;
	};

	v2f vertInner(appdata_t v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);

		return o;
	}

	v2f vertOuter(appdata_t v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);

		float3 norm = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
		float2 offset = TransformViewToProjection(norm.xy);

		o.pos.xy += offset * o.pos.z * _OutlineWith;

		return o;
	}

	fixed4 fragInner(v2f i) : SV_Target{
		return _Color;
	}

	fixed4 fragOuter(v2f i) : SV_Target{
		return _Color;
	}
	ENDCG


	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			CGPROGRAM
			#pragma vertex vertOuter 
			#pragma fragment fragOuter 
			ENDCG
		}

		Pass {
			ColorMask G
			CGPROGRAM
			#pragma vertex vertInner 
			#pragma fragment fragInner
			ENDCG
		}
	}
}
