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
    public class DisplayDropInterval : MonoBehaviour
    {
        [Inject]
        GameStateManager stateManager;

        [Inject]
        MovementManager move;

        TextMeshProUGUI uGui;

        // Start is called before the first frame update
        void Start()
        {
            uGui = GetComponent<TextMeshProUGUI>();

            stateManager.CurrentState
                        .Where(state => state == GameState.LiarTetrisMode)
                        .Subscribe(_ => ShowDropInterval($"LiarModeDropInterval:{move.LiarModeDropInterval}")).AddTo(gameObject);

            stateManager.CurrentState
                        .Where(state => state == GameState.NormalTetrisMode)
                        .Subscribe(_ => ShowDropInterval($"NormalModeDropInterval:{move.NormalModeDropInterval}")).AddTo(gameObject);
        }

        void ShowDropInterval(string info)
        {
            uGui.text = info;
        }
    }
}