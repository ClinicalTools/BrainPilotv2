using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class GameplayEventManager : MonoBehaviour
{

    public List<GameplayEvent> events;

    private void Start()
    {
        InitializeAll();
    }

    [ContextMenu("Initialize All")]
    public void InitializeAll()
    {
        if (events == null)
            return;

        foreach (var gameEvent in events)
        {
            gameEvent.Initialize();
        }
    }
#if UNITY_EDITOR
    [ContextMenu("Build Complete List from Assets")]
    private void BuildCompleteEventList()
    {
        events = new List<GameplayEvent>();
        foreach (var gameEvent in Resources.FindObjectsOfTypeAll<GameplayEvent>())
        {
            if (!events.Contains(gameEvent))
            {
                events.Add(gameEvent);
            }
        }
    }
#endif
}
