// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UI/Default Grey Soft Alpha Masked"
{
	Properties
	{
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
        _MaskRotation ("Mask Rotation in Radians", Float) = 0
		//_AlphaTex("Alpha Mask", 2D) = "white" {}
		_IsThisText("Is This Text?", Float) = 0

		_SoftEdge_ST("SoftEdge ST", Vector) = (0,0,0,0)

		_SoftEdge("SoftEdge", Vector) = (0.1,0.1,0.1,0.1)
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha
		
		Pass
		{
		CGPROGRAM
			#include "UnityCG.cginc"
			
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _MaskRotation;
			
			float _IsThisText;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
				float2 uvMain : TEXCOORD1;
				float2 uvAlpha : TEXCOORD2;
			};
			
			fixed4 _Color;
			float4 _MainTex_ST;
			float4 _SoftEdge_ST;
			float4 _SoftEdge;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uvMain = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color * _Color;
				
				o.uvAlpha = v.vertex.xy;

				float s = sin(_MaskRotation);
				float c = cos(_MaskRotation);
				float2x2 rotationMatrix = float2x2(c, -s, s, c);
				o.uvAlpha = mul(o.uvAlpha, rotationMatrix);

				o.uvAlpha = o.uvAlpha * _SoftEdge_ST.xy + _SoftEdge_ST.zw;

				o.pos = UnityPixelSnap(o.pos);
				
				return o;
			}

			float soft = 0.3;

			half4 frag (v2f i) : COLOR
			{
				float2 alphaCoords = i.uvAlpha;
	
				half4 texcol = tex2D(_MainTex, i.uvMain);
				texcol.a *= i.color.a;
				texcol.rgb = clamp(texcol.rgb + _IsThisText, 0, 1) * i.color.rgb;

				if (alphaCoords.x < 0 || alphaCoords.x > 1 || alphaCoords.y < 0 || alphaCoords.y > 1)
					texcol.a = 0;
				else
				{
					if (alphaCoords.x < _SoftEdge.x)
						texcol.a *= alphaCoords.x / _SoftEdge.x;
					else if (alphaCoords.x > (1 - _SoftEdge.z))
						texcol.a *= (1 - alphaCoords.x) / _SoftEdge.z;

					if (alphaCoords.y < _SoftEdge.y)
						texcol.a *= alphaCoords.y / _SoftEdge.y;
					else if (alphaCoords.y >(1 - _SoftEdge.w))
						texcol.a *= (1 - alphaCoords.y) / _SoftEdge.w;
				}

				texcol.rgb *= texcol.a;
				texcol.rgb = dot(texcol.rgb, fixed3(0.22, 0.707, 0.071));

				return texcol;
			}
			
		ENDCG
		}
	}
	
	Fallback "Unlit/Texture"
}
