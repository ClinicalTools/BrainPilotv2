using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypointer : MonoBehaviour {
	
	[System.Serializable]
	public class PlatformInformation
	{
		public Vector3 waypointLocation;
		//public Transform waypointLocation;

		public Transform lookAtPoint;
		//public Vector3 lookAtDirection;

		public float scaleVal;
	}
	
	[SerializeField]
	private PlatformInformation platformInformation;
	
	#if UNITY_EDITOR
	[ContextMenu("Input Camera Position")]
	public void InputCameraPos()
	{
		Transform cam = UnityEditor.SceneView.lastActiveSceneView.camera.transform;
		if (cam == null) {
			Debug.LogWarning("Scene camera not found!");
			return;
		}
		platformInformation.waypointLocation = cam.position;
	}

	[ContextMenu("Clear Platform Info")]
	public void ClearPlatformInfo()
	{
		platformInformation.lookAtPoint = null;
		platformInformation.scaleVal = 0;
		platformInformation.waypointLocation = Vector3.zero;
	}
	#endif
	public void Activate()
	{
		HandlePlatformMovement();
	}	

	public void HandlePlatformMovement()
	{
		StartCoroutine(IterateMovement());
	}

		private IEnumerator IterateMovement()
	{
		float time = 0;
		float totalTime = 3f;
		while (time < totalTime) {
			/*LerpMovement();
			LerpRotation();
			LerpScale(); */
			time += Time.deltaTime;
			yield return null;
		}
	}

	public PlatformInformation GetPlatformInfo()
	{
		return platformInformation;
	}
}
