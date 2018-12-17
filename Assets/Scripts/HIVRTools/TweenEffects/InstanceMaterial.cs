using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceMaterial : MonoBehaviour {

    public Material instance;

    [SerializeField]
    protected Material material;

    private void OnEnable()
    {
        material = GetComponent<MeshRenderer>().material;

        instance = new Material(material);
        GetComponent<MeshRenderer>().material = instance;
    }

    private void OnDisable()
    {
        GetComponent<MeshRenderer>().material = material;
    }
}
