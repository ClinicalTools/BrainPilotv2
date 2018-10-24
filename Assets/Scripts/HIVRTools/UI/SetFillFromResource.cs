using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetFillFromResource : MonoBehaviour {

    public Resource resource;

    public Image image;

    private void Update()
    {
        image.fillAmount = resource.Ratio;
    }


}
