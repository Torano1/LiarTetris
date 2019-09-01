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
    public class AboutPresenter : MonoBehaviour
    {
        Button button;

        [SerializeField]
        TextMeshProUGUI uGui;

        public bool IsEnabled => button != null && button.enabled;

        // Start is called before the first frame update
        void Start()
        {
            button = GetComponent<Button>();
            button.interactable = false;
            button.enabled = false;
            uGui.enabled = false;

            button.OnClickAsObservable()
                  .Subscribe(_ =>
                  {
                      button.interactable = false;
                      button.enabled = false;
                      uGui.enabled = false;
                  });
        }

        public void EnableButtonAndUGUI()
        {
            button.interactable = true;
            button.enabled = true;
            uGui.enabled = true;
        }

    }
}
