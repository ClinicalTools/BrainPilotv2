using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class LimitedGameplayEvent : GameplayEvent
{

    public int runCount;
    public int startCount;

    public override void Initialize()
    {
        base.Initialize();
        runCount = startCount;
    }

    public override void RaiseEvent()
    {
        if (runCount-- > 0)
            base.RaiseEvent();
    }


}
