using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Selectable state action base class. Contains a public bool and references its own Selectable Element. 
/// </summary>
public class SelectableStateAction : MonoBehaviour, ISelectableStateAction 
{
    public SelectableElement element;

    public bool active;

    [SerializeField]
    public SelectableState State
    {
        get;

        set;
    }

    public virtual void Activate()
    {
        active = true;
    }

    public virtual void Deactivate()
    {
        active = false;
    }

    public virtual void Load()
    {
        element = GetComponentInParent<SelectableElement>();
    }

    /// <summary>
    /// Remove this instance. WILL DESTROY GAMEOBJECT.
    /// </summary>
    public virtual void Remove()
    {
#if UNITY_EDITOR
		//UnityEditor.Undo.RecordObject(this, "Deleted " + element.name);
		Debug.LogWarning("DESTROYING " + gameObject.name);
		//UnityEditor.Undo.DestroyObjectImmediate(gameObject);
        //DestroyImmediate(gameObject);
#else
        Debug.Log("Destroying " + gameObject.name);
		//Destroy(gameObject);
#endif

    }

}
