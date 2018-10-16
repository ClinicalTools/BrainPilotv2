using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class IntResource : ScriptableObject
{
    public int Value
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
    protected int _value;

    public UnityEvent OnValueChanged;

}
