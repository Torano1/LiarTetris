using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace LiarTetris
{
    public enum GameState
    {
        Ready, NormalTetrisMode, LiarTetrisMode, Result, Finished
    }

    public class GameStateManager : MonoBehaviour
    {
        [Inject]
        TetrisSceneAudioManager audioManager;

        [Inject]
        GameTimeManager timeManager;

        [Inject]
        ScoreManager scoreManager;

        [Inject]
        SceneTransitioner sceneTransitioner;

        [SerializeField]
        int numLinesToReleaseLiarMode = 5;

        [SerializeField]
        GameState initialTetrisMode = GameState.NormalTetrisMode;
        public GameState InitialTetrisMode => initialTetrisMode;

        ReactiveProperty<GameState> gameState = new ReactiveProperty<GameState>(GameState.Ready);
        public ReactiveProperty<GameState> CurrentState => gameState;

        // Start is called before the first frame update
        void Start()
        {
            timeManager.ReadyTimer
                       .FirstOrDefault(time => time == 0)
                       .Subscribe(_ =>
                       {
                           gameState.SetValueAndForceNotify(initialTetrisMode);
                       });

            CurrentState.Where(state => state == GameState.NormalTetrisMode && timeManager)
                        .Subscribe(_ =>
                        {
                            audioManager.PlayEnableNormalModeSE();
                            timeManager.StartNormalTetrisModeTimer();
                        });

            CurrentState.Where(state => state == GameState.LiarTetrisMode && timeManager)
                        .Subscribe(_ =>
                        {
                            audioManager.PlayEnableLiarModeSE();
                            timeManager.StartLiarTetrisModeTimer();
                        });

            timeManager.NormalTetrisModeTimer
                       .Where(time => time == 0)
                       .Subscribe(_ =>
                       {
                           gameState.SetValueAndForceNotify(GameState.LiarTetrisMode);
                       }).AddTo(gameObject);

            timeManager.LiarTetrisModeTimer
                       .Where(time => time == 0)
                       .Subscribe(_ =>
                       {
                           gameState.SetValueAndForceNotify(GameState.NormalTetrisMode);
                       }).AddTo(gameObject);

            timeManager.LoadTitleTimer
                       .Where(time => time == 0)
                       .Subscribe(_ =>
                       {
                           gameState.SetValueAndForceNotify(GameState.Finished);
                       }).AddTo(gameObject);

#if UNITY_EDITOR
            //CurrentState.Subscribe(state =>
            //{
            //    Debug.Log($"CurrentState:{state}");
            //});
#endif

            scoreManager.CurrentLines
                        .Where(lines => lines >= numLinesToReleaseLiarMode)
                        .Subscribe(_ => ReleaseLiarMode());

            CurrentState.Where(state => state == GameState.Finished)
                        .Subscribe(_ => sceneTransitioner.StartLoadNextScene());
        }

        public void ReleaseLiarMode()
        {
            if (CurrentState.Value == GameState.LiarTetrisMode)
            {
                audioManager.PlayReleaseLiarModeSE();
                timeManager.SetLiarTetrisModeTimer0();
            }
        }

        public void EnableLiarMode()
        {
            if (CurrentState.Value == GameState.NormalTetrisMode)
            {
                timeManager.SetNormalTetrisModeTimer0();
            }
        }

        public void GameOver()
        {
            audioManager.PlayGameOverSE();
            gameState.SetValueAndForceNotify(GameState.Result);
            timeManager.FinishGame();
        }
    }
}
