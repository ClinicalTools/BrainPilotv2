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

    [SerializeField]
    private Transform platform;

    private Vector3 platformLoc;
    private Vector3 platformScale;
    private Quaternion platformRotQ;

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
        platformLoc = platform.position;
        platformScale = platform.localScale;
        platformRotQ = platform.rotation;

		//platform.GetComponent<TweenScaleByFactor>().TweenToScale(platformInformation.scaleVal, totalTime);
		while (time < totalTime) {
            LerpMovement(time / totalTime);
			LerpRotation(time / totalTime);
			//Scale needs to be handled by TweenScaleByFactor on the platform
			//Do we even need to scale things?
			//LerpScale(time / totalTime);
			time += Time.deltaTime;
			yield return null;
		}
	}

    private void LerpMovement(float val)
    {
        if (platformInformation.waypointLocation != Vector3.zero)
        {
            platform.position = Vector3.Lerp(platformLoc, platformInformation.waypointLocation, val);
        }
    }

    private void LerpRotation(float val)
    {
        if (platformInformation.lookAtPoint != null)
        {
            Vector3 pos = platform.position;
            if (platformInformation.waypointLocation != Vector3.zero)
            {
                pos = platformInformation.waypointLocation;
            }
            Quaternion lookAt = Quaternion.LookRotation(platformInformation.lookAtPoint.position - pos, Vector3.up);

            //Could use Quaternion.Lerp
            platform.rotation = Quaternion.Lerp(platformRotQ, lookAt, val);
            //platform.rotation = Quaternion.RotateTowards(platform.rotation, Quaternion.LookRotation(info.lookAtPoint.position - pos, Vector3.up), deltaAngle);
            pos = platform.rotation.eulerAngles;
            pos.x = 0;
            pos.z = 0;
            platform.eulerAngles = pos;
        }
    }

    private void LerpScale(float val)
    {
        if (platformInformation.scaleVal > 0)
        {
            platform.localScale = Vector3.Lerp(platformScale, Vector3.one * platformInformation.scaleVal, val);
        }
    }

    public PlatformInformation GetPlatformInfo()
	{
		return platformInformation;
	}
}
