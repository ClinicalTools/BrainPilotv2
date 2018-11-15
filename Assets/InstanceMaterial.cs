using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple Utility Class to create an instanced Material so that effects can take place on only one object. Downside is this will add to draw calls since Unity cannot batch them. 
/// </summary>
public class InstanceMaterial : MonoBehaviour {

    //private void Awake()
    //{
    //    var renderer = GetComponent<MeshRenderer>();
    //    var material = new Material(renderer.material);

    //    renderer.material = material;
    //}

}
