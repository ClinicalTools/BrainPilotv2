using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class SignalManager : MonoBehaviour {

	public ParticleSystem destination;
	public ParticleSystem[] relayChain;
	private ParticleSystem signalEmitter;
	private ParticleSystem ps;
	public bool active;
	public bool loop = true;

	private const int DEFAULT = 0;
	private const int PARTICLE_COLLISION = 10;

	void Awake () {
		if (relayChain == null) {
			relayChain = new ParticleSystem[0];
		}
		//StopAll();
		//Debug.Log(relayChain.Length);
		List<ParticleSystem> psList = new List<ParticleSystem>(relayChain);
		for(int i = 0; i < psList.Count; i++) {
			if (psList[i] == null) {
				psList.RemoveAt(i);
				i--;
			}
		}
		relayChain = psList.ToArray();
		Debug.Log(relayChain.Length);

	}

	public void StopAll()
	{
		foreach(ParticleSystem sys in relayChain) {
			if (sys == null) {
				continue;
			}
			sys?.Stop();
		}
		active = false;
	}

	public void Play()
	{
		if (relayChain.Length == 0) {
			return;
		}
		relayChain[0].Play();
		relayChain[0].GetComponent<ParticleSeekOptimized>().target.gameObject.layer = PARTICLE_COLLISION;
		active = true;
	}

	float timer;
	ParticleSystem activeSystem;
	private void Update()
	{
		if (active) {
			if (timer < 0 && activeSystem != null) {
				SendNextSignal(activeSystem);
			} else {
				timer -= Time.deltaTime;
			}
		}
	}

	public void ReduceParticleLifetime(ParticleSystem system)
	{
		//Reduce the lifetime of the particles which hit
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[10];
		int particlesAlive = system.GetParticles(particles);
		if (particles != null) {
			for (int i = 0; i < particlesAlive; i++) {
				//particles[i].velocity = Vector3.zero;
				particles[i].startLifetime = 1;
				particles[i].remainingLifetime = .8f;
			}
			system.SetParticles(particles, particlesAlive);
		}
	}

	void OnEnable()
	{
		//RelayManager.Relay += NextSignal;
	}
	void OnDisable()
	{
		//RelayManager.Relay -= NextSignal;
	}

	//NextSignal is given an GameObject from a relayChain and returns the next GameObject.
	//If it doesn't find 
	ParticleSystem FindNextSignal(ParticleSystem startingPoint)
	{
		destination = null;
		int index = -1;

		if (relayChain.Length == 0) {
			return null;
		}

		for(int i = 0; i < relayChain.Length; i++) {
			if (relayChain[i] == startingPoint) {
				//Match
				index = i + 1;
				break;
			}
		}
		if (index == -1) {
			//No match
			return null;
		} else if (index == relayChain.Length) {
			//End of list
			if (loop) {
				return relayChain[0];
			} else {
				return null;
			}
		} else {
			//Middle of list
			return relayChain[index];
		}
		{
			/*
			//CAN'T HANDLE A LIST OF JUST ONE ELEMENT
			foreach(ParticleSystem part in relayChain)
			{
				index++;
				if(part == startingPoint)
				{
					if (relayChain.Length == (index + 1))
					{
						if (loop) {
							destination = relayChain[0];
						}
					} else 
					{
						destination = relayChain[index+1];
					}
					break;
				}
			}
			if (destination != null) {
				return destination;
			} else if (loop) {
				return relayChain[0];
			} else {
				return null;
			}*/
		}
	}

	
	public void SendNextSignal(ParticleSystem startingPoint)
	{
		//Reduce the remaining lifetime of the active particles
		ReduceParticleLifetime(startingPoint);

		//Find the next point
		ParticleSystem nextPoint = FindNextSignal(startingPoint);
		if (nextPoint == null) {
			Debug.Log("NextPoint is null");
			activeSystem = null;
			return;
		}

		//Play the attached particlesystem
		ps = nextPoint.GetComponent<ParticleSystem>();
		ps.Play();
		
		//Adjust the particle seek
		if (ps.GetComponent<ParticleSeekOptimized>()) {
			ps.GetComponent<ParticleSeekOptimized>().target.gameObject.layer = PARTICLE_COLLISION;
			ps.trigger.SetCollider(0, ps.GetComponent<ParticleSeekOptimized>().target.GetComponent<Collider>());
		}

		//Set up the checks to make sure the particles hit
		timer = ps.main.startLifetimeMultiplier;
		activeSystem = ps;
	}

	/// <summary>
	/// This assumes only one instance of a particle system in the chain
	/// </summary>
	/// <param name="hit">The hit particle system</param>
	/// <param name="sender">The system which sent the colliding particle</param>
	/// <returns></returns>
	public bool VerifyOrder(ParticleSystem hit, ParticleSystem sender)
	{
		for(int i = 0; i < relayChain.Length; i++) {
			if (relayChain[i] == sender) {
				if (relayChain[i + 1] == hit) {
					return true;
				} else {
					return false;
				}
			}
		}
		return false;
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(SignalManager))]
	public class SignalManagerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Start Over")) {
				((SignalManager)target).StopAll();
				((SignalManager)target).Play();
			}
		}
	}

#endif
}
