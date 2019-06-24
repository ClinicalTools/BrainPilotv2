using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalEditor : MonoBehaviour {

	public Material signalMaterial;
	public Color signalColor;
	private ParticleSystem ps;
	private ParticleSystemRenderer psr;

	// Use this for initialization
	void Awake () {
		signalMaterial = GetComponent<Renderer>().material;
		signalMaterial.SetColor("_Color", signalColor);
		ps = GetComponent<ParticleSystem>();
		psr = GetComponent<ParticleSystemRenderer>();
		psr.trailMaterial.SetColor("_Color", signalColor);
	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//signalMaterial.color = signalColor;
		//psr.trailMaterial.SetColor("_Color", signalColor);
	}
}
