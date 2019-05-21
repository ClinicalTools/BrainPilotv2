using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A UX / Movement Controller for casting out an anchor cursor and moving a player platform. 
/// When 'Anchored' in place will follow a line forwards or backwards from an anchor (samples a Line Renderer) and left right in an orbit around the cursor.
/// Input gets a 2d Axis (thumbstick or touch pad).
/// </summary>
public class AnchorUXController : MonoBehaviour {

	/*
	 * Pre-frame dependent values:
	 * forward: .1
	 * orbit: .5
	 * rotation: .4
	 * deadzone: .2
	 */

	public Vec2Resource inputResource;

    public bool isActive;

	public bool lockActive;

	public bool invertX;

	[Tooltip("Max units per second")]
	public float forwardSpeed = 5f;
    public float orbitSpeed = 5f;
	public float rotationSpeed = .8f;
	public float deadzoneRadius = .2f;

	public enum MovementType
	{
		Orbit,
		Rotate,
		Line
	}

	private MovementType _movementType;
	public MovementType movementType
	{// = MovementType.Orbit;
		get
		{
			return _movementType;
		}
		set
		{
			switch (value) {
				case MovementType.Orbit:
					invertX = false;
					break;
				case MovementType.Rotate:
					invertX = true;
					break;
				case MovementType.Line:
					invertX = false;
					break;
			}
			_movementType = value;
		}
	}

    /// <summary>
    /// The target we are using to move to / from and orbit around
    /// </summary>
    public Transform cursor;

    /// <summary>
    /// The platform that contains our player tracking area and camera
    /// </summary>
    public Transform platform;

    /// <summary>
    /// The Line Renderer of our caster, used to get the direction we want to move in or back from. 
    /// </summary>
    public LineRenderer line;

    public UnityEvent StartCustomMovement;
    public UnityEvent StopCustomMovement;

	[ContextMenu("TestDisable")]
	public void DisableInput()
	{
		SetActive(false);
		lockActive = true;
		GradientColorKey red = new GradientColorKey(Color.red, 0);
		GradientColorKey blue = new GradientColorKey(new Color(129, 141, 255), 1);
		GradientAlphaKey reda = new GradientAlphaKey(150f, 0);
		GradientAlphaKey bluea = new GradientAlphaKey(255f, 1);
		Gradient g = new Gradient();
		g.alphaKeys = new[] { reda, bluea };
		g.colorKeys = new[] { red, blue };
		line.colorGradient = g;
	}
	void Start()
	{
		SetInputMethod(2);
	}
	public void EnableInput()
	{
		lockActive = false;
		GradientColorKey white = new GradientColorKey(Color.white, 0);
		GradientAlphaKey whitea = new GradientAlphaKey(0f, 0);
		GradientColorKey blue = new GradientColorKey(new Color(129, 141, 255), 1);
		GradientAlphaKey bluea = new GradientAlphaKey(255f, 1);
		Gradient g = new Gradient();
		g.alphaKeys = new[] { whitea, bluea };
		g.colorKeys = new[] { white, blue };
		line.colorGradient = g;
	}

	/// <summary>
	/// Sets our active status (like when a forward trigger is down). Punts if status is our current status and nothing will happen. Otherwise fires appropriate events and enters an input subroutine. 
	/// </summary>
	/// <param name="status"></param>
	public void SetActive(bool status)
    {
		if (lockActive)
			return;

		if (isActive == status)
            return;

        isActive = status;
        if (isActive)
            StartGetInput();
        else
            StopGetInput();
    }

    /// <summary>
    /// Stops Subroutine collecting input and moving the platform.
    /// </summary>
    public void StopGetInput()
    {
        isActive = false;
        StopAllCoroutines();
        StopCustomMovement.Invoke();
    }

    /// <summary>
    /// Starts a Coroutine to get input and move the platform. 
    /// </summary>
    public void StartGetInput()
    {
        isActive = true;
        StopAllCoroutines();
        StartCoroutine(RunGetInput());
    }

