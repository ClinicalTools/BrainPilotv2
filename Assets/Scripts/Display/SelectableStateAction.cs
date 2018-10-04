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
        DestroyImmediate(gameObject);
#else
        Destroy(gameObject);
#endif

    }

}
