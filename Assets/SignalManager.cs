using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SignalManager : MonoBehaviour {

	public ParticleSystem destination;
	public ParticleSystem[] relayChain;
	private ParticleSystem signalEmitter;
	private ParticleSystem ps;

	void Start () {
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
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
