Shader "FancyUnity/LitLive2D"
{
	Properties
	{
		// Texture and model opacity settings.
		[PerRendererData] _MainTex("Main Texture", 2D) = "white" {}
		[PerRendererData] cubism_ModelOpacity("Model Opacity", Float) = 1

		// Blend settings.
		_SrcColor("Source Color", Int) = 0
		_DstColor("Destination Color", Int) = 0
		_SrcAlpha("Source Alpha", Int) = 0
		_DstAlpha("Destination Alpha", Int) = 0

		// Mask settings.
		[Toggle(CUBISM_MASK_ON)] cubism_MaskOn("Mask?", Int) = 0
		[Toggle(CUBISM_INVERT_ON)] cubism_InvertOn("Inverted?", Int) = 0
		[PerRendererData] cubism_MaskTexture("cubism_Internal", 2D) = "white" {}
		[PerRendererData] cubism_MaskTile("cubism_Internal", Vector) = (0, 0, 0, 0)
		[PerRendererData] cubism_MaskTransform("cubism_Internal", Vector) = (0, 0, 0, 0)
	}

	
	HLSLINCLUDE
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	ENDHLSL
	
	
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
			"RenderPipeline" = "UniversalPipeline"
		}

		Cull     Off
		Lighting Off
		ZWrite   Off
		//Blend[_SrcColor][_DstColor],[_SrcAlpha][_DstAlpha]
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
			Tags { "LightMode" = "Universal2D" }
			HLSLPROGRAM
			#pragma prefer_hlslcc gles
			#pragma vertex CombinedShapeLightVertex
			#pragma fragment CombinedShapeLightFragment
			#pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
			#pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
			#pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
			#pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __

			struct Attributes
			{
				float3 positionOS   : POSITION;
				float4 color        : COLOR;
				float2  uv           : TEXCOORD0;
			};

			struct Varyings
			{
				float4  positionCS  : SV_POSITION;
				float4  color       : COLOR;
				float2	uv          : TEXCOORD0;
				float2	lightingUV  : TEXCOORD1;
			};

			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			TEXTURE2D(_MaskTex);
			SAMPLER(sampler_MaskTex);
			TEXTURE2D(_NormalMap);
			SAMPLER(sampler_NormalMap);
			half4 _MainTex_ST;
			half4 _NormalMap_ST;

			#if USE_SHAPE_LIGHT_TYPE_0
			SHAPE_LIGHT(0)
			#endif

			#if USE_SHAPE_LIGHT_TYPE_1
			SHAPE_LIGHT(1)
			#endif

			#if USE_SHAPE_LIGHT_TYPE_2
			SHAPE_LIGHT(2)
			#endif

			#if USE_SHAPE_LIGHT_TYPE_3
			SHAPE_LIGHT(3)
			#endif

			Varyings CombinedShapeLightVertex(Attributes v)
			{
				Varyings o = (Varyings)0;

				o.positionCS = TransformObjectToHClip(v.positionOS);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float4 clipVertex = o.positionCS / o.positionCS.w;
				o.lightingUV = ComputeScreenPos(clipVertex).xy;
				o.color = v.color;
				return o;
			}

			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

			half4 CombinedShapeLightFragment(Varyings i) : SV_Target
			{
				half4 main = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
				half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);

				return CombinedShapeLightShared(main, mask, i.lightingUV);
			}
			ENDHLSL
		}
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile CUBISM_MASK_ON CUBISM_MASK_OFF CUBISM_INVERT_ON


			#include "UnityCG.cginc"
			#include "/Assets/Live2D/Cubism/Rendering/Resources/Live2D/Cubism/Shaders/CubismCG.cginc"


			struct appdata
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};


			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO

				// Add Cubism specific vertex output data.
				CUBISM_VERTEX_OUTPUT
			};


			sampler2D _MainTex;


			// Include Cubism specific shader variables.
			CUBISM_SHADER_VARIABLES


			v2f vert(appdata IN)
			{
				v2f OUT;


				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);


				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;


				// Initialize Cubism specific vertex output data.
				CUBISM_INITIALIZE_VERTEX_OUTPUT(IN, OUT); 


				return OUT;
			}


			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 OUT = tex2D(_MainTex, IN.texcoord) * IN.color;


			// Apply Cubism alpha to color.
			CUBISM_APPLY_ALPHA(IN, OUT);

			OUT.rgb *= 0.5f;
			return OUT;
			}
			ENDCG
		}
	}
}
