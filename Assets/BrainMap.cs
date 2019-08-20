using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainMap : MonoBehaviour
{
	public Transform centerPos;

	//The red dot representing the player
	public Transform playerPos;

	public float radius;

	public float scale;

	Vector3 difference;

	// Update is called once per frame
	void Update()
    {
		//Get the distance from the brain's center
		difference = transform.position - centerPos.position;

		//Translate the difference to match the map's size
		//Simple scale multiplication for now
		difference *= scale;

		if (difference.magnitude < radius) {
			playerPos.gameObject.SetActive(true);
			playerPos.localPosition = difference;
		} else {
			playerPos.gameObject.SetActive(false);
		}
	}
}
