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
    public class uGuiGameModePresenter : MonoBehaviour
    {
        [Inject]
        GameTimeManager timeManager;

        [Inject]
        GameStateManager stateManager;

        [SerializeField]
        Color normalTetrisColor = Color.blue, liarTetrisColor = Color.red;

        string gameModeInfo;
        TextMeshProUGUI uGui;

        // Start is called before the first frame update
        void Start()
        {
            uGui = GetComponent<TextMeshProUGUI>();
            uGui.enabled = false;

            stateManager.CurrentState
                        .Select(state => state == GameState.LiarTetrisMode || state == GameState.NormalTetrisMode)
                        .Subscribe(enabled => uGui.enabled = enabled).AddTo(uGui);

            stateManager.CurrentState
                        .Where(state => state == GameState.NormalTetrisMode)
                        .Subscribe(_ => uGui.color = normalTetrisColor);

            stateManager.CurrentState
                        .Where(state => state == GameState.LiarTetrisMode)
                        .Subscribe(_ => uGui.color = liarTetrisColor);

            stateManager.CurrentState
                        .Subscribe(state => gameModeInfo = System.Enum.GetName(typeof(GameState), state)).AddTo(gameObject);

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
            uGui.text = $"{gameModeInfo}\n{time}";
        }
    }
}
