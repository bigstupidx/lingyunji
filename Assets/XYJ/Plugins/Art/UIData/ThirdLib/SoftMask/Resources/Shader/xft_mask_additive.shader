// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Xffect/mask/additive Soft Mask" {
Properties {
 _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Main Texture", 2D) = "white" {}
 _MaskTex ("Mask (R)", 2D) = "white" {}
 _ScrollTimeX  ("Scroll Speed X", Float) = 0
 _ScrollTimeY  ("Scroll Speed Y", Float) = 0


	 _Min("Min",Vector) = (0,0,0,0)
	 _Max("Max",Vector) = (1,1,0,0)
	 _SoftEdge("SoftEdge", Vector) = (1,1,1,1)
}

Category {
 Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
 Blend SrcAlpha One
 Cull Off Lighting Off ZWrite Off

 BindChannels {
     Bind "Color", color
     Bind "Vertex", vertex
     Bind "TexCoord", texcoord
 }
 
 // ---- Fragment program cards
 SubShader {
     Pass {
     
         CGPROGRAM
         #pragma vertex vert
         #pragma fragment frag
         #pragma fragmentoption ARB_precision_hint_fastest
         #pragma multi_compile_particles
		 #pragma target 3.0

         #include "UnityCG.cginc"

         sampler2D _MainTex;
         sampler2D _MaskTex;
         half _ScrollTimeX;
         half _ScrollTimeY;
         fixed4 _TintColor;

		 float2 _Min;
		 float2 _Max;
		 float4 _SoftEdge;

         struct appdata_t {
             float4 vertex : POSITION;
             fixed4 color : COLOR;
             float2 texcoord : TEXCOORD0;
         };

         struct v2f {
             float4 vertex : POSITION;
             fixed4 color : COLOR;
             float2 texcoord : TEXCOORD0;

			 float4 worldPosition : TEXCOORD1;
			 float4 worldPosition2 : COLOR1;
         };
         
         v2f vert (appdata_t v)
         {
             v2f o;
			 o.worldPosition = v.vertex;
			 o.vertex = UnityObjectToClipPos(v.vertex);
			 o.worldPosition2 = mul(unity_ObjectToWorld, v.vertex);
			 o.color = v.color;
             o.texcoord = v.texcoord;
             return o;
         }
         fixed4 frag (v2f i) : COLOR
         {
             half2 uvoft = i.texcoord;
             uvoft.x += _Time.y*_ScrollTimeX;
             uvoft.y += _Time.y*_ScrollTimeY;
             fixed4 offsetColor = tex2D(_MaskTex, uvoft);
             fixed grayscale = offsetColor.r;
             fixed4 mainColor = tex2D(_MainTex, i.texcoord);

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

             float4 color = 2.0f * i.color * _TintColor * mainColor * grayscale;
			 color *= alpha;
			 return color;
         }
         ENDCG
     }
 }   
}
}