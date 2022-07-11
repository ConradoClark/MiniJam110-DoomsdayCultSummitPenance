Shader "Thinker/SpriteColorizer"
{
	Properties
	{
		[Header(Texture)]
		_MainTex ("Main Texture", 2D) = "white" {}

		[Header(Color)]
		_Color("Tint", Color) = (1,1,1,1)
		_Colorize("Colorize",Color) = (1,1,1,0)

		[Header(UV Scrolling)]
		_HScroll ("Scroll - Horizontal UV Scroll", float) = 0

		[Header(Color Replacement)]
		_ColorReplSource1("Replacement Source 1", Color) = (1,1,1,1)
		_ColorReplTarget1("Replacement Target 1", Color) = (1,1,1,1)
		_ColorReplTolerance1("Replacement Tolerance 1", Range(0,255)) = 0

		_ColorReplSource2("Replacement Source 2", Color) = (1,1,1,1)
		_ColorReplTarget2("Replacement Target 2", Color) = (1,1,1,1)
		_ColorReplTolerance2("Replacement Tolerance 2", Range(0,255)) = 0
	}
	SubShader
	{

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 
		Cull Off
		
		Pass
		{
			CGPROGRAM
			#pragma target 3.0
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
				float2 screenPos:TEXCOORD2;
				UNITY_FOG_COORDS(1)
				float4 vertex : POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			fixed4 _Color;
			fixed4 _Colorize;

			float _HScroll;

			fixed4 _ColorReplSource1;
			fixed4 _ColorReplTarget1;
			float _ColorReplTolerance1;

			fixed4 _ColorReplSource2;
			fixed4 _ColorReplTarget2;
			float _ColorReplTolerance2;

	
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex = UnityPixelSnap(o.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);

				o.uv = (TRANSFORM_TEX(v.uv, _MainTex) - float2(_HScroll, 0) % float2(1,1));
			

				return o;
			}

			float getHue(fixed4 color) {
				float Epsilon = 1e-10;
				float4 P = (color.g < color.b) ? float4(color.bg, -1.0, 2.0 / 3.0) : float4(color.gb, 0.0, -1.0 / 3.0);
				float4 Q = (color.r < P.x) ? float4(P.xyw, color.r) : float4(color.r, P.yzx);
				float C = Q.x - min(Q.w, Q.y);
				float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
				return H;
			}

			fixed4 shiftHue(fixed4 originalColor, float hue) {
				fixed4 col = originalColor;
				float pi = 3.14159265358979323846;
				float U = cos(hue*pi / 180);
				float W = sin(hue*pi / 180);

				fixed4 colCopy = col;
				col.r = (.299 + .701*U + .168*W)*colCopy.r
					+ (.587 - .587*U + .330*W)*colCopy.g
					+ (.114 - .114*U - .497*W)*colCopy.b;
				col.g = (.299 - .299*U - .328*W)*colCopy.r
					+ (.587 + .413*U + .035*W)*colCopy.g
					+ (.114 - .114*U + .292*W)*colCopy.b;
				col.b = (.299 - .3*U + 1.25*W)*colCopy.r
					+ (.587 - .588*U - 1.05*W)*colCopy.g
					+ (.114 + .886*U - .203*W)*colCopy.b;
				return col;
			}

			fixed4 replaceColor(fixed4 originalColor, fixed4 source, fixed4 target, float tolerance) {
				fixed4 col = originalColor;
				float colorDist = distance(getHue(col), getHue(source));
				
				if (colorDist < tolerance / 255) {
					col = shiftHue(col, getHue(col) * 360 - getHue(target) * 360);
				}
				return col;
			}

			fixed4 frag(v2f i) : SV_Target
			{			
				fixed4 col = tex2D(_MainTex, i.uv);

				// Color Replacement
				col = _ColorReplTarget1.a > 0 ? replaceColor(col, _ColorReplSource1, _ColorReplTarget1, _ColorReplTolerance1) : col;
				col = _ColorReplTarget2.a > 0 ? replaceColor(col, _ColorReplSource2, _ColorReplTarget2, _ColorReplTolerance2) : col;

				col = col * _Color;

				// Colorize
				col = fixed4(_Colorize.r* _Colorize.a + col.r*(1-_Colorize.a),
							 _Colorize.g * _Colorize.a + col.g*(1 - _Colorize.a),
							 _Colorize.b * _Colorize.a + col.b*(1 - _Colorize.a), col.a);

				return col;
			}

			ENDCG
		}
	}
}