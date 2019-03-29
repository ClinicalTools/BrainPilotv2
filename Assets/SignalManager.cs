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

	private const int DEFAULT = 0;
	private const int PARTICLE_COLLISION = 10;

	void Start () {
		//StopAll();
	}

	public void StopAll()
	{
		foreach(ParticleSystem sys in relayChain) {
			sys.Stop();
		}
		active = false;
	}

	public void Play()
	{
		relayChain[0].Play();
		relayChain[0].GetComponent<ParticleSeekOptimized>().target.gameObject.layer = PARTICLE_COLLISION;
		active = true;
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
		foreach(ParticleSystem part in relayChain)
		{
			index++;
			if(part == startingPoint)
			{
				if (relayChain.Length == (index + 1))
				{
					destination = relayChain[0];
				} else 
				{
					destination = relayChain[index+1];
				}
				break;
			}
		}
		if (destination != null)
		{
			return destination;
		} else return relayChain[0];
	}
	
	public void SendNextSignal(ParticleSystem startingPoint)
	{
		ParticleSystem nextPoint = FindNextSignal(startingPoint);
		ps = nextPoint.GetComponent<ParticleSystem>();
		ps.Play();
		ps.GetComponent<ParticleSeekOptimized>().target.gameObject.layer = PARTICLE_COLLISION;
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
	
	// Update is called once per frame
	void Update () {
		
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
