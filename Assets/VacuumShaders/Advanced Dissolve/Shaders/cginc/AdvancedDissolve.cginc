
//SHADER_API_D3D11_9X limitations
#if defined(SHADER_API_D3D11_9X) && (SHADER_TARGET < 25)
	
	#ifdef _DISSOLVEMAPPINGTYPE_TRIPLANAR
	#undef _DISSOLVEMAPPINGTYPE_TRIPLANAR
	#endif

	#ifdef _DISSOLVEMAPPINGTYPE_SCREEN_SPACE
	#undef _DISSOLVEMAPPINGTYPE_SCREEN_SPACE
	#endif

	#ifdef _DISSOLVEALPHASOURCE_THREE_CUSTOM_MAPS
	#undef _DISSOLVEALPHASOURCE_THREE_CUSTOM_MAPS
	#define _DISSOLVEALPHASOURCE_TWO_CUSTOM_MAPS
	#endif

	#if defined(_DISSOLVEMASK_BOX)
		
		#ifdef _DISSOLVEMASKCOUNT_FOUR
		#undef _DISSOLVEMASKCOUNT_FOUR
		#define _DISSOLVEMASKCOUNT_THREE
		#endif	

	#endif

#endif


#define TIME _Time.x


#include "Variables.cginc"



inline void DissolveVertex2Fragment(float4 oPos, float2 vertexUV0, float2 vertexUV1, inout float4 dissolveMapUV)
{
	dissolveMapUV = 0;

	#if defined(_DISSOLVEMAPPINGTYPE_SCREEN_SPACE)
		dissolveMapUV = ComputeScreenPos(oPos);
	#else

		#if defined(_DISSOLVEALPHASOURCE_CUSTOM_MAP) || defined(_DISSOLVEALPHASOURCE_TWO_CUSTOM_MAPS) || defined(_DISSOLVEALPHASOURCE_THREE_CUSTOM_MAPS)
			float2 texUV = VALUE_UVSET == 0 ? vertexUV0 : vertexUV1;

			#if defined(_DISSOLVEALPHASOURCE_CUSTOM_MAP)

				dissolveMapUV.xy = texUV.xy * VALUE_MAP1_ST.xy + VALUE_MAP1_ST.zw + VALUE_MAP1_SCROLL.xy * TIME;

			#elif defined(_DISSOLVEALPHASOURCE_TWO_CUSTOM_MAPS)

				dissolveMapUV.xy = texUV.xy * VALUE_MAP1_ST.xy + VALUE_MAP1_ST.zw + VALUE_MAP1_SCROLL.xy * TIME;
				dissolveMapUV.zw = texUV.xy * VALUE_MAP2_ST.xy + VALUE_MAP2_ST.zw + VALUE_MAP2_SCROLL.xy * TIME;

			#else
				
				dissolveMapUV.xy = texUV;
				dissolveMapUV.zw = texUV;

			#endif
		#endif
	#endif
}

#if defined(_DISSOLVEMASK_BOX)
inline bool IsPointInsideRect(float3 vertex, float3 min, float3 max)
{
	return ((vertex.x > min.x && vertex.x < max.x) && (vertex.y > min.y && vertex.y < max.y) && (vertex.z > min.z && vertex.z < max.z));
}

inline float EdgeIntersection(float p1, float e1, float p2, float e2)
{
	return saturate(e2 - p1) + saturate(e1 - p2) + (e1 * e2);
}
#endif

