Shader "Ferr/Lit Textured Vertex Color Tinted Transparent (4 lights|lightmap +1 light)" {
	Properties {
		_MainTex ("Texture (RGB) Alpha (A)", 2D) = "white" {}
		_Color   ("Color tint (RGBA)", Color) = (1,1,1,1)
	}
	 
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend  SrcAlpha OneMinusSrcAlpha
		
		LOD 100
		ZWrite    Off
		Cull      Off
		Fog {Mode Off}
	 
		Pass {
			Tags { LightMode = Vertex } 
			CGPROGRAM
			#pragma  vertex   vert  
			#pragma  fragment frag
			#pragma  fragmentoption ARB_precision_hint_fastest
			
			#define  MAX_LIGHTS 4
			#define  FERR2DT_TINT
			
			#include "UnityCG.cginc"
			#include "LitCommon.cginc"
			
			ENDCG
		}
		Pass {
			Tags { LightMode = VertexLMRGBM } 
			CGPROGRAM
			#pragma  vertex   vert
			#pragma  fragment frag
			#pragma  fragmentoption ARB_precision_hint_fastest
			
			#define  MAX_LIGHTS 1
			#define  FERR2DT_LIGHTMAP
			#define  FERR2DT_TINT
			
			#include "UnityCG.cginc"
			#include "LitCommon.cginc"
			
			ENDCG
		}
	}
	Fallback "VertexLit"
}