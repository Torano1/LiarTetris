using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace LiarTetris
{
    /// <summary>
    /// take input from InputManager and decide which movement is done
    /// </summary>
    public class MovementManager : MonoBehaviour
    {
        [Inject]
        InputManager inputManager;

        [Inject]
        GridManager tetrominoManager;

        [Inject]
        GameStateManager stateManager;

        [SerializeField]
        float moveInterval;

        [SerializeField]
        float normalModeDropInterval;
        public float NormalModeDropInterval => normalModeDropInterval;

        [SerializeField]
        float liarModeDropInterval;
        public float LiarModeDropInterval => liarModeDropInterval;

        [SerializeField]
        float dropIntervalReduction;

        [SerializeField]
        float softDropInterval;

        Subject<Unit> onMoveDownSubject = new Subject<Unit>();
        public System.IObservable<Unit> OnMoveDownObservable() => onMoveDownSubject;

        Subject<Unit> onMoveRightSubject = new Subject<Unit>();
        public System.IObservable<Unit> OnMoveRightObservable() => onMoveRightSubject;

        Subject<Unit> onMoveLeftSubject = new Subject<Unit>();
        public System.IObservable<Unit> OnMoveLeftObservable() => onMoveLeftSubject;

        Subject<Unit> onRotateRightSubject = new Subject<Unit>();
        public System.IObservable<Unit> OnRotateRightObservable() => onRotateRightSubject;

        Subject<Unit> onRotateLeftSubject = new Subject<Unit>();
        public System.IObservable<Unit> OnRotateLeftObservable() => onRotateLeftSubject;

        Subject<Unit> onHardDropSubject = new Subject<Unit>();
        public System.IObservable<Unit> OnHardDropObservable() => onHardDropSubject;

        Subject<Unit> onHoldSubject = new Subject<Unit>();
        public System.IObservable<Unit> OnHoldObservable() => onHoldSubject;


        float elapsedTime = 0;

        bool movable = false;
        public bool Movable { get => movable; set => movable = value; }

        // Start is called before the first frame update
        void Start()
        {
            stateManager.CurrentState
                        .FirstOrDefault(state => state == GameState.NormalTetrisMode)
                        .Subscribe(_ => normalModeDropInterval = normalModeDropInterval + dropIntervalReduction).AddTo(gameObject);

            stateManager.CurrentState
                        .Where(state => state == GameState.NormalTetrisMode)
                        .Subscribe(_ => normalModeDropInterval = Mathf.Max(softDropInterval, normalModeDropInterval - dropIntervalReduction)).AddTo(gameObject);

            stateManager.CurrentState
                        .Where(state => state == GameState.LiarTetrisMode)
                        .Subscribe(_ => liarModeDropInterval = Mathf.Max(softDropInterval, liarModeDropInterval - dropIntervalReduction)).AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_ => stateManager && (stateManager.CurrentState.Value == GameState.NormalTetrisMode || stateManager.CurrentState.Value == GameState.LiarTetrisMode))
                .Where(_ => Movable)
                .Do(_ => elapsedTime += Time.deltaTime)
                .Select(_ => (inputManager && inputManager.OnMoveDownButton.Value) ? softDropInterval : stateManager.CurrentState.Value == GameState.NormalTetrisMode ? normalModeDropInterval : liarModeDropInterval)
                .Where(interval => elapsedTime >= interval)
                .Subscribe(_ =>
                {
                    elapsedTime = 0;
                    MoveDown();
                });

            this.UpdateAsObservable()
                .Where(_ => stateManager && (stateManager.CurrentState.Value == GameState.NormalTetrisMode || stateManager.CurrentState.Value == GameState.LiarTetrisMode))
                .Where(_ => Movable && inputManager != null && inputManager.OnMoveRightButton.Value)
                .ThrottleFirst(System.TimeSpan.FromMilliseconds(moveInterval))
                .Subscribe(_ => MoveRight());

            this.UpdateAsObservable()
                .Where(_ => stateManager && (stateManager.CurrentState.Value == GameState.NormalTetrisMode || stateManager.CurrentState.Value == GameState.LiarTetrisMode))
                .Where(_ => Movable && inputManager != null && inputManager.OnMoveLeftButton.Value)
                .ThrottleFirst(System.TimeSpan.FromMilliseconds(moveInterval))
                .Subscribe(_ => MoveLeft());

            this.UpdateAsObservable()
                .Where(_ => stateManager && (stateManager.CurrentState.Value == GameState.NormalTetrisMode || stateManager.CurrentState.Value == GameState.LiarTetrisMode))
                .Where(_ => Movable && inputManager != null && inputManager.OnDownRotateRightButton.Value)
                .ThrottleFirst(System.TimeSpan.FromMilliseconds(moveInterval))
                .Subscribe(_ => RotateRight());

            this.UpdateAsObservable()
                .Where(_ => stateManager && (stateManager.CurrentState.Value == GameState.NormalTetrisMode || stateManager.CurrentState.Value == GameState.LiarTetrisMode))
                .Where(_ => Movable && inputManager != null && inputManager.OnDownRotateLeftButton.Value)
                .ThrottleFirst(System.TimeSpan.FromMilliseconds(moveInterval))
                .Subscribe(_ => RotateLeft());

            inputManager.OnDownHardDropButton
                        .Where(_ => stateManager && (stateManager.CurrentState.Value == GameState.NormalTetrisMode || stateManager.CurrentState.Value == GameState.LiarTetrisMode))
                        .Where(down => down && Movable)
                        .Subscribe(_ => HardDrop());

            inputManager.OnDownHoldButton
                        .Where(_ => stateManager && (stateManager.CurrentState.Value == GameState.NormalTetrisMode || stateManager.CurrentState.Value == GameState.LiarTetrisMode))
                        .Where(down => down && Movable)
                        .Subscribe(_ => Hold());
        }

        void MoveDown()
        {
            onMoveDownSubject.OnNext(Unit.Default);
        }

        void MoveRight()
        {
            onMoveRightSubject.OnNext(Unit.Default);
        }

        void MoveLeft()
        {
            onMoveLeftSubject.OnNext(Unit.Default);
        }

        void RotateRight()
        {
            onRotateRightSubject.OnNext(Unit.Default);
        }

        void RotateLeft()
        {
            onRotateLeftSubject.OnNext(Unit.Default);
        }

        void HardDrop()
        {
            onHardDropSubject.OnNext(Unit.Default);
        }

        void Hold()
        {
            onHoldSubject.OnNext(Unit.Default);
        }
    }
}
