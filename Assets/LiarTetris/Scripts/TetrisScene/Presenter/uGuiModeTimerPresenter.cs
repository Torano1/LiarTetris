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
    public class uGuiModeTimerPresenter : MonoBehaviour
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
            uGui.text = "";

            timeManager.NormalTetrisModeTimer
                       .Subscribe(time =>
                       {
                           DisplayModeAndTime(time);
                       }).AddTo(gameObject);

            timeManager.LiarTetrisModeTimer
                       .Subscribe(time =>
                       {
                           DisplayModeAndTime(time);
                       }).AddTo(gameObject);
        }

        void DisplayModeAndTime(int time)
        {
            uGui.text = $"{time}";
        }
    }
}