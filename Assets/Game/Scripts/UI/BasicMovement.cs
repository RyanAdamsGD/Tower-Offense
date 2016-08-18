using UnityEngine;
using System.Collections;
using System;

public class BasicMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController _characterController;

    // Use this for initialization
    void Start()
    {
        MainController.MainInputSource.RegisterButtonHold(Key.MoveUp, Move);
        MainController.MainInputSource.RegisterButtonHold(Key.MoveLeft, Move);
        MainController.MainInputSource.RegisterButtonHold(Key.MoveRight, Move);
        MainController.MainInputSource.RegisterButtonHold(Key.MoveDown, Move);
    }

    private void Move(InputSource sender, ButtonEventArgs args)
    {
        switch (args.Key)
        {
            case Key.MoveUp:
                _characterController.Move(Vector3.up);
                break;
            case Key.MoveDown:
                _characterController.Move(Vector3.down);
                break;
            case Key.MoveLeft:
                _characterController.Move(Vector3.left);
                break;
            case Key.MoveRight:
                _characterController.Move(Vector3.right);
                break;
        }
    }

    void OnDestroy()
    {
        MainController.MainInputSource.UnRegisterButtonHold(Key.MoveUp, Move);
        MainController.MainInputSource.UnRegisterButtonHold(Key.MoveLeft, Move);
        MainController.MainInputSource.UnRegisterButtonHold(Key.MoveRight, Move);
        MainController.MainInputSource.UnRegisterButtonHold(Key.MoveDown, Move);
    }
}
