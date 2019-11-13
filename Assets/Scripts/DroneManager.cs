using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

//ActiveSceneController
public class DroneManager : SubSceneListener {

#if UNITY_EDITOR
	[CustomEditor(typeof(DroneManager))]
	public class DroneManagerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var controller = (DroneManager)target;

			if (GUILayout.Button("Next sequence")) {
				controller.NextSequence();
			}
		}
	}
#endif


	public DroneController drone;
	private SequenceManager sequenceManager;
	private SignalManagerManager signalManager;
	private int activeScene => subSceneManager.activeScene;
	private bool started = false;
	public bool tutorial = true;

	// Use this for initialization
	void Start () {
		if (drone == null) {
			drone = GetComponentInChildren<DroneController>();
		}
		if (subSceneManager == null) {
			RetrieveSubSceneManager();
		}
		subSceneManager.RegisterListener(this);
		UnityEngine.Application.quitting += delegate { subSceneManager.UnregisterListener(this); };
		subSceneManager.activeScene = 1;
		//Invoke("GrabActiveSequence", 2f);
		Invoke("FindAllSequences", 2f);
	}

	/// <summary>
	/// Disables the drone
	/// </summary>
	public void HideDrone()
	{
		drone.SetActive(false);
	}

	/// <summary>
	/// Called by SubSceneManager when the active scene is set
	/// </summary>
	public override void Invoke()
	{
		if (subSceneManager == null) {
			print("I was null");
			RetrieveSubSceneManager();
		}
		sequenceManager = subSceneManager.sequenceManager;
		signalManager = subSceneManager.signalManager;
	}

	[System.Serializable]
	public class SceneManagers
	{
		public Sequence sequence;
		public SignalManagerManager signalManager;

		public SceneManagers(Sequence seq, SignalManagerManager sig)
		{
			sequence = seq; signalManager = sig;
		}
	}

	int activeSequenceIdx = 0;
	List<SceneManagers> managers = new List<SceneManagers>();
	public SceneManagers activeSceneManagers;
	/// <summary>
	/// Gets a reference to every available sequence
	/// Makes navigating amongst SequenceManagers easier
	/// </summary>
	public void FindAllSequences()
	{
		subSceneManager.activeScene = 0;
		activeSequenceIdx = 0;
		managers = new List<SceneManagers>();
		while (activeScene < SceneManager.sceneCount) {
			if (RetrieveNextSequence() != null) {
				managers.Add(new SceneManagers(sequenceManager.GetSequence(), signalManager));
			}
		}

		if (managers.Count == 0 || managers[0].sequence == null) {
			FindObjectOfType<SettingsManager>().transform.Find("Main Menu/Background/LessonsMenu").gameObject.SetActive(false);
		}

		//Move this to a separate start function? this should be only called once
		//Could change this depending on if we want signals to start without sequences
		if (managers.Count > 0 && managers[activeSequenceIdx].sequence.startOnLoad) {
			ActivateSequence(managers[activeSequenceIdx]);
		}
	}

	/// <summary>
	/// Helper method for FindAllSequences()
	/// Seeks out the next available Sequence
	/// </summary>
	/// <returns>The next available sequence, across scenes or not.</returns>
	private Sequence RetrieveNextSequence()
	{
		if (sequenceManager == null) {
			if (subSceneManager.activeScene >= SceneManager.sceneCount) {
				//No more scenes to check
				Debug.Log("Ran out of sequences");
			} else {
				//No sequence loaded yet
				GetSceneSequences1(activeScene + 1);
			}
			return sequenceManager?.GetSequence();
		}
		if (sequenceManager.isAtEnd) {
			//Move to the next scene
			while (sequenceManager == null || sequenceManager.isAtEnd) {
				if (subSceneManager.activeScene <= SceneManager.sceneCount) {
					if (GetSceneSequences1(activeScene + 1) && sequenceManager != null) {
						//Some sequence manager was loaded (This prevents errors with one step sequences)
						return sequenceManager.GetSequence();
					}
				} else {
					//Out of sequences
					Debug.Log("Ran out of sequences");
					return null;
				}
			}
			return sequenceManager?.GetSequence();
		} else {
			//Move to the next sequence in the same scene
			if (sequenceManager.GetSequence() != sequenceManager.AdvanceSequence()) {
				return sequenceManager.GetSequence();
			} else {
				//Ran out of sequences
				Debug.Log("Ran out of sequences");
				return null;
			}
		}
	}

	/// <summary>
	/// Get the sequences for the scene at scene index and updates activeScene index
	/// </summary>
	/// <param name="sceneIdx">The scene's index out of loaded scenes</param>
	/// <returns>True if valid scene index. False if no scene could be loaded.
	/// Will return true even if a scene has no sequences as long as that scene exists</returns>
	private bool GetSceneSequences1(int sceneIdx)
	{
		if (!started || sceneIdx != subSceneManager.activeScene) {
			if (!subSceneManager.UpdateSelectedScene(sceneIdx)) {
				//Could not load managers at sceneIdx
				started = true;
				return false;
			}
		}
		started = true;
		return true;
	}

	/// <summary>
	/// Gets the previous sequence
	/// </summary>
	/// <returns>The previous sequence if available. Null if not.</returns>
	public SceneManagers PreviousSequence()
	{
		if (activeSequenceIdx > 0) {
			activeSequenceIdx--;
		}
		ActivateSequence(managers[activeSequenceIdx]);

		if (activeSequenceIdx < managers.Count - 1) {
			return managers[activeSequenceIdx];
		}
		return null;
	}

	/// <summary>
	/// Gets the next sequence
	/// </summary>
	/// <returns>The next sequence if available. Null if not.</returns>
	public SceneManagers NextSequence()
	{
		if (activeSequenceIdx < managers.Count - 1) {
			activeSequenceIdx++;
		}
		ActivateSequence(managers[activeSequenceIdx]);

		if (activeSequenceIdx < managers.Count - 1) {
			return managers[activeSequenceIdx];
		}
		return null;
	}

	/// <summary>
	/// Gets a sequence at a certain index.
	/// Index increments consistantly across scenes as well
	/// </summary>
	/// <param name="i"></param>
	public void GetSequenceAt(int i)
	{
		if (i > 0 && i < managers.Count) {
			ActivateSequence(managers[i]);
		}
	}

	/// <summary>
	/// Activates the given managers
	/// </summary>
	/// <param name="m"></param>
	private void ActivateSequence(SceneManagers m)
	{
		activeSceneManagers = m;

		LoadLesson(m.sequence);

		//Update the signal manager if needed
		if (signalManager != m.signalManager) {
			signalManager?.StopAll();
			signalManager = m.signalManager;
			
			if (signalManager != null && signalManager.startOnAwake) {
				LoadSignals(signalManager);
			}
		}
	}

	/// <summary>
	/// Loads the given sequence into the drone 
	/// </summary>
	/// <returns>True if a sequence could be loaded</returns>
	private void LoadLesson(Sequence sequence)
	{
		drone.ResumeSequence(sequence);
		if (!tutorial) {
			GameObject.FindObjectOfType<AnchorUXController>().DisableInput();
		} else {
			GameObject.FindObjectOfType<AnchorUXController>().EnableInput();
			tutorial = false;
		}
	}

	/// <summary>
	/// Loads and activates the signals of the active sequence
	/// </summary>
	private void LoadSignals(SignalManagerManager signals)
	{
		signals.PlayAll(false);
	}
}
