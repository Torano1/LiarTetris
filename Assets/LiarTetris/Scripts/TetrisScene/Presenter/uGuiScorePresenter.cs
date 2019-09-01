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
    public class uGuiScorePresenter : MonoBehaviour
    {
        [Inject]
        GameStateManager stateManager;

        [Inject]
        ScoreManager scoreManager;

        TextMeshProUGUI uGui;

        string pointsInfo, linesInfo;

        // Start is called before the first frame update
        void Start()
        {
            uGui = GetComponent<TextMeshProUGUI>();

            scoreManager.TotalLineCount
                        .Where(lines => lines > 0)
                        .Subscribe(lines =>
                        {
                            UpdateLinesInfo(lines);
                            DisplayScore();
                        });

            scoreManager.Points
                        .Where(points => points > 0)
                        .Subscribe(points =>
                        {
                            UpdatePointsInfo(points);
                            DisplayScore();
                        });

            UpdateLinesInfo(0);
            UpdatePointsInfo(0);
            DisplayScore();
        }

        void UpdateLinesInfo(int lines)
        {
            linesInfo = $"Lines {lines}";
        }

        void UpdatePointsInfo(int points)
        {
            pointsInfo = $"Points {points}";
        }

        void DisplayScore()
        {
            uGui.text = $"{linesInfo}\n{pointsInfo}";
        }
    }
}