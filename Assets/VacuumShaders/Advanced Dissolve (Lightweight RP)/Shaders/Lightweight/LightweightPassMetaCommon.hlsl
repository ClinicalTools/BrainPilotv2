#ifndef LIGHTWEIGHT_PASS_META_INCLUDED
#define LIGHTWEIGHT_PASS_META_INCLUDED

#include "LWRP/ShaderLibrary/Lighting.hlsl"

CBUFFER_START(UnityMetaPass)
// x = use uv1 as raster position
// y = use uv2 as raster position
bool4 unity_MetaVertexControl;

// x = return albedo
// y = return normal
bool4 unity_MetaFragmentControl; 
CBUFFER_END

float unity_OneOverOutputBoost;
float unity_MaxOutputValue;
float unity_UseLinearSpace;

struct MetaInput
{ 
    half3 Albedo;
    half3 Emission;
    half3 SpecularColor;
};

struct MetaVertexInput
{
    float4 vertex   : POSITION;
    half3 normal    : NORMAL;
    float2 uv0      : TEXCOORD0;
    float2 uv1      : TEXCOORD1;
    float2 uv2      : TEXCOORD2;
#ifdef _TANGENT_TO_WORLD
    half4 tangent   : TANGENT;
#endif
};

struct MetaVertexOuput
{
    float4 pos      : SV_POSITION;
    float2 uv       : TEXCOORD0;


	float3 posWS : TEXCOORD1;
	float3 normal : TEXCOORD2;

#ifdef _DISSOLVEMAPPINGTYPE_TRIPLANAR
	float3 objNormal : TEXCOORD3;
	float3 coords : TEXCOORD4;
#else
	float4 dissolveUV : TEXCOORD3;
#endif 
};

float4 MetaVertexPosition(float4 vertex, float2 uv1, float2 uv2, float4 lightmapST)
{
    if (unity_MetaVertexControl.x)
    {
        vertex.xy = uv1 * lightmapST.xy + lightmapST.zw;
        // OpenGL right now needs to actually use incoming vertex position,
        // so use it in a very dummy way
        vertex.z = vertex.z > 0 ? REAL_MIN : 0.0f;
    }
    return TransformWorldToHClip(vertex.xyz); // Need to transfer from world to clip compared to legacy
}

half4 MetaFragment(MetaInput IN)
{
    half4 res = 0;
    if (unity_MetaFragmentControl.x)
    {
        res = half4(IN.Albedo, 1);

        // d3d9 shader compiler doesn't like NaNs and infinity.
        unity_OneOverOutputBoost = saturate(unity_OneOverOutputBoost);

        // Apply Albedo Boost from LightmapSettings.
        res.rgb = clamp(PositivePow(res.rgb, unity_OneOverOutputBoost), 0, unity_MaxOutputValue);
    }
    if (unity_MetaFragmentControl.y)
    {
        res = half4(IN.Emission, 1.0);
    }
    return res;
}

MetaVertexOuput LightweightVertexMeta(MetaVertexInput v)
{
    MetaVertexOuput o;
    o.pos = MetaVertexPosition(v.vertex, v.uv1.xy, v.uv2.xy, unity_LightmapST);
    o.uv = TRANSFORM_TEX(v.uv0, _MainTex);



	o.posWS = TransformObjectToWorld(v.vertex.xyz);
	o.normal = TransformObjectToWorldNormal(v.normal);

#ifdef _DISSOLVEMAPPINGTYPE_TRIPLANAR
	o.coords = v.vertex.xyz;
	o.objNormal = lerp(o.normal.xyz, v.normal.xyz, VALUE_TRIPLANARMAPPINGSPACE);
#else

	float4 oPos = 0;
	#ifdef _DISSOLVEMAPPINGTYPE_SCREEN_SPACE
		oPos = TransformWorldToHClip(o.posWS);
	#endif

	DissolveVertex2Fragment(oPos, v.uv0, v.uv1.xy, o.dissolveUV);
#endif

#if defined(_DISSOLVEMASK_XYZ_AXIS) || defined(_DISSOLVEMASK_PLANE) || defined(_DISSOLVEMASK_SPHERE) || defined(_DISSOLVEMASK_BOX) || defined(_DISSOLVEMASK_CYLINDER) || defined(_DISSOLVEMASK_CONE)
	o.posWS += _Dissolve_ObjectWorldPos;
#endif




    return o;
}

#endif
