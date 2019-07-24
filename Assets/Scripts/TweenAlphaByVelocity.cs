using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TweenAlphaByVelocity : MonoBehaviour {

	public Transform velocityTarget;

	public MeshRenderer alphaTarget;

	[Tooltip("The speed per frame required for max alpha")]
	public float maxVelocity;

	public float maxAlpha;

	public float scale;

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
	private float perFrameSpeed;
	float[] factors = new float[] { .4f, .3f, .2f, .1f };
	List<float> fl = new List<float>(4);
	private void AddToList(float a)
	{
		if (fl.Count >= fl.Capacity) {
			fl.RemoveAt(fl.Capacity - 1);
		}
		fl.Insert(0, a);
	}

	private float FrameWeightedAverage(float[] a1)
	{
		float sum = 0;
		for (int i = 0; i < a1.Length; i++) {
			sum += a1[i] * factors[i];
		}
		return sum;
	}

	// Update is called once per frame
	void Update () {
		if (active) {
			alteredColor = mainColor;

			//Find the speed, add it to the list
			perFrameSpeed = (velocityTarget.position - lastPosition).magnitude;
			AddToList(perFrameSpeed);

			//Calculate the alpha for this frame
			alteredColor.a =
				FrameWeightedAverage(fl.ToArray())
				/
				(maxVelocity * scale * Time.deltaTime);
			alteredColor.a = Mathf.Clamp01(alteredColor.a) * maxAlpha;

			//Update values
			alphaTarget.sharedMaterial.color = alteredColor;
			lastPosition = velocityTarget.position;
		}
	}
}