	/// <summary>
	/// Adjust the method of movement
	/// </summary>
	/// <param name="idx">0 for Orbit, 1 for Rotate, 2 for Line</param>
	public void SetInputMethod(int idx)
	{
		movementType = (MovementType)idx;
	}

    IEnumerator RunGetInput()
    {
        StartCustomMovement.Invoke();
		float damper = 0;
		float dampDuration = .75f;
		while (isActive) {
			 
			if (inputResource.Value.sqrMagnitude < deadzoneRadius * deadzoneRadius) {
				damper = 0;
			} else if (damper < dampDuration) {
				damper += Time.deltaTime;
				damper = Mathf.Min(damper, dampDuration);
			}
			
			switch (movementType) {
				case MovementType.Orbit:
					DoForwardMovement(damper / dampDuration);
					DoOrbitAround(damper / dampDuration);
					break;
				case MovementType.Rotate:
					DoForwardMovement(damper / dampDuration);
					DoRotate(damper / dampDuration);
					break;
				case MovementType.Line:
					HandleLineMovement(damper / dampDuration);
					LineRotate(damper / dampDuration);
					Debug.Log("Line Movement methods called.");
					break;
			}
			yield return null;
        }
        StopCustomMovement.Invoke();
    }

	private void HandleLineMovement(float val = 1)
	{
		float changeAmount = val * forwardSpeed * inputResource.Value.y * inputResource.Value.y;
		changeAmount *= Time.deltaTime;
		changeAmount *= inputResource.Value.y > 0 ? 1 : -1;
		Vector3 direction = (line.GetPosition(1) - line.GetPosition(0)).normalized;
		platform.position += direction * changeAmount;
	}

	private void DoOrbitAround(float val = 1)
    {
		float changeRotation = val * orbitSpeed * inputResource.Value.x * (invertX ? 1 : -1);
		changeRotation *= Time.deltaTime;
		platform.RotateAround(cursor.position, Vector3.up, changeRotation);
    }

	private void DoRotate(float val = 1)
	{
		float changeRotation = val * rotationSpeed * inputResource.Value.x * (invertX ? 1 : -1);
		changeRotation *= Time.deltaTime;
		platform.Rotate(Vector3.up, changeRotation);
	}

    private void DoForwardMovement(float val = 1)
    {
        float changeAmount = val * forwardSpeed * inputResource.Value.y * inputResource.Value.y;
		changeAmount *= Time.deltaTime;
		changeAmount *= inputResource.Value.y > 0 ? 1 : -1;
        Vector3 direction = (line.GetPosition(1) - line.GetPosition(0)).normalized;
        platform.position += direction * changeAmount;

    }

	private void LineRotate(float val = 1)
	{
		float changeRotation = val * rotationSpeed;
		Debug.Log("changeRotation starts at: " + changeRotation);
		
		Quaternion facing = platform.rotation;
		float yfacing = facing.eulerAngles.y;
		Debug.Log("Y angle for Platform: " + yfacing);
		
		Vector3 Eulerfacing = new Vector3(0f, yfacing, 0f);
		Vector3 direction = (line.GetPosition(1) - line.GetPosition(0)).normalized;
		Vector3 Ydirection = new Vector3(0f, direction.y, 0f);
		Debug.Log("Y angle for Controller: " + direction.y);
		float measureAngle = Vector3.SignedAngle(Eulerfacing, Ydirection, Vector3.up);
		Debug.Log("Measured Angle: " + measureAngle);
		
		if (measureAngle < 0)
		{
			changeRotation *= -1*Time.deltaTime;
		} else {
			changeRotation *= Time.deltaTime;
		}
		if (measureAngle*measureAngle > 1)
		{
			platform.Rotate(Vector3.up, changeRotation);
		}
	}
}
