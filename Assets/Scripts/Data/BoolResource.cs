using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class BoolResource : ScriptableObject
{
    public bool Value
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
    protected bool _value;

    public UnityEvent OnValueChanged;

}
