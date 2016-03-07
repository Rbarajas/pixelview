Shader "Custom/Curvature" {
	Properties{
		_MainTex("Color", 2D) = "white" { }
		_Metallic("Metallic", Range(0, 1)) = 0
		_Smoothness("Smoothness", Range(0, 1)) = 0.5
	}

	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM

		#pragma surface surf Standard addshadow fullforwardshadows vertex:vert
		#pragma target 3.0

		uniform half _Distance;
		uniform half _MaxCurvature;
		uniform half _CurvatureH;
		uniform half _CurvatureV;
		uniform half _FogMin;
		uniform half _FogMax;

		sampler2D _MainTex;
		fixed _Metallic;
		fixed _Smoothness;

		struct Input {
			float2 uv_MainTex;
			half cameraDistance;
		};

		void vert(inout appdata_full v, out Input o)
		{
			float4 vv = mul(_Object2World, v.vertex);

			half d = vv.z - _WorldSpaceCameraPos.z;
			if (d > _Distance)
			{
				vv = d * (d - _Distance) * _MaxCurvature * float4(_CurvatureH, _CurvatureV, 0.0f, 0.0f);

				v.vertex += mul(_World2Object, vv);
			}

			UNITY_INITIALIZE_OUTPUT(Input, o);

			o.cameraDistance = abs(mul(UNITY_MATRIX_MV, v.vertex).z);
		}

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

			half fogRange = _FogMax - _FogMin;
			if (fogRange > 0)
			{
				fixed fogginess = clamp((IN.cameraDistance - _FogMin) / fogRange, 0, 1);
				c.xyz *= 1 - fogginess;
			}

			o.Albedo = c;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = c.a;
		}

		ENDCG
	}
}