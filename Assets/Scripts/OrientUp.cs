using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientUp : MonoBehaviour {

	RectTransform rt;
	Vector3 eulers;
	private void Awake()
	{
		rt = GetComponent<RectTransform>();
	}

	// Update is called once per frame
	void Update () {
		if (rt.eulerAngles.z != 0) {
			eulers = rt.eulerAngles;
			eulers.z = 0;
			rt.eulerAngles = eulers;
		}
	}
}
