﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Selectable state controller, tracks what states are loaded locally and matches those to what we find in the attached Selectable asset. Relies on the
/// State Action Manager to actually index and toggle the individual actions according to what states are active. 
/// </summary>
[RequireComponent(typeof(StateActionManager))]
[ExecuteInEditMode]
public class SelectableStateController : MonoBehaviour 
{

    public SelectableElement element;
    public StateActionManager manager;
    public bool updateStates;

    public List<SelectableState> localLoadedStates;
    public List<SelectableState> localActiveStates;

	void Start () 
    {
        element = GetComponent<SelectableElement>();
        manager = GetComponent<StateActionManager>();

        element.listener.selectableUpdated.AddListener(UpdateNextFrame);

        updateStates = true;
	}

    /// <summary>
    /// When called, will run updates in the next Update()
    /// </summary>
    void UpdateNextFrame()
    {
        updateStates = true;
    }

    private void Update()
    {
        if (updateStates)
        {
            UpdateLoadedStates();
            UpdateActiveStates();

            updateStates = false;
        }
    }

    /// <summary>
    /// Updates the active states.
    /// </summary>
    private void UpdateActiveStates()
    {
        var activeStates = element.selectable.GetActiveStates();

        var statesToDeactivate = localActiveStates?.FindAll(state => !activeStates.Contains(state));

        var statesToActivate = activeStates?.FindAll(state => !localActiveStates.Contains(state));

        if (statesToDeactivate != null)
        {
            foreach (var state in statesToDeactivate)
            {
                manager.DeactivateState(state);
            }
        }

        if (statesToActivate != null)
        {
            foreach (var state in statesToActivate)
            {
                manager.ActivateState(state);
            }
        }
    }

    /// <summary>
    /// Updates the loaded states.
    /// </summary>
    private void UpdateLoadedStates()
    {
        var loadedStates = element.selectable.GetLoadedStates();

        var statesToUnload = localLoadedStates?.FindAll(state => !loadedStates.Contains(state));

        var statesToLoad = loadedStates?.FindAll(state => !localLoadedStates.Contains(state));

        if (statesToUnload != null)
        {
            foreach (var state in statesToUnload)
            {
                manager.UnloadState(state);
            }
        }

        if (statesToLoad != null)
        {
            foreach (var state in statesToLoad)
            {
                manager.LoadState(state);
            }
        }
    }
}
