using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EffectListener : MonoBehaviour, IEffectListener
{

    public UnityEvent loadEffect;
    public UnityEvent unloadEffect;
    public UnityEvent playEffect;
    public UnityEvent stopEffect;

    public void Load(Effect effect)
    {
        
    }

    public void Play(Effect effect)
    {
        
    }

    public void Stop(Effect effect)
    {

    }

    public void Unload(Effect effect)
    {

    }
}
