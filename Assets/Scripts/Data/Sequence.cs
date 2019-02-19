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
	private int _stepIdx;

	public SequenceElement GetActiveStep()
	{
		return steps[_stepIdx];
	}

	public void AdvanceSequence()
	{
		if (_stepIdx == steps.Length - 1) {
			FinishSequence();
		} else {
			steps[_stepIdx].Deactivate();
			_stepIdx++;
			steps[_stepIdx].Activate();
		}
	}

	public void StartSequence()
	{
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
		_stepIdx = 0;
	}

	public bool IsActive()
	{
		return _active;
	}
}
