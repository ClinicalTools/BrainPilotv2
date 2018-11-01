using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class Vec3Resource : ScriptableObject
{
    public Vector3 Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            OnValueChanged.Invoke();
        }
    }

    [SerializeField]
    protected Vector3 _value;

    public UnityEvent OnValueChanged;

}