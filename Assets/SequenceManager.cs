using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceManager : MonoBehaviour {

	private List<Sequence1> sequences;

	// Use this for initialization
	void Start () {
		sequences = new List<Sequence1>(GetComponentsInChildren<Sequence1>());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Sequence1 GetSequence()
	{
		return sequences[0];
	}
}
