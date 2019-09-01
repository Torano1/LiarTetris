using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace LiarTetris
{
    public class BestScoreHolderFinder : MonoBehaviour
    {
        BestScoreHolder bestScoreHolderInstance;
        public IntReactiveProperty BestLineCount => bestScoreHolderInstance?.BestLineCount;
        public IntReactiveProperty BestPoints => bestScoreHolderInstance?.BestPoints;

        private void Awake()
        {
            bestScoreHolderInstance = FindObjectOfType<BestScoreHolder>();

            if (bestScoreHolderInstance == null)
            {
                var obj = new GameObject("BestScoreHolder");
                bestScoreHolderInstance = obj.AddComponent<BestScoreHolder>();
            }
        }

        public void UpdateScore(int lineCount, int points)
        {
            bestScoreHolderInstance.UpdateScore(lineCount, points);
        }

        public void UpdateBestLineCount(int lineCount)
        {
            bestScoreHolderInstance.UpdateBestLineCount(lineCount);
        }

        public void UpdateBestPoints(int points)
        {
            bestScoreHolderInstance.UpdateBestPoints(points);
        }
    }
}
