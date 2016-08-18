using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainController
{
    private static InputSource _mainInputSource = new InputSource(Resources.Load<KeyBinding>("Prefabs/Keyboard"));

    public static InputSource MainInputSource
    {
        get { return _mainInputSource; }
        private set { _mainInputSource = value; }
    }
}
