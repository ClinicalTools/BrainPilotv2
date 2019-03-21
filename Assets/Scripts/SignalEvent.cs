using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalEvent : MonoBehaviour 
{
	//instantiate the particle system
	public ParticleSystem particle;
	public List<ParticleCollisionEvent> collisionEvents;	
	
	//instantiate the material
	private Material _material;
	private Color startColor = Color.black;
	public Color highlightColor = Color.cyan;
	
	private float duration = 1f;
	private float start;
	private float timer;
	private bool isFading;
	private const float ALPHA_MAX = 1f;
	private const float ALPHA_MIN = 0f;


	void Start () 
	{
		//Fetching the Renderer from the Object
		Renderer rend = GetComponent<Renderer>();
		_material = rend.material;
		startColor = _material.GetColor("_EmissionColor");
		//Fetching the Particle System
		particle = GetComponent<ParticleSystem>();
		collisionEvents = new List<ParticleCollisionEvent>();
		timer = 1f;
		isFading = false;
	}
	void OnParticleCollision (GameObject other)
	{
		Debug.Log("Particle Collision Triggered");
		_material.SetColor("_EmissionColor", highlightColor);
		timer = 0f;
		isFading = true;

	}
	
	// Update is called once per frame
	void Update () {
		if (timer < 1) {
			timer += Time.deltaTime;
			_material.SetColor("_EmissionColor", Color.Lerp(highlightColor, startColor, timer/duration));
		} else {
			//Debug.Log("Time reset");
			isFading = false;
			timer = 1f;
		}
		
	}
}
