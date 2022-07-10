using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;
using Random = UnityEngine.Random;

public class Collectable : BaseGameObject
{
    public ScriptPrefab ItemShine;

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(Shine());
        DefaultMachinery.AddBasicMachine(Wander());
    }

    private IEnumerable<IEnumerable<Action>> Shine()
    {
        while (isActiveAndEnabled)
        {
            if (ItemShine.Pool.TryGetFromPool(out var effect))
            {
                effect.Component.transform.position = transform.position + (Vector3) Random.insideUnitCircle * 0.1f;
            }
            yield return TimeYields.WaitMilliseconds(GameTimer, 500);
        }
    }

    private IEnumerable<IEnumerable<Action>> Wander()
    {
        while (isActiveAndEnabled)
        {
            yield return transform.GetAccessor()
                .Position
                .Y
                .Increase(0.15f)
                .Over(1f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(GameTimer)
                .Build();

            yield return transform.GetAccessor()
                .Position
                .Y
                .Decrease(0.15f)
                .Over(1f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(GameTimer)
                .Build();
        }
    }
}
