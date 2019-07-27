using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Sequence : MonoBehaviour {
	public string sequenceTitle;
	public bool startOnLoad;
	public SequenceElement[] steps;

	[SerializeField]
	private bool _active;
	[SerializeField]
	private int _stepIdx = 0;

	public SequenceElement GetActiveStep()
	{
		if (steps.Length == 0) {
			return null;
		}
		return steps[_stepIdx];
	}

	[ContextMenu("Load elements")]
	public void LoadElements()
	{
		steps = new SequenceElement[transform.childCount];
		for(int i = 0; i < transform.childCount; i++) {
			steps[i] = transform.GetChild(i).GetComponent<SequenceElement>();
		}
		
	}

	public void AdvanceSequence()
	{
		if (_stepIdx == steps.Length - 1) {
			FinishSequence(false);
		} else {
			steps[_stepIdx].Deactivate();
			_stepIdx++;
			steps[_stepIdx].Activate();
		}
	}

	/// <summary>
	/// Receeds the sequence
	/// </summary>
	/// <returns>true if successful, false if not</returns>
	public bool RecedeSequence()
	{
		if (_stepIdx == 0) {
			return false;
		} else {
			steps[_stepIdx].Deactivate();
			_stepIdx--;
			steps[_stepIdx].Activate();
			return true;
		}
	}

	

	public void StartSequence()
	{
		if (steps.Length == 0) {
			_active = false;
			return;
		}
		steps[_stepIdx]?.Activate();
		_active = true;
	}

	public void PauseSequence()
	{
		steps[_stepIdx].Deactivate();
		_active = false;
	}

	public void ResumeSequence()
	{
		steps[_stepIdx].Activate();
		_active = true;
	}

	public void FinishSequence(bool reset)
	{
		steps[_stepIdx]?.Deactivate();
		_active = false;
		if (reset) {
			ResetSequence();
		}
	}

	public void ResetSequence()
	{
		_stepIdx = 0;
	}

	public int GetActiveIndex()
	{
		return _stepIdx;
	}

	public bool IsActive()
	{
		return _active;
	}
}
