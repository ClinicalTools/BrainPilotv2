using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class FloadValueChangedEvent : UnityEvent<float> { }

[CreateAssetMenu]
public class Resource : ScriptableObject
{

    public FloadValueChangedEvent onValueChanged;

    public float Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            onValueChanged.Invoke(_value);
        }
    }
    private float _value;

    public float maxValue;

    public float Ratio
    {
        get
        {
            return _value / maxValue;
        }
    }

}
