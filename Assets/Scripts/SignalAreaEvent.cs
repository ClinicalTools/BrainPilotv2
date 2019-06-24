using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalAreaEvent : MonoBehaviour 
{
	[SerializeField]
	private SignalManager signalManager;
	//instantiate the particle system
	private ParticleSystem particle;
	private ParticleSystem stopper;
	private ParticleSystem ps;
	public List<ParticleCollisionEvent> collisionEvents;	
	
	//instantiate the material
	//private Material _material;
	//private Color startColor = Color.black;
	private Color highlightColor = Color.cyan;
	
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
	}

	void Start () 
	{
		//Fetching the Particle System
		particle = GetComponentInChildren<ParticleSystem>();
		collisionEvents = new List<ParticleCollisionEvent>();
		ps = gameObject.GetComponent<ParticleSystem>();
		timer = 1f;
	}

	//other represents the particle system that sent the colliding particle
	void OnParticleCollision (GameObject other)
	{
		gameObject.layer = 0;
		if (!signalManager.active) {
			return;
		}
		Debug.Log("Particle Collision Triggered");
		timer = 0f;
		stopper = other.GetComponent<ParticleSystem>();
		var mainstopper = stopper.main;
		highlightColor = mainstopper.startColor.color;
		stopper.Stop();
		var mainModule = ps.main;
		mainModule.startColor = highlightColor;
		Debug.Log(stopper.gameObject.name + " was stopped.");
		StartCoroutine(DelayCall(.5f, stopper));
		particle.Play();
		signalManager.SendNextSignal(stopper);
	}

	private IEnumerator DelayCall(float time, ParticleSystem system)
	{
		yield return new WaitForSecondsRealtime(time);
		signalManager.SendNextSignal(system);
	}
	
	// Update is called once per frame
	void Update () {
		if (timer < duration) {
			timer += Time.deltaTime;
			ps.Play();
		} else {
			ps.Stop();
			timer = duration;
		}
		
	}
}
