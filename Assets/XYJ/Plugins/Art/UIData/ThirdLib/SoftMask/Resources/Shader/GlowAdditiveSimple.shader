// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Effects/GlowAdditiveSimple Soft Mask" {
	Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_CoreColor ("Core Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_TintStrength ("Tint Color Strength", Range(0, 5)) = 1
	_CoreStrength ("Core Color Strength", Range(0, 8)) = 1
	_CutOutLightCore ("CutOut Light Core", Range(0, 1)) = 0.5
	
		_Min("Min",Vector) = (0,0,0,0)
		_Max("Max",Vector) = (1,1,0,0)
		_SoftEdge("SoftEdge", Vector) = (1,1,1,1)
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	AlphaTest Greater .01
	ColorMask RGB
	Cull Off 
	Lighting Off 
	ZWrite Off 
	Fog { Color (0,0,0,0) }
	
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

		
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			fixed4 _CoreColor;
			float _CutOutLightCore;
			float _TintStrength;
			float _CoreStrength;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				
				float4 worldPosition : TEXCOORD1;
				float4 worldPosition2 : COLOR;
			};
			
			float4 _MainTex_ST;
			float2 _Min;
			float2 _Max;
			float4 _SoftEdge;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.worldPosition = v.vertex;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPosition2 = mul(unity_ObjectToWorld, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.texcoord);
				fixed4 col = (_TintColor * tex.g * _TintStrength + tex.r * _CoreColor * _CoreStrength  - _CutOutLightCore); 

				float alpha = 0;
				if (i.worldPosition2.x <= _Min.x || i.worldPosition2.x >= _Max.x || i.worldPosition2.y <= _Min.y || i.worldPosition2.y >= _Max.y)
				{
					alpha = 0;
				}
				else // It's in the mask rectangle, so apply the alpha of the mask provided.
				{
					alpha = 1;
					if (i.worldPosition2.x - _Min.x < _SoftEdge.x)
						alpha *= (i.worldPosition2.x - _Min.x) / _SoftEdge.x;
					else if (_Max.x - i.worldPosition2.x < _SoftEdge.z)
						alpha *= (_Max.x - i.worldPosition2.x) / _SoftEdge.z;

					if (i.worldPosition2.y - _Min.y < _SoftEdge.y)
						alpha *= (i.worldPosition2.y - _Min.y) / _SoftEdge.y;
					else if (_Max.y - i.worldPosition2.y < _SoftEdge.w)
						alpha *= (_Max.y - i.worldPosition2.y) / _SoftEdge.w;
				}

				col.a = alpha;
				col = clamp(col, 0, 255);
				return col;
			}
			ENDCG 
		}
	}	
}
}
