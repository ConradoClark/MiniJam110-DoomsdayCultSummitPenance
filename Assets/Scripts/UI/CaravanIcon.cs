using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;
using Random = UnityEngine.Random;

public class CaravanIcon : BaseUIObject
{
    protected override void OnAwake()
    {
        base.OnAwake();
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(Shrink());
        DefaultMachinery.AddBasicMachine(Wander());
    }

    private IEnumerable<IEnumerable<Action>> Wander()
    {
        while (isActiveAndEnabled)
        {
            yield return transform.GetAccessor()
                .Position
                .X
                .Increase(0.35f)
                .Over(Random.Range(0.8f, 2f))
                .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
                .UsingTimer(UITimer)
                .Build();

            yield return transform.GetAccessor()
                .Position
                .X
                .Decrease(0.35f)
                .Over(Random.Range(0.8f, 2f))
                .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
                .UsingTimer(UITimer)
                .Build();
        }
    }

    private IEnumerable<IEnumerable<Action>> Shrink()
    {
        while (isActiveAndEnabled)
        {
            yield return transform.GetAccessor()
                .LocalScale
                .Y
                .Increase(0.15f)
                .Over(0.25f)
                .WithStep(0.025f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(UITimer)
                .Build();

            yield return transform.GetAccessor()
                .LocalScale
                .Y
                .Decrease(0.15f)
                .Over(0.25f)
                .WithStep(0.025f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(UITimer)
                .Build();
        }

    }
}
