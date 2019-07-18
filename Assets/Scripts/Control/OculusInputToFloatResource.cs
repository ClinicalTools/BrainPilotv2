using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusInputToFloatResource : MonoBehaviour
{

    public FloatResource resource;

    public OVRInput.Controller controller;
    public OVRInput.Axis1D axis;

    private void Update()
    {
		resource.Value = OVRInput.Get(axis, ActiveHandAnchor.active);
		//OVRInput.Update();
		/*if (controller == OVRInput.GetActiveController()) {
			resource.Value = OVRInput.Get(axis, controller);
		}*/
	}

}
