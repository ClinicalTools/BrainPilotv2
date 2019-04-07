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

	void Start () {
		//StopAll();
	}

	public void StopAll()
	{
		foreach(ParticleSystem sys in relayChain) {
			sys?.Stop();
		}
		active = false;
	}

	public void Play()
	{
		relayChain[0].Play();
		relayChain[0].GetComponent<ParticleSeekOptimized>().target.gameObject.layer = PARTICLE_COLLISION;
		active = true;
		particles = new ParticleSystem.Particle[5];
		relayChain[0].GetParticles(particles);
		particle = particles[0];
		Invoke("a", .3f);
	}
	public void a()
	{
		trackParticle = true;
	}

	ParticleSystem.Particle particle;
	ParticleSystem.Particle[] particles;
	bool trackParticle;

	void Update()
	{
		if (trackParticle) {
			relayChain[0].GetParticles(particles);
			print(particles[0].startLifetime + ": " + particles[0].remainingLifetime);
			if (particles[0].remainingLifetime <= 0) {
				trackParticle = false;
			}
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
		ParticleSystem nextPoint = FindNextSignal(startingPoint);
		if (nextPoint == null) {
			Debug.Log("NextPoint is null");
			return;
		}
		ps = nextPoint.GetComponent<ParticleSystem>();
		
		ps.Play();
		//Debug.Log("Playing " + ps.name, ps.gameObject);
		//Debug.Break();
		if (ps.GetComponent<ParticleSeekOptimized>()) {
			ps.GetComponent<ParticleSeekOptimized>().target.gameObject.layer = PARTICLE_COLLISION;
			ps.trigger.SetCollider(0, ps.GetComponent<ParticleSeekOptimized>().target.GetComponent<Collider>());
		}
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
