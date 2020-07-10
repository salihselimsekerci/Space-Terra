Shader "Ferr/Lit Textured Vertex Color Tinted (4 lights|lightmap +1 light)" {
	Properties {
		_MainTex ("Texture (RGB)", 2D) = "white" {}
		_Color   ("Color tint (RGB)", Color) = (1,1,1,1)
	}
	 
	SubShader {
		Tags {"IgnoreProjector"="True" "RenderType"="Opaque"}
		Blend Off
		
		LOD 100
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