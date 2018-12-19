#ifndef LIGHTWEIGHT_PASS_META_PBR_INCLUDED
#define LIGHTWEIGHT_PASS_META_PBR_INCLUDED

#include "LightweightPassMetaCommon.hlsl"

half4 LightweightFragmentMeta(MetaVertexOuput i) : SV_Target
{

#ifdef _DISSOLVEMAPPINGTYPE_TRIPLANAR
	float4 alpha = ReadDissolveAlpha_Triplanar(i.coords, i.objNormal, i.posWS);
#else
	float4 alpha = ReadDissolveAlpha(i.uv.xy, i.dissolveUV, i.posWS);
#endif

	float3 dissolveAlbedo = 0;
	float3 dissolveEmission = 0;
	float dissolveBlend = DoDissolveAlbedoEmission(alpha, dissolveAlbedo, dissolveEmission, i.uv.xy);



    SurfaceData surfaceData; 
    InitializeStandardLitSurfaceData(i.uv, surfaceData);

    BRDFData brdfData;
    InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.alpha, brdfData);

    MetaInput o;
    o.Albedo = brdfData.diffuse + brdfData.specular * brdfData.roughness * 0.5;
    o.SpecularColor = surfaceData.specular;
    o.Emission = surfaceData.emission;


//#ifdef _ALPHABLEND_ON
//	dissolveBlend *= o.Alpha;
//#endif

	o.Albedo = lerp(o.Albedo, dissolveAlbedo, dissolveBlend);
	o.Emission = lerp(o.Emission, dissolveEmission, dissolveBlend);

    return MetaFragment(o);
}

#endif // LIGHTWEIGHT_PASS_META_PBR_INCLUDED