inline float ReadMaskValue(float3 vertexPos, float noise)
{
	float cutout = -1;


	#if defined(_DISSOLVEMASK_XYZ_AXIS)

		if(VALUE_MASK_SPACE > 0.5)
			vertexPos = mul(unity_WorldToObject, float4(vertexPos, 1));
	

		cutout = (vertexPos - VALUE_MASK_OFFSET)[(int)VALUE_CUTOFF_AXIS] * VALUE_AXIS_INVERT;

		cutout += noise;

	#elif defined(_DISSOLVEMASK_PLANE)

		#if defined(_DISSOLVEMASKCOUNT_FOUR)
			
			float d1 = dot(VALUE_MASK_NORMAL, vertexPos - VALUE_MASK_POSITION);
			float d2 = dot(VALUE_MASK_2_NORMAL, vertexPos - VALUE_MASK_2_POSITION);
			float d3 = dot(VALUE_MASK_3_NORMAL, vertexPos - VALUE_MASK_3_POSITION);
			float d4 = dot(VALUE_MASK_4_NORMAL, vertexPos - VALUE_MASK_4_POSITION);

			if (VALUE_AXIS_INVERT > 0)
			{
				cutout = min(min(d1, d2), min(d3, d4));
			}
			else
			{
				cutout = max(max(-d1, -d2), max(-d3, -d4));
			}

			cutout += noise;

		#elif defined(_DISSOLVEMASKCOUNT_THREE)
			
			float d1 = dot(VALUE_MASK_NORMAL, vertexPos - VALUE_MASK_POSITION);
			float d2 = dot(VALUE_MASK_2_NORMAL, vertexPos - VALUE_MASK_2_POSITION);
			float d3 = dot(VALUE_MASK_3_NORMAL, vertexPos - VALUE_MASK_3_POSITION);

			if (VALUE_AXIS_INVERT > 0)
			{
				cutout = min(min(d1, d2), d3);
			}
			else
			{
				cutout = max(max(-d1, -d2), -d3);
			}

			cutout += noise;

		#elif defined(_DISSOLVEMASKCOUNT_TWO)

			float d1 = dot(VALUE_MASK_NORMAL, vertexPos - VALUE_MASK_POSITION);
			float d2 = dot(VALUE_MASK_2_NORMAL, vertexPos - VALUE_MASK_2_POSITION);
		
			if (VALUE_AXIS_INVERT > 0)
			{
				cutout = min(d1, d2);
			}
			else
			{
				cutout = max(-d1, -d2);
			}

			cutout += noise;

		#else
			cutout = dot(VALUE_MASK_NORMAL * VALUE_AXIS_INVERT, vertexPos - VALUE_MASK_POSITION);
		
			cutout += noise;
		#endif
		

	#elif defined(_DISSOLVEMASK_SPHERE)

		#if defined(_DISSOLVEMASKCOUNT_FOUR)

			float4 radius = float4(VALUE_MASK_RADIUS, VALUE_MASK_2_RADIUS, VALUE_MASK_3_RADIUS, VALUE_MASK_4_RADIUS);

			float4 n = noise * (radius < 1 ? radius : 1);

			float4 d = float4(distance(vertexPos, VALUE_MASK_POSITION), distance(vertexPos, VALUE_MASK_2_POSITION), distance(vertexPos, VALUE_MASK_3_POSITION), distance(vertexPos, VALUE_MASK_4_POSITION));

			//radius += abs(noise) * (1 - VALUE_AXIS_INVERT * 2);
			radius -= noise;


			if (VALUE_AXIS_INVERT > 0)
			{
				float4 b = radius - min(d, radius);
				cutout = dot(b, 1);
			}
			else
			{
				float4 a = saturate(max(0, d - radius));

				a.xy = a.xz*a.yw;
				cutout = a.x * a.y;
			}

		#elif defined(_DISSOLVEMASKCOUNT_THREE)

			float3 radius = float3(VALUE_MASK_RADIUS, VALUE_MASK_2_RADIUS, VALUE_MASK_3_RADIUS);

			float3 n = noise * (radius < 1 ? radius : 1);

			float3 d = float3(distance(vertexPos, VALUE_MASK_POSITION), distance(vertexPos, VALUE_MASK_2_POSITION), distance(vertexPos, VALUE_MASK_3_POSITION));

			//radius += abs(noise) * (1 - VALUE_AXIS_INVERT * 2);
			radius -= noise;
						

			if (VALUE_AXIS_INVERT > 0)
			{
				float3 b = radius - min(d, radius);
				cutout = dot(b, 1);
			}
			else
			{
				float3 a = saturate(max(0, d - radius));
				cutout = a.x * a.y * a.z;
			}

		#elif defined(_DISSOLVEMASKCOUNT_TWO)


			float2 radius = float2(VALUE_MASK_RADIUS, VALUE_MASK_2_RADIUS);

			float2 n = noise * (radius < 1 ? radius : 1);

			float2 d = float2(distance(vertexPos, VALUE_MASK_POSITION), distance(vertexPos, VALUE_MASK_2_POSITION));

			//radius += abs(noise) * (1 - VALUE_AXIS_INVERT * 2);
			radius -= noise;

			
			if (VALUE_AXIS_INVERT > 0)
			{
				float2 b = radius - min(d, radius);
				cutout = dot(b, 1);
			}
			else
			{
				float2 a = saturate(max(0, d - radius));
				cutout = a.x * a.y;
			}				
			
		#else

			float radius = VALUE_MASK_RADIUS;

			noise *= (radius < 1 ? radius : 1);

			float d = distance(vertexPos, VALUE_MASK_POSITION);

			//radius += abs(noise) * (1 - VALUE_AXIS_INVERT * 2);
			radius -= noise;

			if (VALUE_AXIS_INVERT > 0)
			{
				cutout = radius - min(d, radius);
			}
			else
			{
				cutout = max(0, d - radius);
			} 

		#endif		

	#elif defined(_DISSOLVEMASK_BOX)
		
		float dissolveEdgeSize = VALUE_EDGE_SIZE;
		#ifdef UNITY_PASS_META
			dissolveEdgeSize *= VALUE_GI_MULTIPLIER;
		#endif

			
		#if defined(_DISSOLVEMASKCOUNT_FOUR)
						
			//1////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			float m_1 = 0;
			float e_1 = 0;

			float3 vertexInverseTransform = mul(VALUE_MASK_TRS, float4(vertexPos, 1)).xyz;

			float3 min = VALUE_MASK_BOUNDS_MIN + noise;
			float3 max = VALUE_MASK_BOUNDS_MAX - noise;			

			if (IsPointInsideRect(vertexInverseTransform, min, max))
			{		
				m_1 = 1;
				cutout = 1;
				//Edge Detect
				if (IsPointInsideRect(vertexInverseTransform, min + dissolveEdgeSize, max - dissolveEdgeSize) == false)
				{
					e_1 = 1;
				}
			}


			//2////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			float m_2 = 0;
			float e_2 = 0;

			float3 vertexInverseTransform_2 = mul(VALUE_MASK_2_TRS, float4(vertexPos, 1)).xyz;

			float3 min_2 = VALUE_MASK_2_BOUNDS_MIN + noise;
			float3 max_2 = VALUE_MASK_2_BOUNDS_MAX - noise;

			if (IsPointInsideRect(vertexInverseTransform_2, min_2, max_2))
			{		
				m_2 = 1;
				cutout = 1;
				//Edge Detect
				if (IsPointInsideRect(vertexInverseTransform_2, min_2 + dissolveEdgeSize, max_2 - dissolveEdgeSize) == false)
				{
					e_2 = 1;
				}				
			}


			//3////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			float m_3 = 0;
			float e_3 = 0;

			float3 vertexInverseTransform_3 = mul(VALUE_MASK_3_TRS, float4(vertexPos, 1)).xyz;

			float3 min_3 = VALUE_MASK_3_BOUNDS_MIN + noise;
			float3 max_3 = VALUE_MASK_3_BOUNDS_MAX - noise;

			if (IsPointInsideRect(vertexInverseTransform_3, min_3, max_3))
			{
				m_3 = 1;
				cutout = 1;
				//Edge Detect
				if (IsPointInsideRect(vertexInverseTransform_3, min_3 + dissolveEdgeSize, max_3 - dissolveEdgeSize) == false)
				{
					e_3 = 1;
				}
			}


			//4////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			float m_4 = 0;
			float e_4 = 0;

			float3 vertexInverseTransform_4 = mul(VALUE_MASK_4_TRS, float4(vertexPos, 1)).xyz;

			float3 min_4 = VALUE_MASK_4_BOUNDS_MIN + noise;
			float3 max_4 = VALUE_MASK_4_BOUNDS_MAX - noise;

			if (IsPointInsideRect(vertexInverseTransform_4, min_4, max_4))
			{
				m_4 = 1;
				cutout = 1;
				//Edge Detect
				if (IsPointInsideRect(vertexInverseTransform_4, min_4 + dissolveEdgeSize, max_4 - dissolveEdgeSize) == false)
				{
					e_4 = 1;
				}
			}

			
			float E1 = EdgeIntersection(m_1, e_1, m_2, e_2);
			float E2 = EdgeIntersection(saturate(m_1 + m_2), E1, m_3, e_3);
			float E3 = EdgeIntersection(saturate(m_1 + m_2 + m_3), E2, m_4, e_4);

			cutout = E3 > 0.5 ? dissolveEdgeSize * 0.5 * VALUE_AXIS_INVERT : cutout;


			cutout *= VALUE_AXIS_INVERT;

		#elif defined(_DISSOLVEMASKCOUNT_THREE)
						
			//1////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			float m_1 = 0;
			float e_1 = 0;

			float3 vertexInverseTransform = mul(VALUE_MASK_TRS, float4(vertexPos, 1)).xyz;

			float3 min = VALUE_MASK_BOUNDS_MIN + noise;
			float3 max = VALUE_MASK_BOUNDS_MAX - noise;			

			if (IsPointInsideRect(vertexInverseTransform, min, max))
			{		
				m_1 = 1;
				cutout = 1;
				//Edge Detect
				if (IsPointInsideRect(vertexInverseTransform, min + dissolveEdgeSize, max - dissolveEdgeSize) == false)
				{
					e_1 = 1;
				}
			}


			//2////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			float m_2 = 0;
			float e_2 = 0;

			float3 vertexInverseTransform_2 = mul(VALUE_MASK_2_TRS, float4(vertexPos, 1)).xyz;

			float3 min_2 = VALUE_MASK_2_BOUNDS_MIN + noise;
			float3 max_2 = VALUE_MASK_2_BOUNDS_MAX - noise;

			if (IsPointInsideRect(vertexInverseTransform_2, min_2, max_2))
			{		
				m_2 = 1;
				cutout = 1;
				//Edge Detect
				if (IsPointInsideRect(vertexInverseTransform_2, min_2 + dissolveEdgeSize, max_2 - dissolveEdgeSize) == false)
				{
					e_2 = 1;
				}				
			}


			//3////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			float m_3 = 0;
			float e_3 = 0;

			float3 vertexInverseTransform_3 = mul(VALUE_MASK_3_TRS, float4(vertexPos, 1)).xyz;

			float3 min_3 = VALUE_MASK_3_BOUNDS_MIN + noise;
			float3 max_3 = VALUE_MASK_3_BOUNDS_MAX - noise;

			if (IsPointInsideRect(vertexInverseTransform_3, min_3, max_3))
			{
				m_3 = 1;
				cutout = 1;
				//Edge Detect
				if (IsPointInsideRect(vertexInverseTransform_3, min_3 + dissolveEdgeSize, max_3 - dissolveEdgeSize) == false)
				{
					e_3 = 1;
				}
			}

			
			float E1 = EdgeIntersection(m_1, e_1, m_2, e_2);
			float E2 = EdgeIntersection(saturate(m_1 + m_2), E1, m_3, e_3);

			cutout = E2 > 0.5 ? dissolveEdgeSize * 0.5 * VALUE_AXIS_INVERT : cutout;


			cutout *= VALUE_AXIS_INVERT;

		#elif defined(_DISSOLVEMASKCOUNT_TWO)
			
			//1////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			float m_1 = 0;
			float e_1 = 0;

			float3 vertexInverseTransform = mul(VALUE_MASK_TRS, float4(vertexPos, 1)).xyz;

			float3 min = VALUE_MASK_BOUNDS_MIN + noise;
			float3 max = VALUE_MASK_BOUNDS_MAX - noise;			

			if (IsPointInsideRect(vertexInverseTransform, min, max))
			{		
				m_1 = 1;
				cutout = 1;
				//Edge Detect
				if (IsPointInsideRect(vertexInverseTransform, min + dissolveEdgeSize, max - dissolveEdgeSize) == false)
				{
					e_1 = 1;
				}
			}


			//2////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			float m_2 = 0;
			float e_2 = 0;

			float3 vertexInverseTransform_2 = mul(VALUE_MASK_2_TRS, float4(vertexPos, 1)).xyz;

			float3 min_2 = VALUE_MASK_2_BOUNDS_MIN + noise;
			float3 max_2 = VALUE_MASK_2_BOUNDS_MAX - noise;

			if (IsPointInsideRect(vertexInverseTransform_2, min_2, max_2))
			{		
				m_2 = 1;
				cutout = 1;
				//Edge Detect
				if (IsPointInsideRect(vertexInverseTransform_2, min_2 + dissolveEdgeSize, max_2 - dissolveEdgeSize) == false)
				{
					e_2 = 1;
				}				
			}



			float E = EdgeIntersection(m_1, e_1, m_2, e_2);
			cutout = E > 0.5 ? dissolveEdgeSize * 0.5 * VALUE_AXIS_INVERT : cutout;

		
			cutout *= VALUE_AXIS_INVERT;

		#else

			float3 vertexInverseTransform = mul(VALUE_MASK_TRS, float4(vertexPos, 1)).xyz;

			float3 min = VALUE_MASK_BOUNDS_MIN + noise;
			float3 max = VALUE_MASK_BOUNDS_MAX - noise;

		
			if (IsPointInsideRect(vertexInverseTransform, min, max))
			{
				cutout = 1;	

				//Edge Detect
				if( !IsPointInsideRect(vertexInverseTransform, min + dissolveEdgeSize, max - dissolveEdgeSize) )
				{
					cutout = dissolveEdgeSize * 0.5 * VALUE_AXIS_INVERT;
				}
			}
		
			cutout *= VALUE_AXIS_INVERT;

		#endif

	#elif defined(_DISSOLVEMASK_CYLINDER)
		
		#if defined(_DISSOLVEMASKCOUNT_FOUR)
			
			//1
			float3 p1_1 = VALUE_MASK_POSITION;
			float3 p1_2 = p1_1 + VALUE_MASK_NORMAL * VALUE_MASK_HEIGHT;
			
			float t1 = max(0, min(1, dot(vertexPos - p1_1, p1_2 - p1_1) / (VALUE_MASK_HEIGHT * VALUE_MASK_HEIGHT)));
			float3 projection1 = p1_1 + t1 * (p1_2 - p1_1);

			//2
			float3 p2_1 = VALUE_MASK_2_POSITION;
			float3 p2_2 = p2_1 + VALUE_MASK_2_NORMAL * VALUE_MASK_2_HEIGHT;

			float t2 = max(0, min(1, dot(vertexPos - p2_1, p2_2 - p2_1) / (VALUE_MASK_2_HEIGHT * VALUE_MASK_2_HEIGHT)));
			float3 projection2 = p2_1 + t2 * (p2_2 - p2_1);

			//3
			float3 p3_1 = VALUE_MASK_3_POSITION;
			float3 p3_2 = p3_1 + VALUE_MASK_3_NORMAL * VALUE_MASK_3_HEIGHT;

			float t3 = max(0, min(1, dot(vertexPos - p3_1, p3_2 - p3_1) / (VALUE_MASK_3_HEIGHT * VALUE_MASK_3_HEIGHT)));
			float3 projection3 = p3_1 + t3 * (p3_2 - p3_1);
			
			//4
			float3 p4_1 = VALUE_MASK_4_POSITION;
			float3 p4_2 = p4_1 + VALUE_MASK_4_NORMAL * VALUE_MASK_4_HEIGHT;

			float t4 = max(0, min(1, dot(vertexPos - p4_1, p4_2 - p4_1) / (VALUE_MASK_4_HEIGHT * VALUE_MASK_4_HEIGHT)));
			float3 projection4 = p4_1 + t4 * (p4_2 - p4_1);



			float4 d = float4(distance(vertexPos, projection1), distance(vertexPos, projection2), distance(vertexPos, projection3), distance(vertexPos, projection4));
			
			float4 radius = float4(VALUE_MASK_RADIUS, VALUE_MASK_2_RADIUS, VALUE_MASK_3_RADIUS, VALUE_MASK_4_RADIUS);
			
			float4 n = noise * (radius < 1 ? radius : 1);
			radius -= n;


			if (VALUE_AXIS_INVERT > 0)
			{
				float4 b = radius - min(d, radius);
				cutout = dot(b, 1);
			}
			else
			{
				float4 a = saturate(max(0, d - radius));

				a.xy = a.xz*a.yw;
				cutout = a.x * a.y;
			}

		#elif defined(_DISSOLVEMASKCOUNT_THREE)

			//1
			float3 p1_1 = VALUE_MASK_POSITION;
			float3 p1_2 = p1_1 + VALUE_MASK_NORMAL * VALUE_MASK_HEIGHT;
			
			float t1 = max(0, min(1, dot(vertexPos - p1_1, p1_2 - p1_1) / (VALUE_MASK_HEIGHT * VALUE_MASK_HEIGHT)));
			float3 projection1 = p1_1 + t1 * (p1_2 - p1_1);

			//2
			float3 p2_1 = VALUE_MASK_2_POSITION;
			float3 p2_2 = p2_1 + VALUE_MASK_2_NORMAL * VALUE_MASK_2_HEIGHT;

			float t2 = max(0, min(1, dot(vertexPos - p2_1, p2_2 - p2_1) / (VALUE_MASK_2_HEIGHT * VALUE_MASK_2_HEIGHT)));
			float3 projection2 = p2_1 + t2 * (p2_2 - p2_1);

			//3
			float3 p3_1 = VALUE_MASK_3_POSITION;
			float3 p3_2 = p3_1 + VALUE_MASK_3_NORMAL * VALUE_MASK_3_HEIGHT;

			float t3 = max(0, min(1, dot(vertexPos - p3_1, p3_2 - p3_1) / (VALUE_MASK_3_HEIGHT * VALUE_MASK_3_HEIGHT)));
			float3 projection3 = p3_1 + t3 * (p3_2 - p3_1);



			float3 d = float3(distance(vertexPos, projection1), distance(vertexPos, projection2), distance(vertexPos, projection3));
			
			float3 radius = float3(VALUE_MASK_RADIUS, VALUE_MASK_2_RADIUS, VALUE_MASK_3_RADIUS);
			
			float3 n = noise * (radius < 1 ? radius : 1);
			radius -= n;


			if (VALUE_AXIS_INVERT > 0)
			{
				float3 b = radius - min(d, radius);
				cutout = dot(b, 1);
			}
			else
			{
				float3 a = saturate(max(0, d - radius));
				cutout = a.x * a.y * a.z;
			}

		#elif defined(_DISSOLVEMASKCOUNT_TWO)

			//1
			float3 p1_1 = VALUE_MASK_POSITION;
			float3 p1_2 = p1_1 + VALUE_MASK_NORMAL * VALUE_MASK_HEIGHT;
			
			float t1 = max(0, min(1, dot(vertexPos - p1_1, p1_2 - p1_1) / (VALUE_MASK_HEIGHT * VALUE_MASK_HEIGHT)));
			float3 projection1 = p1_1 + t1 * (p1_2 - p1_1);

			//2
			float3 p2_1 = VALUE_MASK_2_POSITION;
			float3 p2_2 = p2_1 + VALUE_MASK_2_NORMAL * VALUE_MASK_2_HEIGHT;

			float t2 = max(0, min(1, dot(vertexPos - p2_1, p2_2 - p2_1) / (VALUE_MASK_2_HEIGHT * VALUE_MASK_2_HEIGHT)));
			float3 projection2 = p2_1 + t2 * (p2_2 - p2_1);



			float2 d = float2(distance(vertexPos, projection1), distance(vertexPos, projection2));
			
			float2 radius = float2(VALUE_MASK_RADIUS, VALUE_MASK_2_RADIUS);
			
			float2 n = noise * (radius < 1 ? radius : 1);
			radius -= n;


			if (VALUE_AXIS_INVERT > 0)
			{
				float2 b = radius - min(d, radius);
				cutout = dot(b, 1);
			}
			else
			{
				float2 a = saturate(max(0, d - radius));
				cutout = a.x * a.y;
			}
		#else
			float3 p1 = VALUE_MASK_POSITION;
			float3 p2 = p1 + VALUE_MASK_NORMAL * VALUE_MASK_HEIGHT;
			
			float t = max(0, min(1, dot(vertexPos - p1, p2 - p1) / (VALUE_MASK_HEIGHT * VALUE_MASK_HEIGHT)));
			float3 projection = p1 + t * (p2 - p1);

			float d = distance(vertexPos, projection);


			float radius = VALUE_MASK_RADIUS;
			noise *= (radius < 1 ? radius : 1);

			//radius += abs(noise) * (1 - VALUE_AXIS_INVERT * 2);
			radius -= noise;

			if (VALUE_AXIS_INVERT > 0)
			{
				cutout = radius - min(d, radius);
			}
			else
			{
				cutout = max(0, d - radius);
			} 
		#endif
	#elif defined(_DISSOLVEMASK_CONE)

		#if defined(_DISSOLVEMASKCOUNT_FOUR)
			
			//1
			float3 p1_1 = VALUE_MASK_POSITION;
			float3 p1_2 = p1_1 + VALUE_MASK_NORMAL * VALUE_MASK_HEIGHT;
			
			float t1 = max(0, min(1, dot(vertexPos - p1_1, p1_2 - p1_1) / (VALUE_MASK_HEIGHT * VALUE_MASK_HEIGHT)));
			float3 projection1 = p1_1 + t1 * (p1_2 - p1_1);

			//2
			float3 p2_1 = VALUE_MASK_2_POSITION;
			float3 p2_2 = p2_1 + VALUE_MASK_2_NORMAL * VALUE_MASK_2_HEIGHT;

			float t2 = max(0, min(1, dot(vertexPos - p2_1, p2_2 - p2_1) / (VALUE_MASK_2_HEIGHT * VALUE_MASK_2_HEIGHT)));
			float3 projection2 = p2_1 + t2 * (p2_2 - p2_1);

			//3
			float3 p3_1 = VALUE_MASK_3_POSITION;
			float3 p3_2 = p3_1 + VALUE_MASK_3_NORMAL * VALUE_MASK_3_HEIGHT;

			float t3 = max(0, min(1, dot(vertexPos - p3_1, p3_2 - p3_1) / (VALUE_MASK_3_HEIGHT * VALUE_MASK_3_HEIGHT)));
			float3 projection3 = p3_1 + t3 * (p3_2 - p3_1);

			//4
			float3 p4_1 = VALUE_MASK_4_POSITION;
			float3 p4_2 = p4_1 + VALUE_MASK_4_NORMAL * VALUE_MASK_4_HEIGHT;

			float t4 = max(0, min(1, dot(vertexPos - p4_1, p4_2 - p4_1) / (VALUE_MASK_4_HEIGHT * VALUE_MASK_4_HEIGHT)));
			float3 projection4 = p4_1 + t4 * (p4_2 - p4_1);



			float4 d = float4(distance(vertexPos, projection1), distance(vertexPos, projection2), distance(vertexPos, projection3), distance(vertexPos, projection4));


			float4 radius = lerp(0, float4(VALUE_MASK_RADIUS, VALUE_MASK_2_RADIUS, VALUE_MASK_3_RADIUS, VALUE_MASK_4_RADIUS), float4(t1, t2, t3, t4));

			
			float4 n = noise * (radius < 1 ? radius : 1);
			radius -= n;

			if (VALUE_AXIS_INVERT > 0)
			{
				float4 b = radius - min(d, radius);
				cutout = dot(b, 1);
			}
			else
			{
				float4 a = saturate(max(0, d - radius));

				a.xy = a.xz*a.yw;
				cutout = a.x * a.y;
			} 

		#elif defined(_DISSOLVEMASKCOUNT_THREE)
	
			//1
			float3 p1_1 = VALUE_MASK_POSITION;
			float3 p1_2 = p1_1 + VALUE_MASK_NORMAL * VALUE_MASK_HEIGHT;
			
			float t1 = max(0, min(1, dot(vertexPos - p1_1, p1_2 - p1_1) / (VALUE_MASK_HEIGHT * VALUE_MASK_HEIGHT)));
			float3 projection1 = p1_1 + t1 * (p1_2 - p1_1);

			//2
			float3 p2_1 = VALUE_MASK_2_POSITION;
			float3 p2_2 = p2_1 + VALUE_MASK_2_NORMAL * VALUE_MASK_2_HEIGHT;

			float t2 = max(0, min(1, dot(vertexPos - p2_1, p2_2 - p2_1) / (VALUE_MASK_2_HEIGHT * VALUE_MASK_2_HEIGHT)));
			float3 projection2 = p2_1 + t2 * (p2_2 - p2_1);

			//3
			float3 p3_1 = VALUE_MASK_3_POSITION;
			float3 p3_2 = p3_1 + VALUE_MASK_3_NORMAL * VALUE_MASK_3_HEIGHT;

			float t3 = max(0, min(1, dot(vertexPos - p3_1, p3_2 - p3_1) / (VALUE_MASK_3_HEIGHT * VALUE_MASK_3_HEIGHT)));
			float3 projection3 = p3_1 + t3 * (p3_2 - p3_1);
			

			float3 d = float3(distance(vertexPos, projection1), distance(vertexPos, projection2), distance(vertexPos, projection3));


			float3 radius = lerp(0, float3(VALUE_MASK_RADIUS, VALUE_MASK_2_RADIUS, VALUE_MASK_3_RADIUS), float3(t1, t2, t3));
			

			float3 n = noise * (radius < 1 ? radius : 1);
			radius -= n;

			if (VALUE_AXIS_INVERT > 0)
			{
				float3 b = radius - min(d, radius);
				cutout = dot(b, 1);
			}
			else
			{
				float3 a = saturate(max(0, d - radius));
				cutout = a.x * a.y * a.z;
			}

		#elif defined(_DISSOLVEMASKCOUNT_TWO)
	
			//1
			float3 p1_1 = VALUE_MASK_POSITION;
			float3 p1_2 = p1_1 + VALUE_MASK_NORMAL * VALUE_MASK_HEIGHT;
			
			float t1 = max(0, min(1, dot(vertexPos - p1_1, p1_2 - p1_1) / (VALUE_MASK_HEIGHT * VALUE_MASK_HEIGHT)));
			float3 projection1 = p1_1 + t1 * (p1_2 - p1_1);

			//2
			float3 p2_1 = VALUE_MASK_2_POSITION;
			float3 p2_2 = p2_1 + VALUE_MASK_2_NORMAL * VALUE_MASK_2_HEIGHT;

			float t2 = max(0, min(1, dot(vertexPos - p2_1, p2_2 - p2_1) / (VALUE_MASK_2_HEIGHT * VALUE_MASK_2_HEIGHT)));
			float3 projection2 = p2_1 + t2 * (p2_2 - p2_1);
			

			float2 d = float2(distance(vertexPos, projection1), distance(vertexPos, projection2));


			float2 radius = lerp(0, float2(VALUE_MASK_RADIUS, VALUE_MASK_2_RADIUS), float2(t1, t2));

			
			float2 n = noise * (radius < 1 ? radius : 1);
			radius -= n;

			if (VALUE_AXIS_INVERT > 0)
			{
				float2 b = radius - min(d, radius);
				cutout = dot(b, 1);
			}
			else
			{
				float2 a = saturate(max(0, d - radius));
				cutout = a.x * a.y;
			}

		#else
			float3 p1 = VALUE_MASK_POSITION;
			float3 p2 = p1 + VALUE_MASK_NORMAL * VALUE_MASK_HEIGHT;
			
			float t = max(0, min(1, dot(vertexPos - p1, p2 - p1) / (VALUE_MASK_HEIGHT * VALUE_MASK_HEIGHT)));
			float3 projection = p1 + t * (p2 - p1);

			float d = distance(vertexPos, projection);


			float radius = lerp(0, VALUE_MASK_RADIUS, t);
			noise *= (radius < 1 ? radius : 1);

			//radius += abs(noise) * (1 - VALUE_AXIS_INVERT * 2);
			radius -= noise;

			if (VALUE_AXIS_INVERT > 0)
			{
				cutout = radius - min(d, radius);
			}
			else
			{
				cutout = max(0, d - radius);
			} 
		#endif

	#endif

	
	return (cutout > 0 ? cutout : -1);
}


