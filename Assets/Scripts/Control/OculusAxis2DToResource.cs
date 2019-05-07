using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusAxis2DToResource : MonoBehaviour
{

    public Vec2Resource resource;

    public OVRInput.Axis2D axis;

    public OVRInput.Controller controller;

    private void Update()
    {

        //OVRInput.Update();
		if (controller == OVRInput.GetActiveController()) {
			resource.Value = OVRInput.Get(axis, controller);
		}
        
    }


}
