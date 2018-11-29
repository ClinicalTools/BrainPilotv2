using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu]
public class ColorResource : ScriptableObject
{

    public UnityEvent onValueChanged;

    public Color Color {
        get { return color; }
        set {
            color = value;
            onValueChanged.Invoke();
        }

    }
    [SerializeField]
    protected Color color;

}
