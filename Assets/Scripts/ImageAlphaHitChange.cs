using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAlphaHitChange : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Image i = GetComponent<Image>();
		i.alphaHitTestMinimumThreshold = .1f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
