using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// UILabelText sets a textmeshpro text to either the label from a UI Element data store, or to an override provided in the inspector.
/// </summary>
public class UILabelText : MonoBehaviour
{

    public TextMeshPro textMeshPro;

    [Tooltip("If checked, will use the text from the inspector. If not checked, this component will fetch the text from it's parent UI Element data")]
    public bool useOverrideText;

    [TextArea]
    public string overrideText;

    public UIElement uIElement;

    private void OnEnable()
    {
        var element = uIElement ?? GetComponent<UISelectableObject>()?.uiElement ?? GetComponent<UISelectableElement>()?.uiSelectable;

        textMeshPro = textMeshPro ?? GetComponentInChildren<TextMeshPro>();
        if (useOverrideText)
            textMeshPro.text = overrideText;
        else
            textMeshPro.text = element.label;
    }

}
