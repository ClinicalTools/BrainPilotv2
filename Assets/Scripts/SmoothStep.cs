using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothStep {


	public static float SmoothStart(int pow, float time)
	{
		//x^2

		float calcValue = time;
		for(int i = 1; i < pow; i++) {
			calcValue *= time;
		}
		return calcValue;
	}

	public static float SmoothStop(int pow, float time)
	{
		//1 - (1-x)^2

		float baseVal = (1 - time);
		float calcVal = baseVal;
		for (int i = 1; i < pow; i++) {
			calcVal *= baseVal;
		}
		return 1 - calcVal;
	}
	
	public static float Mix(int powStart, int powStop, float weightStop, float time)
	{
		return SmoothStart(powStart, time) * (1 - weightStop) + weightStop * SmoothStop(powStop, time);
	}

	public static float Crossfade(int powStart, int powStop, float time)
	{
		return Mix(powStart, powStop, time, time);
	}

	public static float Crossfade(int pow, float time)
	{
		return Crossfade(pow, pow, time);
	}
}
