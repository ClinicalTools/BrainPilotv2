using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimator : MonoBehaviour
{

	public Animation ani;

	// Use this for initialization
	void Start()
	{
		if (ani == null) {
			ani = GetComponent<Animation>();
		}
	}

	public void Play()
	{
		ani.Play("ButtonScale");
	}

}
