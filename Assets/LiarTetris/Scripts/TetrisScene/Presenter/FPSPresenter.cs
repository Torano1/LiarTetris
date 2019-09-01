using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FPSPresenter : MonoBehaviour
{
    TextMeshProUGUI uGui;

    // Start is called before the first frame update
    void Start()
    {
        uGui = GetComponent<TextMeshProUGUI>();
        FPSCounter.Current.Subscribe(fps => DisplayFps(fps));
    }

    void DisplayFps(float fps)
    {
        uGui.text = $"FPS:{fps:#}";
    }
}
