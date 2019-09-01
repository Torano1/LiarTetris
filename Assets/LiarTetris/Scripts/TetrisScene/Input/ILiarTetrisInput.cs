using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiarTetris
{
    public interface ILiarTetrisInput
    {
        bool GetMoveRightButton();
        bool GetMoveLeftButton();
        bool GetMoveDownButton();
        bool GetRotateRightButtonDown();
        bool GetRotateLeftButtonDown();
        // button to hard drop a tetromino
        bool GetHardDropButtonDown();
        // button to hold a tetromino
        bool GetHoldButtonDown();
        bool GetMenuButtonDown();
    }
}