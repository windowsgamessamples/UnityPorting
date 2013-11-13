//  This code is for demonstration only and has not been heavily tested or optimized.
//
//	Based on the MyCustomFog shader from the Windows Store getting started example.
//
//  Demonstrates a custom implementation of linear fog.
//  To function correctly, fog mode should be set to linear in main menu)->edit->render settings
//
//  It appears about the same as diffuse material in-game
//  Unlike the inbuilt fog, it unfortunately also displays in the editor view.
//

Shader "Custom/MyCustomFogLinear" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Fog {Mode Off}
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert finalcolor:mycolor vertex:myvert

		sampler2D _MainTex;
		uniform half4 unity_FogColor;
		uniform half4 unity_FogStart;
		uniform half4 unity_FogEnd;

		struct Input {
			float2 uv_MainTex;
			half fog;
		};

		void myvert (inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input,data);
			float pos = length(mul (UNITY_MATRIX_MV, v.vertex).xyz);

			float diff = unity_FogEnd.x - unity_FogStart.x;
			float invDiff = 1.0f / diff;
			data.fog = clamp ((unity_FogEnd.x - pos) * invDiff, 0.0, 1.0);
		}
		void mycolor (Input IN, SurfaceOutput o, inout fixed4 color)
		{
			fixed3 fogColor = unity_FogColor.rgb;
			#ifdef UNITY_PASS_FORWARDADD
			fogColor = 0;
			#endif
			color.rgb = lerp (fogColor, color.rgb, IN.fog);
		}

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	//In general a fallback is desired, but for demonstration purposes we wish to reveal any shader issues.
	//FallBack "Diffuse"
}