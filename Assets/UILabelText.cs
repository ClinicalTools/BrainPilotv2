using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UILabelText : MonoBehaviour
{

    public TextMeshPro textMeshPro;

    public bool useOverrideText;

    public string overrideText;

    private void OnEnable()
    {
        var element = GetComponent<UISelectableObject>().uiElement;

        textMeshPro = textMeshPro ?? GetComponentInChildren<TextMeshPro>();
        if (useOverrideText)
            textMeshPro.text = overrideText;
        else
            textMeshPro.text = element.label;
    }

}
