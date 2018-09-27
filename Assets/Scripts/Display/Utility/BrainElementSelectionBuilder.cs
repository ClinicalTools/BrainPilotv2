using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BrainElementSelectionBuilder : MonoBehaviour 
{

    public const string selectableAssetPath = "Assets/Data/BrainElements/";

    [ContextMenu("Build Brain Element Selectable")]
    public void Build()
    {
        var newSelectable = ScriptableObject.CreateInstance<BrainElement>();

        AssetDatabase.CreateAsset(newSelectable, selectableAssetPath + transform.name + ".asset");
        AssetDatabase.SaveAssets();

        var element = gameObject.AddComponent<SelectableElement>();
        element.selectable = newSelectable;


    }


}
