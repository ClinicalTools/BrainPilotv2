using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScreenController : MonoBehaviour
{
	bool active;
	public Transform mainMenu;

	public bool useAnimation;

	public float speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
		active = Time.time < 5;
		if (!active) {
			//Just disable objects since we don't want to play the animation
			mainMenu.gameObject.SetActive(true);
			gameObject.SetActive(false);
			mainMenu.GetComponent<CanvasGroup>().interactable = true;
		}
    }

    // Update is called once per frame
    void Update()
    {
        if (active) {
			if (OVRInput.Get(OVRInput.Button.Any)) {
				StartCoroutine(Disable());
			}
		}
    }

	IEnumerator Disable()
	{
		CanvasGroup menu = mainMenu.GetComponent<CanvasGroup>();
		menu.alpha = 0;
		menu.interactable = false;
		mainMenu.gameObject.SetActive(true);
		CanvasGroup intro = GetComponent<CanvasGroup>();
		while (menu.alpha < 1) {
			menu.alpha += Time.deltaTime * speed;
			intro.alpha = 1 - menu.alpha;
			yield return null;
		}

		menu.alpha = 1;
		menu.interactable = true;
		intro.gameObject.SetActive(false);
	}
}
