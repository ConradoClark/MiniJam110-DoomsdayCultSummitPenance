using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

public class IndicatorIcon : BaseUIObject
{
    protected override void OnAwake()
    {
        base.OnAwake();
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(Float());
    }

    private IEnumerable<IEnumerable<Action>> Float()
    {
        while (isActiveAndEnabled)
        {
            yield return transform.GetAccessor()
                .Position
                .Y
                .Increase(0.25f)
                .Over(0.5f)
                .WithStep(0.1f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(UITimer)
                .Build();

            yield return transform.GetAccessor()
                .Position
                .Y
                .Decrease(0.25f)
                .Over(0.5f)
                .WithStep(0.05f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(UITimer)
                .Build();
        }

    }
}
