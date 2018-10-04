using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class SelectableListener : MonoBehaviour, ISelectableListener
{

    public Selectable selectable;

    public UnityEvent selectableUpdated;

    private void OnEnable()
    {
        selectable?.RegisterListener(this);
    }

    private void OnDisable()
    {
        selectable?.UnregisterListener(this);
    }



    public void SelectableUpdated()
    {
        selectableUpdated.Invoke();
        
    }
}