inline float4 ReadDissolveAlpha(float2 mainTexUV, float4 dissolveMapUV, float3 vertexPos)
{
	float4 alphaSource = 1;


	#ifdef _DISSOLVEMAPPINGTYPE_SCREEN_SPACE
		float2 screenUV = dissolveMapUV.xy / dissolveMapUV.w;
		screenUV.y *= _ScreenParams.y / _ScreenParams.x;
		screenUV *= distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz) * 0.1;
	#endif


	#if defined(_DISSOLVEALPHASOURCE_CUSTOM_MAP)

		#ifdef _DISSOLVEMAPPINGTYPE_SCREEN_SPACE
			screenUV = screenUV * VALUE_MAP1_ST.xy + VALUE_MAP1_ST.zw + VALUE_MAP1_SCROLL.xy * TIME;

			alphaSource = saturate(tex2D(VALUE_MAP1, screenUV) + (1 - VALUE_MAP1_INTENSITY).xxxx);
		#else
			alphaSource = saturate(tex2D(VALUE_MAP1, dissolveMapUV.xy) + (1 - VALUE_MAP1_INTENSITY).xxxx);
		#endif

		alphaSource.a = alphaSource[VALUE_MAP1_CHANNEL];

	#elif defined(_DISSOLVEALPHASOURCE_TWO_CUSTOM_MAPS)

		#ifdef _DISSOLVEMAPPINGTYPE_SCREEN_SPACE
			float2 uv1 = screenUV * VALUE_MAP1_ST.xy + VALUE_MAP1_ST.zw + VALUE_MAP1_SCROLL.xy * TIME;
			float2 uv2 = screenUV * VALUE_MAP2_ST.xy + VALUE_MAP2_ST.zw + VALUE_MAP2_SCROLL.xy * TIME;

			float4 t1 = saturate(tex2D(VALUE_MAP1, uv1) + (1 - VALUE_MAP1_INTENSITY).xxxx);
			float4 t2 = saturate(tex2D(VALUE_MAP2, uv2) + (1 - VALUE_MAP2_INTENSITY).xxxx);
		#else
			float4 t1 = saturate(tex2D(VALUE_MAP1, dissolveMapUV.xy) + (1 - VALUE_MAP1_INTENSITY).xxxx);
			float4 t2 = saturate(tex2D(VALUE_MAP2, dissolveMapUV.zw) + (1 - VALUE_MAP2_INTENSITY).xxxx);
		#endif

		t1.a = t1[VALUE_MAP1_CHANNEL];
		t2.a = t2[VALUE_MAP2_CHANNEL];

		alphaSource = lerp((t1 * t2), (t1 + t2) * 0.5, VALUE_ALPHATEXTUREBLEND);

	#elif defined(_DISSOLVEALPHASOURCE_THREE_CUSTOM_MAPS)

		#ifdef _DISSOLVEMAPPINGTYPE_SCREEN_SPACE
			float2 uv1 = screenUV * VALUE_MAP1_ST.xy + VALUE_MAP1_ST.zw + VALUE_MAP1_SCROLL.xy * TIME;
			float2 uv2 = screenUV * VALUE_MAP2_ST.xy + VALUE_MAP2_ST.zw + VALUE_MAP2_SCROLL.xy * TIME;
			float2 uv3 = screenUV * VALUE_MAP3_ST.xy + VALUE_MAP3_ST.zw + VALUE_MAP3_SCROLL.xy * TIME;
		#else
			float2 uv1 = dissolveMapUV.xy * VALUE_MAP1_ST.xy + VALUE_MAP1_ST.zw + VALUE_MAP1_SCROLL.xy * TIME;
			float2 uv2 = dissolveMapUV.xy * VALUE_MAP2_ST.xy + VALUE_MAP2_ST.zw + VALUE_MAP2_SCROLL.xy * TIME;
			float2 uv3 = dissolveMapUV.xy * VALUE_MAP3_ST.xy + VALUE_MAP3_ST.zw + VALUE_MAP3_SCROLL.xy * TIME;
		#endif

		float4 t1 = saturate(tex2D(VALUE_MAP1, uv1) + (1 - VALUE_MAP1_INTENSITY).xxxx);
		float4 t2 = saturate(tex2D(VALUE_MAP2, uv2) + (1 - VALUE_MAP2_INTENSITY).xxxx);
		float4 t3 = saturate(tex2D(VALUE_MAP3, uv3) + (1 - VALUE_MAP3_INTENSITY).xxxx);
		
		t1.a = t1[VALUE_MAP1_CHANNEL];
		t2.a = t2[VALUE_MAP2_CHANNEL];
		t3.a = t3[VALUE_MAP3_CHANNEL];

		alphaSource = lerp((t1 * t2 * t3), (t1 + t2 + t3) / 3.0, VALUE_ALPHATEXTUREBLEND);

	#else

		#ifdef _DISSOLVEMAPPINGTYPE_SCREEN_SPACE
			alphaSource = tex2D(_MainTex, screenUV * VALUE_MAIN_MAP_TILING);
		#else
			alphaSource = tex2D(_MainTex, mainTexUV * VALUE_MAIN_MAP_TILING);
		#endif

	#endif


	
	#if defined(_DISSOLVEMASK_XYZ_AXIS) || defined(_DISSOLVEMASK_PLANE) || defined(_DISSOLVEMASK_SPHERE) || defined(_DISSOLVEMASK_BOX) || defined(_DISSOLVEMASK_CYLINDER) || defined(_DISSOLVEMASK_CONE)
	
		float noise = ((alphaSource.a - 0.5) * 2) * VALUE_NOISE_STRENGTH;

		alphaSource.a = ReadMaskValue(vertexPos, noise);

	#endif	

	return alphaSource;
}
 



