using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusAxis2DToResource : MonoBehaviour
{

    public Vec2Resource resource;

    public OVRInput.Axis2D axis;

    public OVRInput.Controller controller;

	bool update;

	public int debugLogRate = 15;
	private int debugCounter;

	private void Start()
	{
#if UNITY_EDITOR
		controller = OVRInput.Controller.RTouch;
#endif
	}

	public void SetController(OVRInput.Controller c)
	{
		controller = c;
	}

	public void SetAxis(OVRInput.Axis2D a)
	{
		axis = a;
	}

	public void CycleController()
	{
		switch (controller) {
			case OVRInput.Controller.RTouch:
				controller = OVRInput.Controller.RTrackedRemote;
				break;
			case OVRInput.Controller.RTrackedRemote:
				controller = OVRInput.Controller.Touch;
				break;
			case OVRInput.Controller.Touch:
				controller = OVRInput.Controller.RTouch;
				break;
		}

		Debug.Log("Switched to " + controller);
	}

	public void ToggleAxis()
	{
		switch(axis) {
			case OVRInput.Axis2D.PrimaryThumbstick:
				axis = OVRInput.Axis2D.SecondaryThumbstick;
				break;
			case OVRInput.Axis2D.SecondaryThumbstick:
				axis = OVRInput.Axis2D.PrimaryTouchpad;
				break;
			case OVRInput.Axis2D.PrimaryTouchpad:
				axis = OVRInput.Axis2D.PrimaryThumbstick;
				break;
		}
		Debug.Log("Switched to " + axis);
	}

	private void Update()
    {
		update = (controller == OVRInput.GetActiveController());
		/*if (!update) {
			OVRInput.Controller c = OVRInput.GetActiveController();
			if (c == OVRInput.Controller.Touch && controller == (OVRInput.Controller.RTouch | OVRInput.Controller.LTouch)) {
				controller = c;
				update = true;
			} else if (c == (OVRInput.Controller.RTouch | OVRInput.Controller.LTouch) && controller == OVRInput.Controller.Touch) {
				controller = c;
				update = true;
			}
		}*/
		OVRInput.Update();
		debugCounter++;
		if (debugCounter >= debugLogRate) {
			debugCounter = 0;
			/*Debug.Log(controller + "\n" +
				OVRInput.Get(axis, controller) + "\n" +
				OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, controller) + "\n" +
				OVRInput.GetConnectedControllers() + "\n" +
				OVRInput.GetActiveController() + "\n" +
				update);*/
			Debug.Log(controller + "\n" +
				GetDebugString(OVRInput.Axis2D.PrimaryTouchpad, OVRInput.Controller.RTouch) + "\n" +
				GetDebugString(OVRInput.Axis2D.SecondaryTouchpad, OVRInput.Controller.RTouch) + "\n" +
				GetDebugString(OVRInput.Axis2D.PrimaryTouchpad, OVRInput.Controller.RTrackedRemote) + "\n" +
				GetDebugString(OVRInput.Axis2D.SecondaryTouchpad, OVRInput.Controller.RTrackedRemote) + "\n" +
				GetDebugString(OVRInput.Axis2D.PrimaryTouchpad, OVRInput.Controller.Touch) + "\n" +
				GetDebugString(OVRInput.Axis2D.SecondaryTouchpad, OVRInput.Controller.Touch) + "\n" +
				"Active: " + OVRInput.Get(OVRInput.RawAxis2D.RThumbstick));
				
		}

		if (update) {
			resource.Value = OVRInput.Get(axis, controller);
		}
        
    }

	private string GetDebugString(OVRInput.Axis2D a, OVRInput.Controller c)
	{
		return c + "," + a + ":" + OVRInput.Get(a, c);
	}
}
