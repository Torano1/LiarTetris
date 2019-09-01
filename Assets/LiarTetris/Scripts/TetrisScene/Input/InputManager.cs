using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace LiarTetris
{
    public class InputManager : MonoBehaviour
    {
        [Inject]
        ILiarTetrisInput input;

        BoolReactiveProperty moveRight = new BoolReactiveProperty(false);
        public ReactiveProperty<bool> OnMoveRightButton => moveRight;

        BoolReactiveProperty moveLeft = new BoolReactiveProperty(false);
        public ReactiveProperty<bool> OnMoveLeftButton => moveLeft;

        BoolReactiveProperty moveDown = new BoolReactiveProperty(false);
        public ReactiveProperty<bool> OnMoveDownButton => moveDown;

        BoolReactiveProperty rotateRight = new BoolReactiveProperty(false);
        public ReactiveProperty<bool> OnDownRotateRightButton => rotateRight;

        BoolReactiveProperty rotateLeft = new BoolReactiveProperty(false);
        public ReactiveProperty<bool> OnDownRotateLeftButton => rotateLeft;

        BoolReactiveProperty hardDrop = new BoolReactiveProperty(false);
        public ReactiveProperty<bool> OnDownHardDropButton => hardDrop;

        BoolReactiveProperty holdButton = new BoolReactiveProperty(false);
        public ReactiveProperty<bool> OnDownHoldButton => holdButton;

        BoolReactiveProperty menuButton = new BoolReactiveProperty(false);
        public ReactiveProperty<bool> OnDownMenuButton => menuButton;

        // Start is called before the first frame update
        void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => input != null)
                .Subscribe(_ =>
                    {
                        moveRight.Value = input.GetMoveRightButton();
                        moveLeft.Value = input.GetMoveLeftButton();
                        moveDown.Value = input.GetMoveDownButton();
                        rotateRight.Value = input.GetRotateRightButtonDown();
                        rotateLeft.Value = input.GetRotateLeftButtonDown();
                        hardDrop.Value = input.GetHardDropButtonDown();
                        holdButton.Value = input.GetHoldButtonDown();
                        menuButton.Value = input.GetMenuButtonDown();
                    }
                );
        }
    }
}
