using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Key
{
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    Fire,
    Interact,
    _Count
}

public delegate void ButtonEvent(InputSource sender, ButtonEventArgs args);

public class InputSource
{
    private Gamepad _gamepad;

    public Gamepad Gamepad
    {
        get { return _gamepad; }
        set { _gamepad = value; }
    }


    Dictionary<Key, List<ButtonEvent>> _pressEvents =  new Dictionary<Key, List<ButtonEvent>>();
    Dictionary<Key, List<ButtonEvent>> _releaseEvents = new Dictionary<Key, List<ButtonEvent>>();
    Dictionary<Key, List<ButtonEvent>> _heldEvents = new Dictionary<Key, List<ButtonEvent>>();

    public InputSource(KeyBinding bindings)
    {
        if (bindings == null)
            throw new Exception("attempting to create inputsource with null bindings");

        //create a new game object to hold the gamepad and mark it do not destroy
        var gameObject = new GameObject("Gamepad");
        GameObject.DontDestroyOnLoad(gameObject);
        //add the gamepad and assign the keybindings
        var gamepad = gameObject.AddComponent<Gamepad>();
        gamepad.SetBoundKeys(bindings.BoundKeys);
        //add listeners to the gamepads events
        gamepad.ButtonPressed += Gamepad_ButtonPressed;
        gamepad.ButtonHeld += Gamepad_ButtonHeld;
        gamepad.ButtonReleased += Gamepad_ButtonReleased;
    }

    private void Gamepad_ButtonReleased(object sender, ButtonStateChangeEventArgs args)
    {
        TryInvokeEvents(args.Key, _releaseEvents);
    }

    private void Gamepad_ButtonHeld(object sender, ButtonStateChangeEventArgs args)
    {
        TryInvokeEvents(args.Key, _heldEvents);
    }

    private void Gamepad_ButtonPressed(object sender, ButtonStateChangeEventArgs args)
    {
        TryInvokeEvents(args.Key, _pressEvents);
    }

    private void TryInvokeEvents(Key key, Dictionary<Key, List<ButtonEvent>> eventsDictionary)
    {
        List<ButtonEvent> events = null;
        if (eventsDictionary.TryGetValue(key, out events) && events != null)
        {
            //copy the events to avoid issues with editing the list while invoking the events
            ButtonEvent[] clonedEvents = new ButtonEvent[events.Count];
            events.CopyTo(clonedEvents);
            foreach (var call in clonedEvents)
                call(this, new ButtonEventArgs(key));
        }

    }

    public void RegisterButtonPress(Key key, ButtonEvent evt)
    {
        AddEvent(_pressEvents, key, evt);
    }

    public void RegisterButtonRelease(Key key, ButtonEvent evt)
    {
        AddEvent(_releaseEvents, key, evt);
    }

    public void RegisterButtonHold(Key key, ButtonEvent evt)
    {
        AddEvent(_heldEvents, key, evt);
    }

    public void UnRegisterButtonPress(Key key, ButtonEvent evt)
    {
        AddEvent(_pressEvents, key, evt);
    }

    public void UnRegisterButtonRelease(Key key, ButtonEvent evt)
    {
        AddEvent(_releaseEvents, key, evt);
    }

    public void UnRegisterButtonHold(Key key, ButtonEvent evt)
    {
        AddEvent(_heldEvents, key, evt);
    }

    #region Generic Add Remove
    /// <summary>
    /// Adds the event into the dictionary and creates a new list at the specified key if needed
    /// </summary>
    private void AddEvent(Dictionary<Key, List<ButtonEvent>> dictionary, Key key, ButtonEvent evt)
    {
        List<ButtonEvent> eventList;
        if (dictionary.TryGetValue(key, out eventList))
            eventList.Add(evt);
        else
        {
            eventList = new List<ButtonEvent>();
            eventList.Add(evt);
            dictionary.Add(key, eventList);
        }
    }

    /// <summary>
    /// Removes the event from the dictionary if it exists
    /// </summary>
    private void RemoveEvent(Dictionary<Key, List<ButtonEvent>> dictionary, Key key, ButtonEvent evt)
    {
        List<ButtonEvent> eventList;
        if (dictionary.TryGetValue(key, out eventList))
            eventList.Remove(evt);
    }
    #endregion //Generic Add Remove
}

public class ButtonEventArgs : EventArgs
{
    public Key Key { get; private set; }
    public ButtonEventArgs(Key key)
    {
        Key = key;
    }
}
