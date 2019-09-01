using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiarTetris
{
    [RequireComponent(typeof(AudioSource))]
    public class TetrisSceneAudioManager : MonoBehaviour
    {
        [SerializeField]
        AudioClip MoveTetrominoSE, RotateTetrominoSE, SoftDropSE, HardDropSE, HoldSE, LineClearSE, LineClearAndEnableLiarModeSE, ReleaseLiarModeSE, EnableLiarModeSE, EnableNormalModeSE, GameOverSE, ModeCountDownSE, OnUiSE, StartCountDownSE;

        AudioSource audioSource;

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayReleaseLiarModeSE()
        {
            PlaySE(ReleaseLiarModeSE);
        }

        public void PlayMoveTetrominoSE()
        {
            PlaySE(MoveTetrominoSE);
        }

        public void PlayRotateTetrominoSE()
        {
            PlaySE(RotateTetrominoSE);
        }

        public void PlaySoftDropSE()
        {
            PlaySE(SoftDropSE);
        }

        public void PlayHardDropSE()
        {
            PlaySE(HardDropSE);
        }

        public void PlayHoldSE()
        {
            PlaySE(HoldSE);
        }

        public void PlayLineClearSE()
        {
            PlaySE(LineClearSE);
        }

        public void PlayLineClearAndEnableLiarModeSE()
        {
            if (LineClearAndEnableLiarModeSE != null)
            {
                PlaySE(LineClearAndEnableLiarModeSE);
            }
            else
            {
                PlayLineClearSE();
            }
        }

        public void PlayEnableLiarModeSE()
        {
            PlaySE(EnableLiarModeSE);
        }

        public void PlayEnableNormalModeSE()
        {
            PlaySE(EnableNormalModeSE);
        }

        public void PlayGameOverSE()
        {
            PlaySE(GameOverSE);
        }

        public void PlayModeCountDownSE()
        {
            PlaySE(ModeCountDownSE);
        }

        public void PlayOnUiSE()
        {
            PlaySE(OnUiSE);
        }

        public void PlayStartCountDownSE()
        {
            PlaySE(StartCountDownSE);
        }

        void PlaySE(AudioClip se)
        {
            if (se != null)
            {
                audioSource.PlayOneShot(se);
            }
        }
    }
}
