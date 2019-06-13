using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusInputToFloatResource : MonoBehaviour
{

    public FloatResource resource;

    public OVRInput.Controller controller;
    public OVRInput.Axis1D axis;
	bool update;

	private void Start()
	{
#if UNITY_EDITOR
		controller = OVRInput.Controller.RTouch;
#endif
	}

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
		//Debug.Log(controller + "\n" + OVRInput.Get(axis, controller));

		//OVRInput.Update();
		if (update) {
			resource.Value = OVRInput.Get(axis, controller);
		}
    }

}