#ifdef _DISSOLVEMAPPINGTYPE_TRIPLANAR
	inline float4 ReadTriplanarTexture(sampler2D _texture, float3 coords, float3 blend, float intensity)
	{
		fixed4 cx = saturate(tex2D(_texture, coords.yz) + intensity.xxxx);
		fixed4 cy = saturate(tex2D(_texture, coords.xz) + intensity.xxxx);
		fixed4 cz = saturate(tex2D(_texture, coords.xy) + intensity.xxxx);

		return (cx * blend.x + cy * blend.y + cz * blend.z);
	}

	inline float4 ReadDissolveAlpha_Triplanar(float3 coords, float3 normal, float3 vertexPos)
	{	
		coords = lerp(vertexPos, coords, VALUE_TRIPLANARMAPPINGSPACE);


		half3 blend = abs(normal);
		blend /= dot(blend, 1.0);

		float4 alphaSource = 1;
		#if defined(_DISSOLVEALPHASOURCE_CUSTOM_MAP)

			alphaSource = ReadTriplanarTexture(VALUE_MAP1, coords * VALUE_MAP1_ST.x * 0.1 + VALUE_MAP1_SCROLL.xyz * TIME, blend, 1 - VALUE_MAP1_INTENSITY);

			alphaSource.a = alphaSource[VALUE_MAP1_CHANNEL];

		#elif defined(_DISSOLVEALPHASOURCE_TWO_CUSTOM_MAPS)

			float4 t1 = ReadTriplanarTexture(VALUE_MAP1, coords * VALUE_MAP1_ST.x * 0.1 + VALUE_MAP1_SCROLL.xyz * TIME, blend, 1 - VALUE_MAP1_INTENSITY);
			float4 t2 = ReadTriplanarTexture(VALUE_MAP2, coords * VALUE_MAP2_ST.x * 0.1 + VALUE_MAP2_SCROLL.xyz * TIME, blend, 1 - VALUE_MAP2_INTENSITY);

			t1.a = t1[VALUE_MAP1_CHANNEL];
			t2.a = t2[VALUE_MAP2_CHANNEL];

			alphaSource = lerp((t1 * t2), (t1 + t2) * 0.5, VALUE_ALPHATEXTUREBLEND);

		#elif defined(_DISSOLVEALPHASOURCE_THREE_CUSTOM_MAPS)

			float4 t1 = ReadTriplanarTexture(VALUE_MAP1, coords * VALUE_MAP1_ST.x * 0.1 + VALUE_MAP1_SCROLL.xyz * TIME, blend, 1 - VALUE_MAP1_INTENSITY);
			float4 t2 = ReadTriplanarTexture(VALUE_MAP2, coords * VALUE_MAP2_ST.x * 0.1 + VALUE_MAP2_SCROLL.xyz * TIME, blend, 1 - VALUE_MAP2_INTENSITY);
			float4 t3 = ReadTriplanarTexture(VALUE_MAP3, coords * VALUE_MAP3_ST.x * 0.1 + VALUE_MAP3_SCROLL.xyz * TIME, blend, 1 - VALUE_MAP3_INTENSITY);

			t1.a = t1[VALUE_MAP1_CHANNEL];
			t2.a = t2[VALUE_MAP2_CHANNEL];
			t3.a = t3[VALUE_MAP3_CHANNEL];

			alphaSource = lerp((t1 * t2 * t3), (t1 + t2 + t3) / 3.0, VALUE_ALPHATEXTUREBLEND);

		#else		

			alphaSource = ReadTriplanarTexture(_MainTex, coords * VALUE_MAIN_MAP_TILING * 0.1, blend, 0).a;

		#endif



		#if defined(_DISSOLVEMASK_XYZ_AXIS) || defined(_DISSOLVEMASK_PLANE) || defined(_DISSOLVEMASK_SPHERE) || defined(_DISSOLVEMASK_BOX) || defined(_DISSOLVEMASK_CYLINDER) || defined(_DISSOLVEMASK_CONE)

			float noise = ((alphaSource.a - 0.5) * 2) * VALUE_NOISE_STRENGTH;

			alphaSource.a = ReadMaskValue(vertexPos, noise);

		#endif	


		return alphaSource;
	}
