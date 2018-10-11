using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ResetObjects : MonoBehaviour
{

    public List<IResettable> resettables;
    
    [ContextMenu("Reset All")]
    public void ResetAll()
    {
        if (resettables == null)
            BuildResetList();
        foreach(var reset in resettables)
        {
            reset.Reset();
        }
    }

    [ContextMenu("BuildResetList")]
    public void BuildResetList()
    {
        resettables = new List<IResettable>();
        var r = Resources.FindObjectsOfTypeAll<ScriptableObject>().OfType<IResettable>();
        foreach(var resetable in r)
        {
            resettables.Add(resetable);
            Debug.Log("adding to list " + resettables.Count);
        }
    }

}
