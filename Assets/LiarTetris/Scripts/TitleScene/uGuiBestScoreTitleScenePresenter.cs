using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;
using TMPro;

namespace LiarTetris
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class uGuiBestScoreTitleScenePresenter : MonoBehaviour
    {
        [Inject]
        BestScoreHolderFinder bestScoreHolderFinder;

        TextMeshProUGUI uGui;

        // Start is called before the first frame update
        void Start()
        {
            uGui = GetComponent<TextMeshProUGUI>();

            var enabled = bestScoreHolderFinder.BestLineCount.Value > 0;
            uGui.enabled = enabled;
            if (enabled)
            {
                DisplayScores();
            }
        }

        void DisplayScores()
        {
            var lineCount = bestScoreHolderFinder.BestLineCount;
            var points = bestScoreHolderFinder.BestPoints;

            uGui.text = $"Best Score\nLines {lineCount}\nPoints {points}";
        }
    }
}
