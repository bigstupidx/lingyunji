// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Eyesblack/Image Effects/BlurryDistortion" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}
		_MaskTex("Mask (RGB)", 2D) = "white" {}
		_DistortTex("Distort (RGB)", 2D) = "grey" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	sampler2D _BlurTex;
	sampler2D _MaskTex;
	sampler2D _DistortTex;
	half _DistortTexCoordScale;
	half _DistortIntensity;
	half _SampleStrength;

	uniform half4 _MainTex_TexelSize;
	uniform half4 _Parameter;

	struct v2f {
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		half2 uv2 : TEXCOORD1;
		half2 uv3 : TEXCOORD2;
	};

	v2f vert(appdata_img v) {
		v2f o;

		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord;
		o.uv2 = v.texcoord;
#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0.0)
			o.uv2.y = 1.0 - o.uv2.y;
#endif
		o.uv3 = o.uv * _DistortTexCoordScale;
		
		return o;
	}

	fixed4 frag(v2f i) : SV_Target {
		fixed2 distort = tex2D(_DistortTex, i.uv3).rg * 2 - 1;
		fixed2 mask = tex2D(_MaskTex, i.uv).rg;
		distort *= mask.x * _DistortIntensity * _SampleStrength;
		fixed4 rawColor = tex2D(_MainTex, i.uv + distort);
		fixed4 bulrColor = tex2D(_BlurTex, i.uv2 + distort);
		fixed4 color = lerp(rawColor, bulrColor, mask.x * _SampleStrength);

		return color;
	}

	static const half4 curve4[7] = { half4(0.0205, 0.0205, 0.0205, 0), half4(0.0855, 0.0855, 0.0855, 0), half4(0.232, 0.232, 0.232, 0),
		half4(0.324, 0.324, 0.324, 1), half4(0.232, 0.232, 0.232, 0), half4(0.0855, 0.0855, 0.0855, 0), half4(0.0205, 0.0205, 0.0205, 0) };

	struct v2f_withBlurCoords8
	{
		float4 pos : SV_POSITION;
		half4 uv : TEXCOORD0;
		half2 offs : TEXCOORD1;
	};


	v2f_withBlurCoords8 vertBlurHorizontal(appdata_img v)
	{
		v2f_withBlurCoords8 o;
		o.pos = UnityObjectToClipPos(v.vertex);

		o.uv = half4(v.texcoord.xy, 1, 1);
		o.offs = _MainTex_TexelSize.xy * half2(1.0, 0.0) * _Parameter.x;

		return o;
	}

	v2f_withBlurCoords8 vertBlurVertical(appdata_img v)
	{
		v2f_withBlurCoords8 o;
		o.pos = UnityObjectToClipPos(v.vertex);

		o.uv = half4(v.texcoord.xy, 1, 1);
		o.offs = _MainTex_TexelSize.xy * half2(0.0, 1.0) * _Parameter.x;

		return o;
	}

	half4 fragBlur8(v2f_withBlurCoords8 i) : SV_Target
	{
		half2 uv = i.uv.xy;
		half2 netFilterWidth = i.offs;
		half2 coords = uv - netFilterWidth * 2.0;

		half4 color = 0;
		for (int l = 0; l < 5; l++)
		{
			half4 tap = tex2D(_MainTex, coords);
				color += tap;// *curve4[l];
			coords += netFilterWidth;
		}
		color /= 5;
		return color;
	}
	ENDCG

	SubShader {
		ZTest Off Cull Off ZWrite Off Blend Off

		// 0
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}

		// 1
		Pass {
			ZTest Always
			Cull Off

			CGPROGRAM
			#pragma vertex vertBlurVertical
			#pragma fragment fragBlur8
			ENDCG
		}

		// 2
		Pass {
			ZTest Always
			Cull Off

			CGPROGRAM
			#pragma vertex vertBlurHorizontal
			#pragma fragment fragBlur8
			ENDCG
		}
	
	}

	FallBack Off
}
