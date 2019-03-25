using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalEvent : MonoBehaviour 
{
	private SignalManager signalManager;
	//instantiate the particle system
	public ParticleSystem particle;
	private ParticleSystem stopper;
	public List<ParticleCollisionEvent> collisionEvents;	
	
	//instantiate the material
	private Material _material;
	private Color startColor = Color.black;
	public Color highlightColor = Color.cyan;
	
	private float duration = 1f;
	private float timer;

	void Awake ()
	{
		signalManager = GameObject.FindObjectOfType<SignalManager>();
	}

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
	}
	void OnParticleCollision (GameObject other)
	{
		Debug.Log("Particle Collision Triggered");
		_material.SetColor("_EmissionColor", highlightColor);
		timer = 0f;
		stopper = other.GetComponent<ParticleSystem>();
		stopper.Stop();
		Debug.Log(stopper.gameObject.name + " was stopped.");
		signalManager.SendNextSignal(stopper);
	}
	
	// Update is called once per frame
	void Update () {
		if (timer < 1) {
			timer += Time.deltaTime;
			_material.SetColor("_EmissionColor", Color.Lerp(highlightColor, startColor, timer/duration));
		} else {
			timer = 1f;
		}
		
	}
}
