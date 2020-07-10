// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

//#define UNITY_4

#if defined(UNITY_4)
#define UNITY_SAMPLE_TEX2D tex2D
#endif

#if defined(FERR2DT_LIGHTMAP) && defined(UNITY_4)
sampler2D_half unity_Lightmap;
//half4          unity_LightmapST;
#endif

#if defined(FERR2DT_TINT)
float4 _Color;
#endif

#if defined(FERR2DT_WAVY)
float _WaveSizeX;
float _WaveSizeY;
float _WaveSpeed;
float _PositionScale;
#endif

sampler2D _MainTex;
half4     _MainTex_ST;

struct appdata_ferr {
	float4 vertex     : POSITION;
	float2 texcoord   : TEXCOORD0;
	#ifdef FERR2DT_LIGHTMAP
	half2  lightcoord : TEXCOORD1;
	#endif
	fixed4 color      : COLOR;
};
struct VS_OUT {
	float4 position: SV_POSITION;
	half2  uv      : TEXCOORD0;
	#ifdef FERR2DT_LIGHTMAP
	half2  lightuv : TEXCOORD1;
	#endif
	#if MAX_LIGHTS > 0
	half3  viewpos : TEXCOORD2;
	#endif
	fixed4 color   : COLOR;
};

float3 GetLight(int i, float3 aViewPos) {
	half3  toLight = unity_LightPosition[i].xyz - aViewPos * unity_LightPosition[i].w;
	half   distSq  = dot(toLight, toLight);
	#ifdef UNITY_4
	half   atten   = 2.0 / ((distSq * unity_LightAtten[i].z) + 1.0);
	#else
	half   atten   = 1.0 / ((distSq * unity_LightAtten[i].z) + 1.0);
	#endif

	return ((unity_LightColor[i].rgb * pow(atten, 1.75)));
}

VS_OUT vert(appdata_ferr input) {
	VS_OUT result;

	#if defined(FERR2DT_WAVY)
	float4 world      = mul(unity_ObjectToWorld, input.vertex);
	float  waveOffset = (world.x + world.y + world.z) / _PositionScale;
	float  wave       = (_Time.z + waveOffset) * _WaveSpeed;
	result.position   = UnityObjectToClipPos(input.vertex + float4(cos(wave) * _WaveSizeX, sin(wave) * _WaveSizeY, 0, 0));
	#else
	result.position   = UnityObjectToClipPos(input.vertex);
	#endif
	
	#ifdef FERR2DT_TINT
	result.color   = input.color * _Color;
	#else
	result.color   = input.color;
	#endif
	
	#if MAX_LIGHTS > 0
	result.viewpos = mul(UNITY_MATRIX_MV, input.vertex).xyz;
	#endif
	#ifdef FERR2DT_LIGHTMAP
	result.lightuv = input.lightcoord * unity_LightmapST.xy + unity_LightmapST.zw;
	#endif
	result.uv      = TRANSFORM_TEX(input.texcoord, _MainTex);

	return result;
}

fixed4 frag(VS_OUT inp) : COLOR{
	fixed4 color = tex2D(_MainTex, inp.uv);
	#ifdef FERR2DT_LIGHTMAP
	fixed3 light = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, inp.lightuv));
	#elif  MAX_LIGHTS > 0
	fixed3 light = UNITY_LIGHTMODEL_AMBIENT;
	#endif

	color      = color * inp.color;
	#if MAX_LIGHTS > 0
	for (int i = 0; i < MAX_LIGHTS; i++) {
		light += GetLight(i, inp.viewpos);
	}
	#endif

	#if defined(FERR2DT_LIGHMAP) || MAX_LIGHTS > 0
	color.rgb *= light;
	#endif

	return color;
}