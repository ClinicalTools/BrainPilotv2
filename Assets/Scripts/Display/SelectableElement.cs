using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Selectable Element indexes common dependencies. 
/// </summary>
[RequireComponent(typeof(SelectableStateController))]
[RequireComponent(typeof(SelectableListener))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class SelectableElement : MonoBehaviour 
{

    public Selectable selectable;

    public SelectableStateController stateController;
    public SelectableListener listener;
    public MeshRenderer meshRenderer;

    private void Awake()
    {
        stateController = GetComponent<SelectableStateController>();
        listener = GetComponent<SelectableListener>();
        meshRenderer = GetComponent<MeshRenderer>();

        listener.selectable = selectable;
    }

}
