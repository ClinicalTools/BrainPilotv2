using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleUpdater : MonoBehaviour
{
	public Transform player;

	//The bigger the startSize, the smaller it is. 1 = default size
	[Tooltip("Bigger the startSize, the smaller it is. 1 = default")]
	public float startSize = 15;

	//cursor decreases at 1/(sizeDecreaseRate * x)
	[Tooltip("Decreses object at 1/(sizeDecreaseRate * x)")]
	public float sizeDecreaseRate = 3;

	//Controls the furthest point where the object will no longer increase in size
	public float maxDistance = 10f;

	private void Awake()
	{
		if (player == null) {
			player = Camera.main.transform;
		}
	}

	private void Update()
	{
		UpdateScale();
	}

	private void UpdateScale()
	{
		float distance = (transform.position - player.position).magnitude;
		//Multiply distance by ArcTan(x), where x is default size of the cursor we want.
		float y = distance * Mathf.Atan(2);

		//The adjustment is used to slightly alter the scale of the cursor based on distance.
		float adjustment = 1 / (sizeDecreaseRate * (distance / maxDistance) + startSize);
		y *= adjustment;

		transform.localScale = Vector3.one * y;
	}
}
