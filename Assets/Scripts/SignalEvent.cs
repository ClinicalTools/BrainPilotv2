using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalEvent : MonoBehaviour 
{
	[SerializeField]
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
		particle = GetComponentInChildren<ParticleSystem>();
		collisionEvents = new List<ParticleCollisionEvent>();
		timer = 1f;
	}

	//other represents the particle system that sent the colliding particle
	void OnParticleCollision (GameObject other)
	{
		gameObject.layer = 0;
		if (!signalManager.active) {
			return;
		}
		//Debug.Log("Particle Collision Triggered");
		//_material.SetColor("_EmissionColor", highlightColor);
		timer = 0f;
		stopper = other.GetComponent<ParticleSystem>();
		stopper.Stop();
		
		Debug.Log(stopper.gameObject.name + " was stopped.");
		StartCoroutine(DelayCall(.5f, stopper));
		//signalManager.SendNextSignal(stopper);
	}

	private IEnumerator DelayCall(float time, ParticleSystem system)
	{
		yield return new WaitForSecondsRealtime(time);
		signalManager.SendNextSignal(system);
	}

	MaterialPropertyBlock block;
	public bool activated;
	// Update is called once per frame
	void Update () {
		if (timer < duration) {
			activated = true;
			timer += Time.deltaTime;
			block.SetColor("_EmissionColor", Color.Lerp(highlightColor, startColor, timer / duration));

			if (GetComponent<MaterialSwitchState>()) {
				GetComponent<MaterialSwitchState>().renderer.SetPropertyBlock(block);
			} else {
				GetComponent<Renderer>().SetPropertyBlock(block);
			}
			//GetComponent<Renderer>().SetPropertyBlock(block);
			//_material.SetColor("_EmissionColor", Color.Lerp(highlightColor, startColor, timer/duration));
		} else {
			timer = duration;
			activated = false;
		}
		
	}
}
