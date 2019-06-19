using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTex : MonoBehaviour {

	public float Scrollx = 0.5f;
	public float Scrolly = 0.5f;
	
	// Update is called once per frame
	void Update () {
		float OffsetX = Time.time * Scrollx;
		float OffsetY = Time.time * Scrolly;
		GetComponent<Renderer>().material.mainTextureOffset = new Vector2(OffsetX,OffsetY);
	}
}
