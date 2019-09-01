using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using TMPro;
using UnityEngine.UI;

namespace LiarTetris
{
    [RequireComponent(typeof(Button))]
    public class LoadTetrisScenePresenterButton : MonoBehaviour
    {
        [Inject]
        SceneTransitioner sceneTransitioner;

        [SerializeField]
        AboutPresenter aboutPresenter;

        bool pressable = true;
        public bool Pressable => pressable;

        Button button;
        // Start is called before the first frame update
        void Start()
        {
            button = GetComponent<Button>();

            button.OnClickAsObservable()
                  .Where(_ => pressable)
                  .Subscribe(_ => sceneTransitioner.StartLoadNextScene());
        }

        // Update is called once per frame
        void Update()
        {
            button.interactable = !aboutPresenter.IsEnabled;
        }
    }
}
