using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectionNameDisplay : MonoBehaviour
{

    public TextMeshPro textMesh;
    public string defaultString = "---";

    private void Start()
    {
        UpdateName(null);
    }

    public void UpdateName(Selectable selectable)
    {
        string newName = selectable?.name;
        newName = newName ?? defaultString;

        textMesh.text = newName;
    }

}
