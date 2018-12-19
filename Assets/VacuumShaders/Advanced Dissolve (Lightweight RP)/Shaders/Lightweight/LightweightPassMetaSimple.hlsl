#ifndef LIGHTWEIGHT_PASS_META_SIMPLE_INCLUDED
#define LIGHTWEIGHT_PASS_META_SIMPLE_INCLUDED

#include "LightweightPassMetaCommon.hlsl"

half4 LightweightFragmentMetaSimple(MetaVertexOuput i) : SV_Target
{

#ifdef _DISSOLVEMAPPINGTYPE_TRIPLANAR
	float4 alpha = ReadDissolveAlpha_Triplanar(i.coords, i.objNormal, i.posWS);
#else
	float4 alpha = ReadDissolveAlpha(i.uv.xy, i.dissolveUV, i.posWS);
#endif

	float3 dissolveAlbedo = 0;
	float3 dissolveEmission = 0;
	float dissolveBlend = DoDissolveAlbedoEmission(alpha, dissolveAlbedo, dissolveEmission, i.uv.xy);



    float2 uv = i.uv;
    MetaInput o;
    o.Albedo = _Color.rgb * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).rgb;
    o.SpecularColor = SampleSpecularGloss(uv, 1.0h, _SpecColor, TEXTURE2D_PARAM(_SpecGlossMap, sampler_SpecGlossMap)).xyz;
    o.Emission = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_PARAM(_EmissionMap, sampler_EmissionMap));


//#ifdef _ALPHABLEND_ON
//	dissolveBlend *= o.Alpha;
//#endif

	o.Albedo = lerp(o.Albedo, dissolveAlbedo, dissolveBlend);
	o.Emission = lerp(o.Emission, dissolveEmission, dissolveBlend);



    return MetaFragment(o);
}

#endif // LIGHTWEIGHT_PASS_META_SIMPLE_INCLUDED
