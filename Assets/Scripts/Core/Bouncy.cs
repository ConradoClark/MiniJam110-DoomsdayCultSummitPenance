using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;

public class Bouncy : BaseGameObject
{
    public LichtPhysicsObject PhysicsObject;
    public LichtPhysicsCollisionDetector WallDetector;
    public Kickable Kickable;
    public Pickupable Pickupable;
    public float BounceFactor;

    protected override void OnAwake()
    {
        base.OnAwake();
        PhysicsObject.AddCustomObject(this);
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleBounce());
    }

    private IEnumerable<IEnumerable<Action>> HandleBounce()
    {
        while (isActiveAndEnabled)
        {
            if (HitByWall())
            {
                var bounceSpeed = 1 * -PhysicsObject.LatestDirection.x * BounceFactor;

                Debug.Log("bounced! Bounce speed: "+ bounceSpeed);

                var kickX = PhysicsObject.GetSpeedAccessor(new Vector2(bounceSpeed,0))
                    .X
                    .SetTarget(0)
                    .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
                    .Over(0.5f)
                    .UsingTimer(GameTimer)
                    .Build();

                DefaultMachinery.AddBasicMachine(kickX);
                yield return TimeYields.WaitMilliseconds(GameTimer, 100);
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    public bool HitByWall()
    {
        return WallDetector.Triggers.Any(t => t.TriggeredHit && Mathf.Abs(t.Hit.normal.x) > 0) &&
               (Pickupable != null && Pickupable.Releasing || Kickable == null || Kickable.WasKickedRecently);
    }
}
