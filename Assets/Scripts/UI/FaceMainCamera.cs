using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceMainCamera : MonoBehaviour {

    bool runInUpdate = true;

    private void Start()
    {
        transform.LookAt(Camera.main.transform);
    }

    private void Update()
    {
        if (runInUpdate)
            transform.LookAt(Camera.main.transform);
    }

}
