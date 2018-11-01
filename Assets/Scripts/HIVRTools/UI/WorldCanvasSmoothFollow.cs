
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldCanvasSmoothFollow : MonoBehaviour
{

    public Transform target;

    public Camera mainCamera;

    public RectTransform mainPanel;

    [Range(1f, 10f)]
    public float followSpeed;

	// Use this for initialization
	void Start () {
        mainCamera = Camera.main;
        if (mainPanel != null)
            mainPanel.Rotate(0, 180, 0);
        Vector3 pos = transform.position;
        transform.SetParent(null, false);
        transform.position = pos;
	}

    private void LateUpdate()
    {
        if (target == null)
            transform.position = new Vector3(0, 20f, 0);
        else
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
        transform.LookAt(mainCamera.transform);
    }


}
