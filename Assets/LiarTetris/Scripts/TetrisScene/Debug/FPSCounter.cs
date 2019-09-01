using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public static class FPSCounter
{
    const int BufferSize = 5;
    public static IReadOnlyReactiveProperty<float> Current { get; private set; }

    static FPSCounter()
    {
        Current = Observable.EveryUpdate()
            .Select(_ => Time.deltaTime)
            .Buffer(BufferSize, 1)
            .Select(x => 1.0f / x.Average())
            .ToReadOnlyReactiveProperty();
    }
}
