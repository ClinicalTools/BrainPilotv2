using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusAxis2DToResource : MonoBehaviour
{

    public Vec2Resource resource;

    public OVRInput.Axis2D axis;

    public OVRInput.Controller controller;

	bool update;

    private void Update()
    {
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

		//OVRInput.Update();
		if (update) {
			resource.Value = OVRInput.Get(axis, controller);
		}
        
    }


}
