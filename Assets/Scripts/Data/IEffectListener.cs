using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectListener
{

    void Load(Effect effect);

    void Unload(Effect effect);

    void Play(Effect effect);

    void Stop(Effect effect);

}
