using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace LiarTetris
{
    public class ScoreManager : MonoBehaviour
    {
        [Inject]
        GameStateManager stateManager;

        [Inject]
        BestScoreHolderFinder bestScoreHoldlerFinder;

        ReactiveProperty<int> linesBrokenInCurrentMode = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> CurrentLines => linesBrokenInCurrentMode;

        ReactiveProperty<int> brokenLineCount = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> TotalLineCount => brokenLineCount;

        int pointsForSingleLine = 100;
        ReactiveProperty<int> points = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> Points => points;

        [SerializeField]
        int amplifyForLiarTetrisMode = 5;
        int Amplify
        {
            get => stateManager != null && stateManager.CurrentState.Value == GameState.LiarTetrisMode ? amplifyForLiarTetrisMode : 1;
        }

        // Start is called before the first frame update
        void Start()
        {
            InitializeScores();

            stateManager.CurrentState
                        .Where(state => state == GameState.NormalTetrisMode || state == GameState.LiarTetrisMode)
                        .Subscribe(_ => linesBrokenInCurrentMode.Value = 0);

            TotalLineCount.Subscribe(lineCount => bestScoreHoldlerFinder.UpdateBestLineCount(lineCount)).AddTo(gameObject);
            Points.Subscribe(points => bestScoreHoldlerFinder.UpdateBestPoints(points)).AddTo(gameObject);
        }

        void InitializeScores()
        {
            if (brokenLineCount.Value != 0)
            {
                brokenLineCount.SetValueAndForceNotify(0);
            }

            if (points.Value != 0)
            {
                points.SetValueAndForceNotify(0);
            }
        }

        public void ClearLines(int numLines)
        {
            linesBrokenInCurrentMode.Value += numLines;
            brokenLineCount.Value += numLines;
            points.Value += Amplify * numLines * pointsForSingleLine;
        }
    }
}
