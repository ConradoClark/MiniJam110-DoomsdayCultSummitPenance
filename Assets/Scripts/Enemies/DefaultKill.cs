using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Physics;
using UnityEngine;

public class DefaultKill : Killable
{
    public LichtPhysicsObject PhysicsObject;
    public override IEnumerable<IEnumerable<Action>> Kill()
    {
        PhysicsObject.enabled = false;

        transform.localRotation = Quaternion.AngleAxis(180, Vector3.forward);

        yield return transform.GetAccessor()
            .Position
            .Y
            .Increase(0.5f)
            .Over(0.25f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(GameTimer)
            .Build();

        yield return transform.GetAccessor()
            .Position
            .Y
            .Decrease(5f)
            .Over(1f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .UsingTimer(GameTimer)
            .Build();

        gameObject.SetActive(false); // temporary
    }
}
