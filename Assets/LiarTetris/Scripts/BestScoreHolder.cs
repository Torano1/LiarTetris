using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace LiarTetris
{
    public class BestScoreHolder : MonoBehaviour
    {

        IntReactiveProperty bestLineCount = new IntReactiveProperty();
        IntReactiveProperty bestPoints = new IntReactiveProperty();

        public IntReactiveProperty BestLineCount => bestLineCount;
        public IntReactiveProperty BestPoints => bestPoints;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void UpdateScore(int lineCount, int points)
        {
            UpdateBestLineCount(lineCount);
            UpdateBestPoints(points);
        }

        public void UpdateBestLineCount(int lineCount)
        {
            bestLineCount.Value = Mathf.Max(bestLineCount.Value, lineCount);
        }

        public void UpdateBestPoints(int points)
        {
            bestPoints.Value = Mathf.Max(bestPoints.Value, points);
        }
    }
}
