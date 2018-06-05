// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'

Shader "Hidden/Eyesblack/Projector With Mask" {
	Properties {
		_Color("Color(RGB) Transparency(A)", Color) = (1, 1, 1, 1)
		_MaskTex("Mask (RGB) Trans (A)", 2D) = "white"
	}

	CGINCLUDE
	fixed4 _Color;
	sampler2D _MaskTex;
	float4x4 unity_Projector;
	struct v2f {
		float4 pos : SV_POSITION;
		float4 uv: TEXCOORD0;
	};
	
	v2f vert(float4 vertex : POSITION) {
		v2f o;
		o.pos = UnityObjectToClipPos(vertex);
		o.uv = mul(unity_Projector, vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target {
		fixed4 color = _Color;
		color.a *= tex2D(_MaskTex, i.uv).a;
		return color;
	}
	ENDCG

	SubShader {
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}
