/****************************************************************
*
* Copyright 2019 © Leia Inc.  All rights reserved.
*
* NOTICE:  All information contained herein is, and remains
* the property of Leia Inc. and its suppliers, if any.  The
* intellectual and technical concepts contained herein are
* proprietary to Leia Inc. and its suppliers and may be covered
* by U.S. and Foreign Patents, patents in process, and are
* protected by trade secret or copyright law.  Dissemination of
* this information or reproduction of this materials strictly
* forbidden unless prior written permission is obtained from
* Leia Inc.
*
****************************************************************
*/
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LeiaLoft/ViewSharpening"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_a ("A", float) = 0
		_b ("B", float) = 0
	}

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;

			uniform float _a;
			uniform float _b;
			uniform float4 _MainTex_TexelSize;

			#define VIEW_COUNT 4.0

			float GetTextureWidth()
			{
				return _MainTex_TexelSize.z;
			}

			half3 GammaToLinear(half3 col)
			{
				return pow(col, 2);
			}

			half3 LinearToGamma(half3 col)
			{
				return sqrt(col);
			}

			half3 ReadTextureByOffset(float2 uv, float offset_from_center)
			{
			    return GammaToLinear(tex2D(_MainTex, uv + float2(offset_from_center / GetTextureWidth(), 0.0)));
			}

			half3 NaiveTap(v2f i)
			{
				half3 original = GammaToLinear(tex2D(_MainTex, i.uv));

				float rightOffset    = +1.0;
				float leftOffset     = -1.0;
				float farRightOffset = +2.0;
				float farLeftOffset  = -2.0;

				half3 right    = ReadTextureByOffset(i.uv, rightOffset);
				half3 left     = ReadTextureByOffset(i.uv, leftOffset);
				half3 farRight = ReadTextureByOffset(i.uv, farRightOffset);
				half3 farLeft  = ReadTextureByOffset(i.uv, farLeftOffset);

				float multiplier = (1.0 - 2.0 * _a - 2.0 * _b);
				half3 right_with_coef = _a * right;
				half3 left_with_coef  = _a * left;
				half3 far_right_with_coef = _b * farRight;
				half3 far_left_with_coef  = _b * farLeft;

				half3 clamped = clamp((original -
					right_with_coef -
					left_with_coef -
					far_right_with_coef -
					far_left_with_coef) / multiplier, 0.0, 1.0);

				return LinearToGamma(clamped);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				return fixed4(NaiveTap(i), 0.0);
			}
			ENDCG
		}
	}
}
