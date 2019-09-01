using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;
using TMPro;

namespace LiarTetris
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class uGuiReadyTimerPresenter : MonoBehaviour
    {
        [Inject]
        GameTimeManager timeManager;


        [Inject]
        GameStateManager stateManager;

        TextMeshProUGUI uGui;

        // Start is called before the first frame update
        void Start()
        {
            uGui = GetComponent<TextMeshProUGUI>();

            stateManager.CurrentState
                        .Where(state => state != GameState.Ready)
                        .Subscribe(_ => UnableUgui()).AddTo(gameObject);

            timeManager.ReadyTimer
                       .Subscribe(time =>
                       {
                           DisplayReadyTime(time);
                       }).AddTo(gameObject);
        }

        void DisplayReadyTime(int time)
        {
            uGui.text = $"{time}";
        }

        void UnableUgui()
        {
            uGui.enabled = false;
        }
    }
}
