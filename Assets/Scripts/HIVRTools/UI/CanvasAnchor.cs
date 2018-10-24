using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAnchor : MonoBehaviour {

    CanvasLocationController controller;

    // Use this for initialization
    void Start ()
    {
        gameObject.tag = "CanvasAnchor";
        controller = FindObjectOfType<CanvasLocationController>();
        if (controller != null)
            controller.RegisterAnchor(transform);
	}

    private void OnDestroy()
    {
        controller?.UnregisterAnchor(transform);
    }

}
