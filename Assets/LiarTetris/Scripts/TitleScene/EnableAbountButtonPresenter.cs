using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using TMPro;
using UnityEngine.UI;

namespace LiarTetris
{

    [RequireComponent(typeof(Button))]
    public class EnableAbountButtonPresenter : MonoBehaviour
    {
        [SerializeField]
        AboutPresenter aboutPresenter;

        Button button;
        // Start is called before the first frame update
        void Start()
        {

            button = GetComponent<Button>();

            button.OnClickAsObservable()
                  .Subscribe(_ => {
                      Debug.Log("pressed enable about button");

                      aboutPresenter.EnableButtonAndUGUI();
                      });
        }

        // Update is called once per frame
        void Update()
        {
            button.interactable = !aboutPresenter.IsEnabled;
        }
    }
}
