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
    public SpriteRenderer Renderer;

    public float RotationAngle = 180;
    public float DecreaseY = 5f;

    public override IEnumerable<IEnumerable<Action>> Kill()
    {
        PhysicsObject.enabled = false;

        transform.localRotation = Quaternion.AngleAxis(RotationAngle, Vector3.forward);

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
            .Decrease(DecreaseY)
            .Over(1f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .UsingTimer(GameTimer)
            .Build();

        Renderer.enabled = false;
    }

    public override bool PerformReset()
    {
        PhysicsObject.enabled = true;
        transform.localRotation = Quaternion.identity;
        Renderer.enabled = true;
        return base.PerformReset();
    }
}
