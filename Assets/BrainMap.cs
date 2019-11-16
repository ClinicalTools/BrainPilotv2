using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainMap : MonoBehaviour
{
	public Transform centerPos;

	public Transform player;

	//The red dot representing the player
	public Transform playerDot;

	public Transform playerDirection;

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
			playerDot.gameObject.SetActive(true);
			playerDot.localPosition = difference;
		} else {
			playerDot.gameObject.SetActive(false);
		}

		if (playerDirection != null)
			playerDirection.localRotation = player.localRotation;
	}
}
