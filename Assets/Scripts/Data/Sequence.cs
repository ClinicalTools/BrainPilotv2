using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sequences/Sequence")]
public class Sequence : ScriptableObject {
	public string sequenceTitle;
	public SequenceElement[] steps;

	[SerializeField]
	private bool _active;
	[SerializeField]
	private int _stepIdx = 0;

	public SequenceElement GetActiveStep()
	{
		return steps[_stepIdx];
	}

	public void AdvanceSequence()
	{
		Debug.Log("Advancing Sequence in sequence");
		if (_stepIdx == steps.Length - 1) {
			FinishSequence();
		} else {
			steps[_stepIdx].Deactivate();
			_stepIdx++;
			steps[_stepIdx].Activate();
		}
	}

	public void RecedeSequence()
	{
		if (_stepIdx == 0) {
			return;
		} else {
			steps[_stepIdx].Deactivate();
			_stepIdx--;
			steps[_stepIdx].Activate();
		}
	}

	

	public void StartSequence()
	{
		if (steps.Length == 0) {
			_active = false;
			return;
		}
		steps[_stepIdx].Activate();
		_active = true;
	}

	public void PauseSequence()
	{
		steps[_stepIdx].Deactivate();
		_active = false;
	}

	public void FinishSequence()
	{
		steps[_stepIdx].Deactivate();
		_active = false;
		ResetSequence();
	}

	public void ResetSequence()
	{
		_stepIdx = 0;
	}

	public bool IsActive()
	{
		return _active;
	}
}
