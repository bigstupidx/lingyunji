// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UI/Button"  
{  
    Properties  
    {  
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}  
        _Color ("Tint", Color) = (1,1,1,1)  
          
        _StencilComp ("Stencil Comparison", Float) = 8  
        _Stencil ("Stencil ID", Float) = 0  
        _StencilOp ("Stencil Operation", Float) = 0  
        _StencilWriteMask ("Stencil Write Mask", Float) = 255  
        _StencilReadMask ("Stencil Read Mask", Float) = 255  
  
        _ColorMask ("Color Mask", Float) = 15  
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
        CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag  
            #include "UnityCG.cginc"  
              
            struct appdata_t  
            {  
                float4 vertex   : POSITION;  
                float4 color    : COLOR;  
                float2 texcoord : TEXCOORD0;  
            };  
  
            struct v2f  
            {  
                float4 vertex   : SV_POSITION;  
                fixed4 color    : COLOR;  
                half2 texcoord  : TEXCOORD0;  
            };  
              
            int _type;  
            fixed4 _Color;  
  
            v2f vert(appdata_t IN)  
            {  
                v2f OUT;  
                OUT.vertex = UnityObjectToClipPos(IN.vertex);  
                OUT.texcoord = IN.texcoord;  
#ifdef UNITY_HALF_TEXEL_OFFSET  
                OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);  
#endif  
                OUT.color = IN.color * _Color;  
                return OUT;  
            }  
  
            sampler2D _MainTex;  
  
            fixed4 frag(v2f IN) : SV_Target  
            {  
                half4 color = tex2D(_MainTex, IN.texcoord) * IN.color;  
                clip (color.a - 0.01);  
  
                if(_type == 1)  
                {  
                    color = color * 1.6f;  
                }  
                else if(_type == 2)  
                {  
                    color = color * 0.8f;  
                }  
                else if(_type == 3)  
                {  
                    float grey = dot(color.rgb, fixed3(0.22, 0.707, 0.071));     
                    color = half4(grey,grey,grey,color.a);    
                }  
                return color;  
            }  
        ENDCG  
        }  
    }  
}  