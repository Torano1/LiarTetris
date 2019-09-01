using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuggerSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] debuggerPrefabs;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        if (debuggerPrefabs != null)
        {
            foreach (var d in debuggerPrefabs)
            {
                Instantiate(d, this.transform);
            }
        }
#endif
    }
}
