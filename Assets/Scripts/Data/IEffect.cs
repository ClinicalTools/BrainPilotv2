using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffect
{

    void RegisterListener(EffectListener effectListener);

    void DeRegisterListener(EffectListener effectListener);

    void Load();

    void Unload();

    void LoadActions(EffectListener effectListener);

    void UnloadActions(EffectListener effectListener);

    void Play();

    void Stop();


}
