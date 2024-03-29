Shader "Hidden/Vol/Vhs" 
{
	Properties 
	{
		_Intensity ("Intensity", Range(0.0, 1.0)) = 1
		_Rocking ("Rocking", Range(0.0, 0.1)) = 0.01
		_VhsTex ("VhsTex", 2D) = "white" {}
	}

	SubShader 
	{
		Pass 
		{
			name "Vhs"
			
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
					
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			sampler2D _MainTex;
			sampler2D _VhsTex;
			
			float _yScanline;
			float _xScanline;
			float _Intensity;
			float _Rocking;
			float _Tape;
			float _Noise;
			fixed4 _Glitch;
			
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
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }
			
			float rand(float3 co)
			{
			     return frac(sin(dot(co.xyz ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
			}
 
			fixed4 frag (v2f i) : COLOR
			{
				const fixed4 main = tex2D(_MainTex, i.uv);
				fixed4 vhs = tex2D(_VhsTex, i.uv) * _Tape;

				float dx = 1 - abs(distance(i.uv.y, _xScanline));
				float dy = 1 - abs(distance(i.uv.y, _yScanline));
				
				i.uv.x += (dy - .5) * _Rocking + rand(float3(dy, dy, dy)) / _Noise;
								
				if (dx > 0.99)
					i.uv.y = _xScanline;
				
				i.uv = frac(i.uv);
				
				const fixed4 c = tex2D(_MainTex, i.uv);
				vhs.a = c.a;
				
				fixed3 bleed = tex2D(_MainTex, i.uv + float2(0.01, 0)).rgb;
				bleed += tex2D(_MainTex, i.uv + float2(0.02, 0)).rgb;
				bleed += tex2D(_MainTex, i.uv + float2(0.01, 0.01)).rgb;
				bleed += tex2D(_MainTex, i.uv + float2(0.02, 0.02)).rgb;
				bleed /= 6;
				
				if (dot(bleed, fixed3(1, 1, 1))  > 0.1)
				{
					vhs += fixed4(bleed * _xScanline * _Glitch.rgb * _Glitch.a, 0);
				}
								
				vhs += c - rand(float3(i.uv.x, i.uv.y, _xScanline)) * _xScanline / 5;
				return lerp(main, vhs, _Intensity);
			}
			ENDCG
		}
	}
Fallback off
}