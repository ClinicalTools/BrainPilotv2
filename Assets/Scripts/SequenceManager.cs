using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceManager : MonoBehaviour {

	private List<Sequence> sequences;
	private int activeIdx = 0;
	public bool isAtEnd
	{
		get
		{
			return sequences.Count == 0 | activeIdx == sequences.Count - 1;
		}
	}

	public bool isAtBeginning
	{
		get
		{
			return activeIdx == 0;
		}
	}

	public int count
	{
		get
		{
			return sequences.Count;
		}
	}

	// Use this for initialization
	void Start () {
		sequences = new List<Sequence>(GetComponentsInChildren<Sequence>());
	}

	public Sequence GetSequence()
	{
		return sequences[activeIdx];
	}

	public Sequence GetSequenceAt(int idx)
	{
		return sequences[idx];
	}

	/// <summary>
	/// Advances the active sequence and returns the new active sequence.
	/// </summary>
	/// <returns>The active sequence (or the last sequence available)</returns>
	public Sequence AdvanceSequence()
	{
		if (!isAtEnd) {
			activeIdx++;
		}
		return GetSequence();
	}

	/// <summary>
	/// Receeds the sequence and returns the new active sequence. Returns the first sequence if it cannot receed further.
	/// </summary>
	/// <returns>The previous sequence</returns>
	public Sequence ReceedSequence()
	{
		if (!isAtBeginning) {
			activeIdx--;
		}
		return GetSequence();
	}
}
