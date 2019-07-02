using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTex : MonoBehaviour {

	public float Scrollx = 0.5f;
	public float Scrolly = 0.5f;

	Renderer r;

	private void Start()
	{
		r = GetComponent<Renderer>();
	}

	// Update is called once per frame
	void Update () {
		if (r == null) {
			r = GetComponent<Renderer>();
		}
		float OffsetX = Time.time * Scrollx;
		float OffsetY = Time.time * Scrolly;
		r.material.mainTextureOffset = new Vector2(OffsetX,OffsetY);
	}
}
