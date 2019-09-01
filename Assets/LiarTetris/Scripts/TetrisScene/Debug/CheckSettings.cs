using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.LWRP;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CheckSettings : MonoBehaviour
{
    [SerializeField]
    LightweightRenderPipelineAsset lwrpAsset;

    [SerializeField]
    PostProcessLayer ppLayer;

    TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        var info = "Debug Info\n";
        text.text = info;

        if (lwrpAsset)
        {
            info = $"LWRP Asset HDR support:{lwrpAsset.supportsHDR}\n";
            text.text += info;
        }

        if (ppLayer)
        {
            info = $"antialiasingMode:{ppLayer.antialiasingMode}";
            text.text += info;
        }
    }
}
