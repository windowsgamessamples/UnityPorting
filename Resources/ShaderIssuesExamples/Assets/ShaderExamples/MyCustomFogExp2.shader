//  This code is for demonstration only and has not been heavily tested or optimized.
//
//	Based on the MyCustomFog shader from the Windows Store getting started example.
//
//  Demonstrates a custom implementation of Exp2 fog.
//  To function correctly, fog mode should be set to Exp2 in (main menu)->edit->render settings
//
//  It appears about the same as diffuse material in-game
//  Unlike the inbuilt fog, it unfortunately also displays in the editor view.
//

Shader "Custom/MyCustomFogExp2" {
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
		uniform half4 unity_FogDensity;

		struct Input {
			float2 uv_MainTex;
			half fog;
		};

		void myvert (inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input,data);
			float fogDensity = unity_FogDensity;
			float fogDistance = length(mul (UNITY_MATRIX_MV, v.vertex).xyz);
			data.fog =  1 / pow( 2.71828 , pow( (fogDistance * fogDensity) , 2 ) );
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
