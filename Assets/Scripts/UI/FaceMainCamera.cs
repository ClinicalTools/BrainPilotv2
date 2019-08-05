using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceMainCamera : MonoBehaviour {

    public bool runInUpdate = true;
	public Transform player;

    private void Start()
    {
		player = Camera.main.transform;
        transform.LookAt(player);
    }

    private void Update()
    {
        if (runInUpdate)
            transform.LookAt(player);
    }

}
