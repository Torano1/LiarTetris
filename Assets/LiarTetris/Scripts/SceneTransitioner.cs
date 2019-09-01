using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LiarTetris
{
    public class SceneTransitioner : MonoBehaviour
    {
        [Inject]
        INextSceneProvider nextSceneProvider;

        [SerializeField]
        float timeToTransition = 1f;

        [SerializeField]
        Image transitionImage;

        float waitTime = 0.05f;

        ReactiveProperty<bool> onTransitionning = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> OnTransitionning => onTransitionning;

        private void Start()
        {
            transitionImage.enabled = false;
        }

        public void StartLoadNextScene()
        {
            transitionImage.enabled = true;
            onTransitionning.Value = true;
            StartCoroutine("LoadNextSceneCoroutine");
        }

        IEnumerator LoadNextSceneCoroutine()
        {
            var color = transitionImage.color;
            color.a = 0f;
            transitionImage.color = color;

            var elapsed = 0f;

            while (elapsed < timeToTransition)
            {
                yield return new WaitForSeconds(waitTime);
                elapsed += waitTime;
                var alpha = elapsed / timeToTransition;

                color = transitionImage.color;
                color.a = alpha;
                transitionImage.color = color;
            }

            color = transitionImage.color;
            color.a = 1f;
            transitionImage.color = color;

            LoadScene();
        }

        void LoadScene()
        {
            SceneManager.LoadScene(nextSceneProvider.GetNextScene());
        }
    }
}