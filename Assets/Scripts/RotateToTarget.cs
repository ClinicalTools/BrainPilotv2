using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToTarget : MonoBehaviour {

	public Transform objToRotate;
	public Transform target;
	public float rotationTime = .4f;

	private Quaternion startingRotation;

	public void RotateTo(Transform rotTarget)
	{
		target = rotTarget;
		RotateTo();
	}

	public void RotateTo()
	{
		startingRotation = objToRotate.rotation;
		StartCoroutine(Rotate());
	}
	
	private IEnumerator Rotate()
	{
		bool rotDone = false;
		Vector3 pos;
		float lerpVal = 0;
		while (!rotDone) {
			lerpVal += Time.deltaTime / rotationTime;
			if (objToRotate == null || target == null) {
				yield break;
			}

			Quaternion lookAt = Quaternion.LookRotation(target.position - objToRotate.position, Vector3.up);

			//Could use Quaternion.Lerp
			objToRotate.rotation = Quaternion.Lerp(startingRotation, lookAt, lerpVal);
			//platform.rotation = Quaternion.RotateTowards(platform.rotation, Quaternion.LookRotation(info.lookAtPoint.position - pos, Vector3.up), deltaAngle);
			pos = objToRotate.rotation.eulerAngles;
			pos.x = 0;
			pos.z = 0;
			objToRotate.eulerAngles = pos;

			//if (objToRotate.rotation == lookAt) { //This is blender mode
			if (lerpVal >= 1) {
				rotDone = true;
			}
			yield return null;
		}
	}
}
