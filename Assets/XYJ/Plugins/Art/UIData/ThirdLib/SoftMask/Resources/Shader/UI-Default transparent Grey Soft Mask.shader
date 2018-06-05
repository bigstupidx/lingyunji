// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "UI/Default transparent Grey Soft Mask"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_Width ("Font Width",Range(1,3)) = 1.2 		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	
		_SoftEdge("SoftEdge", Vector) = (1,1,1,1)
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
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			Name "Default"
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				float4 worldPosition2 : COLOR1;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;
			fixed _Width;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			float2 _Min;
			float2 _Max;
			float4 _SoftEdge;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
				OUT.worldPosition2 = mul(unity_ObjectToWorld, IN.vertex);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
				
				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif
				color.a *= _Width;

				float alpha = 0;
				if (IN.worldPosition2.x <= _Min.x || IN.worldPosition2.x >= _Max.x || IN.worldPosition2.y <= _Min.y || IN.worldPosition2.y >= _Max.y)
				{
					alpha = 0;
				}
				else // It's in the mask rectangle, so apply the alpha of the mask provided.
				{
					alpha = 1;
					if (IN.worldPosition2.x - _Min.x < _SoftEdge.x)
						alpha *= (IN.worldPosition2.x - _Min.x) / _SoftEdge.x;
					else if (_Max.x - IN.worldPosition2.x < _SoftEdge.z)
						alpha *= (_Max.x - IN.worldPosition2.x) / _SoftEdge.z;

					if (IN.worldPosition2.y - _Min.y < _SoftEdge.y)
						alpha *= (IN.worldPosition2.y - _Min.y) / _SoftEdge.y;
					else if (_Max.y - IN.worldPosition2.y < _SoftEdge.w)
						alpha *= (_Max.y - IN.worldPosition2.y) / _SoftEdge.w;
				}


				float grey = dot(color.rgb, fixed3(0.22, 0.707, 0.071));
				return half4(grey, grey, grey, color.a * alpha);
			}
		ENDCG
		}
	}
}
