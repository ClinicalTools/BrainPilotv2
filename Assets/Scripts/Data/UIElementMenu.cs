using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UIElementMenu : ScriptableObject {

    public List<UIElement> elements;

    protected bool _savedStatus = false;

    public void ToggleVisible(bool status) 
    {
        elements.ForEach(element => {
            element.visibleEvent.Invoke(status);
        });
        _savedStatus = status;
    }

    public void ToggleVisible(){
        ToggleVisible(!_savedStatus);
    }

}
