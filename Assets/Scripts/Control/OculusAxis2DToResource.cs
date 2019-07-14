using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusAxis2DToResource : MonoBehaviour
{

    public Vec2Resource resource;

    public OVRInput.Axis2D axis;

    public OVRInput.Controller controller;

	bool update;

	private void Awake()
	{
		//Quest and Rift S use Touch
		controller = OVRInput.Controller.Touch;
	}

	private void Update()
    {
		/**
		 * Accessed as Touch, all inputs are separated
		 * Accessed individually (r or ltouch), they are mapped as the same
		 * We want them accessed the same way, but to call them indipendently.
		 */

		//Need to account for one controller or both active
		//r/ltouch and touch
		//Which is active while touch is the controller type?
		//Do we need the input check? Yes, because we 

		//The active controller is decided in ActiveHandAnchor
		resource.Value = OVRInput.Get(axis, ActiveHandAnchor.active);

		/*
		controller = OVRInput.GetActiveController();
		update = (controller == OVRInput.GetActiveController());
		
		if (!update) {
			OVRInput.Controller c = OVRInput.GetActiveController();
			if (c == OVRInput.Controller.Touch && controller == (OVRInput.Controller.RTouch | OVRInput.Controller.LTouch)) {
				controller = c;
				update = true;
			} else if (c == (OVRInput.Controller.RTouch | OVRInput.Controller.LTouch) && controller == OVRInput.Controller.Touch) {
				controller = c;
				update = true;
			}
		}

		if (update) {
			//Doing this, the left controller is marked as primary, so the right doesn't work.
			//Need to check the value of multiple controllers then most likely
			resource.Value = OVRInput.Get(axis);
		} else {
			print(OVRInput.GetActiveController());
		}*/


	}


}
