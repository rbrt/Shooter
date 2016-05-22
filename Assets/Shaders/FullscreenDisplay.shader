Shader "Custom/FullscreenDisplay"
{
	Properties
	{
		_UpperScreen ("Texture", 2D) = "white" {}
		_LowerScreen ("Texture", 2D) = "white" {}
		_UpperPain ("Float", Range(0,1)) = 0
		_LowerPain ("Float", Range(0,1)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _UpperScreen;
			sampler2D _LowerScreen;

			float _UpperPain;
			float _LowerPain;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = fixed4(0,0,0,0);
				if (i.uv.y > .5)
				{
					float fallOff = distance(i.uv, half2(.5, .75)) * _UpperPain;
					i.uv.y = (i.uv.y - .5) * 2;
					col = tex2D(_UpperScreen, i.uv);
					col += (_UpperPain * fixed4(.5,0,0,1)) * fallOff;
				}
				else
				{
					float fallOff = distance(i.uv, half2(.5, .25)) * _LowerPain;
					i.uv.y = (i.uv.y) * 2;
					col = tex2D(_LowerScreen, i.uv);
					col += (_LowerPain * fixed4(.5,0,0,1)) * fallOff;
				}
				return col;
			}
			ENDCG
		}
	}
}
