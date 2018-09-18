using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Effect : ScriptableObject, IEffect
{
    [Tooltip("Replacement Material, can be null!")]
    public Material material;

    [Tooltip("Sub behaviors that will be loaded as children on listeners.")]
    public List<GameObject> behaviors;

    public List<EffectListener> listeners;

    public void Load()
    {

    }

    public void Play()
    {

    }

    public void RegisterListener(EffectListener effectListener)
    {
        
    }

    public void DeRegisterListener(EffectListener effectListener)
    {
        
    }

    public void Unload()
    {
        
    }

    public void LoadActions(EffectListener effectListener)
    {
        
    }

    public void UnloadActions(EffectListener effectListener)
    {
        
    }

    public void Stop()
    {
        
    }
}
