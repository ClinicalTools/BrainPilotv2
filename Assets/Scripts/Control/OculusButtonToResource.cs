using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusButtonToResource : MonoBehaviour
{
    public BoolResource resource;

    public OVRInput.Controller controller;
    public OVRInput.Button button;

    private void Update()
    {
        resource.Value = OVRInput.GetDown(button);
    }
}