using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace LiarTetris
{
    public class GameTimeManager : MonoBehaviour
    {
        [Inject]
        TetrisSceneAudioManager audioManager;

        [SerializeField] private IntReactiveProperty _readyTimer = new IntReactiveProperty(3);

        [SerializeField] private IntReactiveProperty _normalTetrisModeTimer = new IntReactiveProperty(30);
        [SerializeField] private IntReactiveProperty _liarTetrisModeTimer = new IntReactiveProperty(30);

        [SerializeField] private IntReactiveProperty _loadTitleTimer = new IntReactiveProperty(4);

        public IReadOnlyReactiveProperty<int> ReadyTimer => _readyTimer;
        public IReadOnlyReactiveProperty<int> NormalTetrisModeTimer => _normalTetrisModeTimer;
        public IReadOnlyReactiveProperty<int> LiarTetrisModeTimer => _liarTetrisModeTimer;
        public IReadOnlyReactiveProperty<int> LoadTitleTimer => _loadTitleTimer;

        int initialNormalTetrisModeTime, initialLiarTetrisModeTime;
        public void Start()
        {
            initialNormalTetrisModeTime = _normalTetrisModeTimer.Value;
            initialLiarTetrisModeTime = _liarTetrisModeTimer.Value;

            StartCoroutine(ReadyGameCoroutine());

            NormalTetrisModeTimer.Where(time => time <= 5 && time > 0)
                                 .Subscribe(_ => audioManager.PlayModeCountDownSE());


            LiarTetrisModeTimer.Where(time => time <= 5 && time > 0)
                               .Subscribe(_ => audioManager.PlayModeCountDownSE());

#if UNITY_EDITOR
            //ReadyTimer.Subscribe(time =>
            //{
            //    Debug.Log($"ReadyTimer:{time}");
            //});

            //NormalTetrisModeTimer.Subscribe(time =>
            //{
            //    Debug.Log($"NormalTetrisModeTimer:{time}");
            //});

            //LiarTetrisModeTimer.Subscribe(time =>
            //{
            //    Debug.Log($"LiarTetrisModeTimer:{time}");
            //});

            //FinishTimer.Subscribe(time =>
            //{
            //    Debug.Log($"FinishTimer:{time}");
            //});

            //ResultTimer.Subscribe(time =>
            //{
            //    Debug.Log($"ResultTimer:{time}");
            //});
#endif
        }

        IEnumerator ReadyGameCoroutine()
        {
            _readyTimer.SetValueAndForceNotify(Mathf.Max(1, _readyTimer.Value));
            yield return new WaitForSecondsRealtime(1);

            while (_readyTimer.Value > 0)
            {
                _readyTimer.SetValueAndForceNotify(_readyTimer.Value - 1);
                yield return new WaitForSecondsRealtime(1);
            }
        }

        public void StartNormalTetrisModeTimer()
        {
            StartCoroutine("StartNormalTetrisModeCoroutine");
        }

        IEnumerator StartNormalTetrisModeCoroutine()
        {
            _normalTetrisModeTimer.SetValueAndForceNotify(initialNormalTetrisModeTime);
            yield return new WaitForSecondsRealtime(1);

            while (_normalTetrisModeTimer.Value > 0)
            {
                _normalTetrisModeTimer.SetValueAndForceNotify(_normalTetrisModeTimer.Value - 1);
                yield return new WaitForSecondsRealtime(1);
            }
        }
        public void StartLiarTetrisModeTimer()
        {
            StartCoroutine("StartLiarTetrisModeCoroutine");
        }

        IEnumerator StartLiarTetrisModeCoroutine()
        {
            _liarTetrisModeTimer.SetValueAndForceNotify(initialLiarTetrisModeTime);
            yield return new WaitForSecondsRealtime(1);

            while (_liarTetrisModeTimer.Value > 0)
            {
                _liarTetrisModeTimer.SetValueAndForceNotify(_liarTetrisModeTimer.Value - 1);
                yield return new WaitForSecondsRealtime(1);
            }
        }

        public void SetNormalTetrisModeTimer0()
        {
            if (_normalTetrisModeTimer.Value > 0)
            {
                StopCoroutine("StartNormalTetrisModeCoroutine");
                _normalTetrisModeTimer.SetValueAndForceNotify(0);
            }
        }
        public void SetLiarTetrisModeTimer0()
        {
            if (_liarTetrisModeTimer.Value > 0)
            {
                StopCoroutine("StartLiarTetrisModeCoroutine");
                _liarTetrisModeTimer.SetValueAndForceNotify(0);
            }
        }

        public void FinishGame()
        {
            StopCoroutine("StartNormalTetrisModeCoroutine");
            StopCoroutine("StartLiarTetrisModeCoroutine");
            StartCoroutine(FinishGameCoroutine());
        }

        IEnumerator FinishGameCoroutine()
        {
            _loadTitleTimer.SetValueAndForceNotify(Mathf.Max(1, _loadTitleTimer.Value));
            yield return new WaitForSecondsRealtime(1);

            while (_loadTitleTimer.Value > 0)
            {
                _loadTitleTimer.Value--;
                yield return new WaitForSecondsRealtime(1);
            }
        }
    }
}
