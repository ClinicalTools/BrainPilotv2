﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Selection : ScriptableObject, ISelection {

    [Tooltip("The Base Selectables which will be loaded in Initialize()")]
    public List<Selectable> baseSelectables;

    [Tooltip("The active list of selectables to be called on DoSelect() or DoDeselect().")]
    public List<Selectable> registeredSelectables;

    [SerializeField]
    private bool _active;

    [SerializeField]
    public bool Active
    {
        set
        {
            if (value == true && _active != true)
            {
                Select();
            }
            if (value == false && _active != false)
            {
                Deselect();
                
            }
            _active = value;
        }
        get
        {
            return _active;
        }
    }

    [ContextMenu("Initialize")]
    public void Initialize()
    {
        if (registeredSelectables == null)
            registeredSelectables = new List<Selectable>();

        if (baseSelectables != null)
            LoadBaseSelectables();

        Active = false;
    }

    private void LoadBaseSelectables()
    {
        if (registeredSelectables == null)
            registeredSelectables = new List<Selectable>();

        registeredSelectables.AddRange(baseSelectables);


    }

    public void RegisterSelectable(Selectable selection)
    {
        if (registeredSelectables == null)
            Initialize();

        if(!registeredSelectables.Contains(selection))
        {
            registeredSelectables.Add(selection);
        }
    }

    public void DeregisterSelectable(Selectable selection)
    {
        if (registeredSelectables == null)
        {
            Initialize();
            return;
        }
           
        if (registeredSelectables.Contains(selection))
        {
            registeredSelectables.Remove(selection);
        }


    }

    [ContextMenu("DoSelect")]
    public void Select()
    {
        for (int i = registeredSelectables.Count - 1; i >= 0; i--)
        {
            registeredSelectables[i].OnSelect(this);
        }

        _active = true;
    }

    public List<Selectable> GetCurrentSelection()
    {
        if (!Active)
            return null;
        else
        {
            return registeredSelectables;
        }
    }

    [ContextMenu("DoDeselect")]
    public void Deselect()
    {
        for (int i = registeredSelectables.Count - 1; i >= 0; i--)
        {
            registeredSelectables[i].OnDeselect(this);
        }

        _active = false;
    }

    public void SetSelect(Selection selection)
    {
        if (this == selection && !Active)
            Select();
    }

    public bool IsSelected(Selection selection)
    {
        return (this == selection);
    }
}
