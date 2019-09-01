using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using LiarTetris;

public class DebugInput : MonoBehaviour
{
    [Inject]
    InputManager inputManager;

    // Start is called before the first frame update
    void Start()
    {
        inputManager.OnMoveRightButton.Where(input => input).Subscribe(_ => Log("move right"));
        inputManager.OnMoveLeftButton.Where(input => input).Subscribe(_ => Log("move left"));
        inputManager.OnMoveDownButton.Where(input => input).Subscribe(_ => Log("move down"));
        inputManager.OnDownRotateRightButton.Where(input => input).Subscribe(_ => Log("rotate right"));
        inputManager.OnDownRotateLeftButton.Where(input => input).Subscribe(_ => Log("rotate left"));
        inputManager.OnDownHardDropButton.Where(input => input).Subscribe(_ => Log("hard drop"));
        inputManager.OnDownHoldButton.Where(input => input).Subscribe(_ => Log("hold"));
        inputManager.OnDownMenuButton.Where(input => input).Subscribe(_ => Log("open menu"));
    }

    void Log(string text)
    {
        Debug.Log(text);
    }
}
