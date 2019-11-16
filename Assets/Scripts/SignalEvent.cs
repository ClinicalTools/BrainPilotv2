using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalEvent : MonoBehaviour 
{
	[SerializeField]
	private SignalManager signalManager;
	[SerializeField]
	private List<SignalManager> signalManagers;
	//instantiate the particle system
	//public ParticleSystem particle;
	private ParticleSystem stopper;
	public List<ParticleCollisionEvent> collisionEvents;	
	
	//instantiate the material
	private Material _material;
	private Color startColor = Color.black;
	private Color highlightColor = Color.cyan;
	
	public float duration = 1f;
	public float timer;

	void Awake ()
	{
		if (signalManager == null) {
			Debug.LogWarning("Did you forget to assign a signal manager?", gameObject);
			SignalManager[] array = GameObject.FindObjectsOfType<SignalManager>();
			foreach (SignalManager sm in array) {
				if (sm.gameObject.scene == gameObject.scene) {
					Debug.Log("Loaded the chosen signal manager", sm.gameObject);
					signalManager = sm;
					break;
				}
			}
		}
		if (block == null) {
			block = new MaterialPropertyBlock();
		}
	}

	void Start ()
	{
		//Fetching the Renderer from the Object
		Renderer rend = GetComponent<Renderer>();
		if (GetComponent<MaterialSwitchState>()) {
			startColor = GetComponent<MaterialSwitchState>().renderer.sharedMaterial.GetColor("_EmissionColor");
			//_material = GetComponent<MaterialSwitchState>().renderer.material;
		} else {
			startColor = rend.sharedMaterial.GetColor("_EmissionColor");
			//_material = rend.material;
		}
		//startColor = _material.GetColor("_EmissionColor");
		//Fetching the Particle System
		/*if (particle == null) {
			particle = GetComponentInChildren<ParticleSystem>();
		}*/
		collisionEvents = new List<ParticleCollisionEvent>();
		timer = 1f;
		state = GetComponent<MaterialSwitchState>();
	}
	//other represents the particle system that sent the colliding particle
	void OnParticleCollision (GameObject other)
	{
		//Ensure the collision should happen
		if (gameObject.layer != 10) {
			return;
		}
		gameObject.layer = 0;
		if (!signalManagers.Find(x => x.active)) {
			return;
		}
		/*
		if (!signalManager.active) {
			return;
		}*/
		
		timer = 0f;
		stopper = other.GetComponent<ParticleSystem>();
		var mainstopper = stopper.main;
		highlightColor = mainstopper.startColor.color;

		//stopper.Stop();

		//This kills all particles and deletes their trails as well
		//stopper.SetParticles(new ParticleSystem.Particle[] { }, 0);
		
		//Debug.Log(stopper.gameObject.name + " was stopped.", stopper.gameObject);
		StartCoroutine(DelayCall(.5f, stopper));
		//signalManager.SendNextSignal(stopper);
	}

	private IEnumerator DelayCall(float time, ParticleSystem system)
	{
		yield return new WaitForSecondsRealtime(time);

		foreach(SignalManager manager in signalManagers) {
			manager.SendNextSignal(system);
		}

		//signalManager.SendNextSignal(system);
	}

	MaterialPropertyBlock block;
	public bool activated;
	// Update is called once per frame
	MaterialSwitchState state;
	void Update () {
		if (timer < duration) {
			activated = true;
			timer += Time.deltaTime;
			block.SetColor("_EmissionColor", Color.Lerp(highlightColor, startColor, timer / duration));

			if (state != null) {  // || (state = GetComponent<MaterialSwitchState>()) != null) {
				state.renderer.SetPropertyBlock(block);
			} else {
				GetComponent<Renderer>().SetPropertyBlock(block);
			}
			//GetComponent<Renderer>().SetPropertyBlock(block);
			//_material.SetColor("_EmissionColor", Color.Lerp(highlightColor, startColor, timer/duration));
		} else {
			//state.renderer.SetPropertyBlock(null);
			timer = duration;
			activated = false;
		}
		
	}
}
