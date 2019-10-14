using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveHandAnchor : MonoBehaviour
{
	[SerializeField]
	public Transform rightHandAnchor, leftHandAnchor;
	[SerializeField]
	private GameObject rightHandController, leftHandController;

	public OVRInput.Controller defaultController = OVRInput.Controller.RTouch;

	public static OVRInput.Controller active;
	//For debugging purposes
	public OVRInput.Controller staticActive;

	private OVRInput.Controller OppositeController(OVRInput.Controller c)
	{
		switch (c) {
			case OVRInput.Controller.RTouch:
				return OVRInput.Controller.LTouch;
			case OVRInput.Controller.LTouch:
				return OVRInput.Controller.RTouch;
			case OVRInput.Controller.RTrackedRemote:
				return OVRInput.Controller.LTrackedRemote;
			case OVRInput.Controller.LTrackedRemote:
				return OVRInput.Controller.RTrackedRemote;
			default:
				throw new System.Exception("No opposite controller");
		}
	}
	public bool debug = false;
	void CheckLastInput()
	{
		//If no input by not-default controller, set default as active
		//default input --> default
		//Not default input --> not default
		//No input --> default

		//If no default input, set other as active
		if (debug) {
			OVRInput.Get(OVRInput.Button.Any, defaultController);
			OVRInput.Get(OVRInput.Button.Any, OppositeController(defaultController));
		}

		try {
			if (OVRInput.Get(OVRInput.Button.Any, OppositeController(active))) {
				active = OppositeController(active);
			}
		} catch (System.Exception) {
			//No controller set yet
			if (!OVRInput.Get(OVRInput.Button.Any, OppositeController(defaultController))) {
				active = defaultController;
			} else {
				active = OppositeController(defaultController);
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		CheckLastInput();
		//For debugging
		staticActive = active;

		// If right hand is active
		if (active == OVRInput.Controller.RTouch) {
			transform.position = rightHandAnchor.position;
			transform.rotation = rightHandAnchor.rotation;

			if (!rightHandController.activeSelf)
				rightHandController.SetActive(true);
			if (leftHandController.activeSelf)
				leftHandController.SetActive(false);
		}
		// If left hand is active
		else if (active == OVRInput.Controller.LTouch) {
			transform.position = leftHandAnchor.position;
			transform.rotation = leftHandAnchor.rotation;

			if (!leftHandController.activeSelf)
				leftHandController.SetActive(true);
			if (rightHandController.activeSelf)
				rightHandController.SetActive(false);
		}

	}
}