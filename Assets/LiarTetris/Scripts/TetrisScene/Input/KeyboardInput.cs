using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiarTetris
{
    public class KeyboardInput : ILiarTetrisInput
    {
        public bool GetHardDropButtonDown()
        {
            return Input.GetKeyDown(KeyCode.UpArrow);
        }

        public bool GetHoldButtonDown()
        {
            return Input.GetKeyDown(KeyCode.Space);
        }

        public bool GetMenuButtonDown()
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }

        public bool GetMoveDownButton()
        {
            return Input.GetKey(KeyCode.DownArrow);
        }

        public bool GetMoveLeftButton()
        {
            return Input.GetKey(KeyCode.LeftArrow);
        }

        public bool GetMoveRightButton()
        {
            return Input.GetKey(KeyCode.RightArrow);
        }

        public bool GetRotateLeftButtonDown()
        {
            return Input.GetKeyDown(KeyCode.Z);
        }

        public bool GetRotateRightButtonDown()
        {
            return Input.GetKeyDown(KeyCode.X);
        }
    }
}