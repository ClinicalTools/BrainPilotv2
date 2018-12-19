#ifndef LIGHTWEIGHT_PASS_DEPTH_ONLY_INCLUDED
#define LIGHTWEIGHT_PASS_DEPTH_ONLY_INCLUDED

#include "LWRP/ShaderLibrary/Core.hlsl"

struct VertexInput
{
    float4 position     : POSITION;
	float3 normal       : NORMAL;
    float2 texcoord     : TEXCOORD0;
	float2 texcoord1    : TEXCOORD1;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexOutput
{
    float2 uv           : TEXCOORD0;
    float4 clipPos      : SV_POSITION;

	float3 posWS : TEXCOORD1;
	float3 normal : TEXCOORD2;

#ifdef _DISSOLVEMAPPINGTYPE_TRIPLANAR
	float3 objNormal : TEXCOORD3;
	float3 coords : TEXCOORD4;
#else
	float4 dissolveUV : TEXCOORD3;
#endif

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

VertexOutput DepthOnlyVertex(VertexInput v)
{
    VertexOutput o = (VertexOutput)0;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
    o.clipPos = TransformObjectToHClip(v.position.xyz);


	o.posWS = TransformObjectToWorld(v.position.xyz);
	o.normal = TransformObjectToWorldNormal(v.normal);

#ifdef _DISSOLVEMAPPINGTYPE_TRIPLANAR
	o.coords = v.position.xyz;
	o.objNormal = lerp(o.normal.xyz, v.normal.xyz, VALUE_TRIPLANARMAPPINGSPACE);
#else

	float4 oPos = 0;
	#ifdef _DISSOLVEMAPPINGTYPE_SCREEN_SPACE
		oPos = TransformWorldToHClip(o.posWS);
	#endif

	DissolveVertex2Fragment(oPos, v.texcoord, v.texcoord1.xy, o.dissolveUV);
#endif

    return o;
}

half4 DepthOnlyFragment(VertexOutput IN) : SV_TARGET
{
#ifdef _DISSOLVEMAPPINGTYPE_TRIPLANAR
	float4 alpha = ReadDissolveAlpha_Triplanar(IN.coords, IN.objNormal, IN.posWS);
#else
	float4 alpha = ReadDissolveAlpha(IN.uv.xy, IN.dissolveUV, IN.posWS);
#endif
	DoDissolveClip(alpha);


    Alpha(SampleAlbedoAlpha(IN.uv, TEXTURE2D_PARAM(_MainTex, sampler_MainTex)).a, _Color, _Cutoff);
    return 0;
}
#endif
