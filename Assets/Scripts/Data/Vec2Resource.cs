using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class Vec2Resource : ScriptableObject
{
    public Vector2 Value
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
    protected Vector2 _value;

    public UnityEvent OnValueChanged;

}
