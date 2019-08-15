using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class EnumResource : ScriptableObject
{
    public float Value
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
    protected float _value;

    public UnityEvent OnValueChanged;

}
