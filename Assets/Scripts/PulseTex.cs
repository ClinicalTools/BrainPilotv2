using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseTex : MonoBehaviour {

	public float lowDepth = .5f;
	public float highDepth = .8f;
	public float pulseSpeed = 1f;
	private float strength;

	Renderer m_Renderer;

	// Use this for initialization
	void Start () {
		m_Renderer = GetComponent<Renderer> ();
		m_Renderer.material.EnableKeyword("_NORMALMAP");
	}
	
	// Update is called once per frame
	void Update () {
		strength = Mathf.Lerp(lowDepth, highDepth, Mathf.Sin(Time.time*pulseSpeed));
		m_Renderer.material.SetFloat("_BumpScale", strength);
	}
}
