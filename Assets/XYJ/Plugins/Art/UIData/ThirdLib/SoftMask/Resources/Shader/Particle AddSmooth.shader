// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Particles/Additive (Soft) Soft Mask" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0

		_Min("Min",Vector) = (0,0,0,0)
		_Max("Max",Vector) = (1,1,0,0)
		_SoftEdge("SoftEdge", Vector) = (1,1,1,1)
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend One OneMinusSrcColor
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off

	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_particles
			//#pragma multi_compile_fog
			#pragma target 3.0

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				//#ifdef SOFTPARTICLES_ON
				//float4 projPos : TEXCOORD2;
				//#endif

				float4 worldPosition : TEXCOORD1;
				float4 worldPosition2 : COLOR1;
			};

			float4 _MainTex_ST;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.worldPosition = v.vertex;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPosition2 = mul(unity_ObjectToWorld, v.vertex);
				//#ifdef SOFTPARTICLES_ON
				//o.projPos = ComputeScreenPos (o.vertex);
				//COMPUTE_EYEDEPTH(o.projPos.z);
				//#endif
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			sampler2D_float _CameraDepthTexture;
			float _InvFade;

			float2 _Min;
			float2 _Max;
			float4 _SoftEdge;

			fixed4 frag (v2f i) : SV_Target
			{
				half4 col = i.color * tex2D(_MainTex, i.texcoord);
				col.rgb *= col.a;
				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
				
				float alpha = 0;
				//return color;
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

				col *= alpha;
				return col;
			}
			ENDCG 
		}
	} 
}
}
