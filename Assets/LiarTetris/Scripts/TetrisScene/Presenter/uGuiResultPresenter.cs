using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using TMPro;

namespace LiarTetris
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class uGuiResultPresenter : MonoBehaviour
    {
        [Inject]
        GameStateManager stateManager;

        [Inject]
        ScoreManager scoreManager;

        [Inject]
        BestScoreHolderFinder bestScoreHolderFinder;

        TextMeshProUGUI uGui;

        // Start is called before the first frame update
        void Start()
        {
            uGui = GetComponent<TextMeshProUGUI>();
            uGui.enabled = false;

            stateManager.CurrentState
                        .Where(state => state == GameState.Result)
                        .Subscribe(_ => ShowResult());
        }

        void ShowResult()
        {
            uGui.enabled = true;
            uGui.text = $"Score\nLines {scoreManager.TotalLineCount}\nPoints {scoreManager.Points}\n\nBestScore\nLines {bestScoreHolderFinder.BestLineCount}\nPoints {bestScoreHolderFinder.BestPoints}";
        }
    }
}
