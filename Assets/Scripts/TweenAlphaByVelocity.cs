using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenAlphaByVelocity : MonoBehaviour {

	public Transform velocityTarget;

	public MeshRenderer alphaTarget;

	public float maxVelocity;

	public float maxAlpha;

	[SerializeField]
	private bool active;

	private Color mainColor;

	private Vector3 lastPosition;

	// Use this for initialization
	void Start () {
		lastPosition = velocityTarget.position;
		maxAlpha = Mathf.Clamp01(maxAlpha);
		mainColor = alphaTarget.sharedMaterial.color;
	}

	public void Enable()
	{
		active = true;
	}

	public void Disable()
	{
		active = false;
	}

	private Color alteredColor;
	// Update is called once per frame
	void Update () {
		if (active) {
			alteredColor = mainColor;
			alteredColor.a = (velocityTarget.position - lastPosition).magnitude / maxVelocity;
			alteredColor.a = Mathf.Clamp01(alteredColor.a) * maxAlpha;
			alphaTarget.sharedMaterial.color = alteredColor;
			lastPosition = velocityTarget.position;
		}
	}
}
