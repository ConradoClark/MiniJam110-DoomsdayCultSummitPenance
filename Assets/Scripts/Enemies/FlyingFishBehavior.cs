using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class FlyingFishBehavior : BaseGameObject
{
    public Damageable Damageable;
    public LichtPhysicsObject PhysicsObject;
    public float Speed;
    public float Time;

    private Vector3 _originalPosition;

    private void OnEnable()
    {
        _originalPosition = transform.position;
        DefaultMachinery.AddBasicMachine(HandleFlyingFish());
    }

    private IEnumerable<IEnumerable<Action>> HandleFlyingFish()
    {
        yield return TimeYields.WaitOneFrameX;

        while (isActiveAndEnabled)
        {
            while (Damageable.CurrentHitPoints > 0 && Time > 0)
            {
                yield return PhysicsObject.GetSpeedAccessor(new Vector2(0, Speed))
                    .Y
                    .SetTarget(0)
                    .Over(Time)
                    .Easing(EasingYields.EasingFunction.CubicEaseOut)
                    .BreakIf(() => Damageable.CurrentHitPoints < 0)
                    .UsingTimer(GameTimer)
                    .Build();

                if (Damageable.CanBeDamaged) transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);

                yield return PhysicsObject.GetSpeedAccessor()
                    .Y
                    .SetTarget(-Speed)
                    .Over(Time)
                    .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
                    .BreakIf(() => Damageable.CurrentHitPoints < 0)
                    .UsingTimer(GameTimer)
                    .Build();

                if (Damageable.CanBeDamaged)
                {
                    transform.rotation = Quaternion.identity;
                    transform.position = _originalPosition;
                }
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
