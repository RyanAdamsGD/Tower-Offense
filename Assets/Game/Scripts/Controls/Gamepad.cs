using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Gamepad : MonoBehaviour
{
    enum State
    {
        Pressed,
        Held,
        Released,
        Nothing
    }

    BoundKeyDictionary _boundKeys;
    Dictionary<KeyCode, State> _previousState;

    public delegate void ButtonStateChange(object sender, ButtonStateChangeEventArgs args);
    public event ButtonStateChange ButtonPressed;
    public event ButtonStateChange ButtonHeld;
    public event ButtonStateChange ButtonReleased;

    public void SetBoundKeys(BoundKeyDictionary boundKeys)
    {
        _boundKeys = boundKeys;
        _previousState = new Dictionary<KeyCode, State>();
        foreach (var key in boundKeys.Keys)
        {
            _previousState.Add(key, State.Nothing);
        }
    }

    /// <summary>
    /// Check for changes to any of the relevant key states
    /// </summary>
    void Update()
    {
        if (_boundKeys == null)
            return;

        foreach (var keycode in _boundKeys.Keys)
        {
            bool buttonState = Input.GetKey(keycode);
            List<Key> keys = _boundKeys[keycode].List;
            if (buttonState)
            {
                switch (_previousState[keycode])
                {
                    case State.Pressed:
                        foreach (var key in keys)
                            InvokeButtonHeld(key);
                        _previousState[keycode] = State.Held;
                        break;
                    case State.Held:
                        foreach (var key in keys)
                            InvokeButtonHeld(key);
                        break;
                    case State.Released:
                        foreach (var key in keys)
                        {
                            InvokeButtonPressed(key);
                            InvokeButtonHeld(key);
                        }
                        _previousState[keycode] = State.Pressed;
                        break;
                    case State.Nothing:
                        foreach (var key in keys)
                        {
                            InvokeButtonPressed(key);
                            InvokeButtonHeld(key);
                        }
                        _previousState[keycode] = State.Pressed;
                        break;
                }
            }
            else
            {
                switch (_previousState[keycode])
                {
                    case State.Pressed:
                        foreach (var key in keys)
                            InvokeButtonReleased(key);
                        _previousState[keycode] = State.Released;
                        break;
                    case State.Held:
                        foreach (var key in keys)
                            InvokeButtonReleased(key);
                        _previousState[keycode] = State.Released;
                        break;
                    case State.Released:
                        _previousState[keycode] = State.Nothing;
                        break;
                }
            }
        }
    }

    private void InvokeButtonReleased(Key key)
    {
        var buttonReleasedEvent = ButtonReleased;
        if (ButtonReleased != null)
            buttonReleasedEvent(this, new ButtonStateChangeEventArgs(key));
    }

    private void InvokeButtonHeld(Key key)
    {
        var buttonHeldEvent = ButtonHeld;
        if (ButtonHeld != null)
            buttonHeldEvent(this, new ButtonStateChangeEventArgs(key));
    }

    private void InvokeButtonPressed(Key key)
    {
        var buttonPressedEvent = ButtonPressed;
        if (ButtonPressed != null)
            buttonPressedEvent(this, new ButtonStateChangeEventArgs(key));
    }
}

public class ButtonStateChangeEventArgs
{
    public Key Key { get; private set; }

    public ButtonStateChangeEventArgs(Key key)
    {
        Key = key;
    }
}