#endif

inline void DoDissolveClip(float4 alpha)
{
	#if defined(_DISSOLVEMASK_XYZ_AXIS) || defined(_DISSOLVEMASK_PLANE) || defined(_DISSOLVEMASK_SPHERE) || defined(_DISSOLVEMASK_BOX) || defined(_DISSOLVEMASK_CYLINDER) || defined(_DISSOLVEMASK_CONE)
		clip(alpha.a);
	#else
		clip(alpha.a - VALUE_CUTOFF * 1.01);
	#endif
}


float DoDissolveAlbedoEmission(float4 alpha, inout float3 albedo, inout float3 emission, inout float2 uv)
{	
	float retValue = 0;


	float dissolveEdgeSize = VALUE_EDGE_SIZE;


	#ifdef UNITY_PASS_META
		dissolveEdgeSize *= VALUE_GI_MULTIPLIER;
	#endif


	#if defined(_DISSOLVEMASK_XYZ_AXIS) || defined(_DISSOLVEMASK_PLANE) || defined(_DISSOLVEMASK_SPHERE) || defined(_DISSOLVEMASK_BOX)  || defined(_DISSOLVEMASK_CYLINDER) || defined(_DISSOLVEMASK_CONE)
		
	#else
	
		dissolveEdgeSize *= VALUE_CUTOFF < 0.1 ? (VALUE_CUTOFF * 10) : 1;

		alpha.a -= VALUE_CUTOFF;

	#endif	



	if (dissolveEdgeSize > 0 && dissolveEdgeSize > alpha.a)
	{
		float edgeGradient = saturate(alpha.a) * (1.0 / dissolveEdgeSize);


		float invertGradient = 1 - edgeGradient;
		uv += (lerp(alpha.rg, tex2D(_MainTex, uv).rg, VALUE_EDGE_DISTORTION_SOURCE) - 0.5) * VALUE_EDGE_DISTORTION * invertGradient * invertGradient;
	
		float4 edgeColor = VALUE_EDGE_COLOR;


		
		float4 edgeTexture = 1;
		#if defined(_DISSOLVEEDGETEXTURESOURCE_MAIN_MAP)
			#if (SHADER_TARGET < 30)
				edgeTexture = tex2D(_MainTex, uv);
			#else
				edgeTexture = tex2Dlod(_MainTex, float4(uv, 0, VALUE_EDGE_TEXTURE_MIPMAP));
			#endif				

			edgeTexture.a = saturate(edgeTexture.a + VALUE_EDGETEXTUREALPHAOFFSET);

		#elif defined(_DISSOLVEEDGETEXTURESOURCE_GRADIENT) && !defined(_DISSOLVEMASK_BOX)

			float u = lerp(edgeGradient, 1 - edgeGradient, VALUE_EDGE_TEXTURE_REVERSE) + VALUE_EDGE_TEXTURE_OFFSET;

			edgeTexture = tex2D(VALUE_EDGE_TEXTURE, float2(lerp(u, VALUE_CUTOFF, VALUE_EDGE_TEXTURE_IS_DYNAMIC), 0.5));

			edgeTexture.a = saturate(edgeTexture.a + VALUE_EDGETEXTUREALPHAOFFSET);

		#elif defined(_DISSOLVEEDGETEXTURESOURCE_CUSTOM)
			#if (SHADER_TARGET < 30)
				edgeTexture = tex2D(VALUE_EDGE_TEXTURE, uv);
			#else
				edgeTexture = tex2Dlod(VALUE_EDGE_TEXTURE, float4(uv, 0, VALUE_EDGE_TEXTURE_MIPMAP));
			#endif

			edgeTexture.a = saturate(edgeTexture.a + VALUE_EDGETEXTUREALPHAOFFSET);
		#endif

		edgeColor *= edgeTexture;

		//Box mask always has solid edge
		#if !defined(_DISSOLVEMASK_BOX)
			float3 shape;
			shape.x = 1;
			shape.y = invertGradient;
			shape.z = invertGradient * invertGradient;

			edgeColor.a *= shape[(int)VALUE_EDGE_SHAPE];
		#endif
		



		albedo = edgeColor.rgb;
		
		
		#ifdef UNITY_PASS_META
			
			#if defined(_DISSOLVEMASK_XYZ_AXIS) || defined(_DISSOLVEMASK_PLANE) || defined(_DISSOLVEMASK_SPHERE) || defined(_DISSOLVEMASK_BOX)  || defined(_DISSOLVEMASK_CYLINDER) || defined(_DISSOLVEMASK_CONE)

			#else
				edgeColor *= VALUE_CUTOFF >= 0.99 ? 0 : 1;
			#endif

			emission = edgeColor.rgb * (1 + VALUE_EDGE_COLOR_INTENSITY) * VALUE_GI_MULTIPLIER;			
		#else
			emission = edgeColor.rgb * VALUE_EDGE_COLOR_INTENSITY;
		#endif
			

		retValue = saturate(edgeColor.a);
	}


	#ifdef UNITY_PASS_META
		if (alpha.a <= 0)
			emission = const_zero;
	#endif


	return retValue;
}


half4 DoOutputForward(half4 output, half alphaFromSurface)
{
	#if defined(_ALPHABLEND_ON) || defined(_ALPHAPREMULTIPLY_ON)
		output.a = alphaFromSurface;
	#else
		UNITY_OPAQUE_ALPHA(output.a);
	#endif

	return output;
}
