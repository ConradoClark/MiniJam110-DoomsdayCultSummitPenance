using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;
using Random = UnityEngine.Random;

public class Kick : BaseGameObject
{
    public float KickCooldownInSeconds;
    public float KickStrength;
    public LichtPhysicsCollisionDetector KickDetector;

    public AudioSource KickSFX;

    private LichtPhysics _physics;

    private void OnEnable()
    {
        _physics = this.GetLichtPhysics();
        DefaultMachinery.AddBasicMachine(HandleKick());
    }

    private IEnumerable<IEnumerable<Action>> HandleKick()
    {
        while (isActiveAndEnabled)
        {
            Kickable kickable = null;
            var kick = KickDetector.Triggers.FirstOrDefault(t =>
                _physics.TryGetPhysicsObjectByCollider(t.Collider, out var physicsObject) &&
                physicsObject.TryGetCustomObject(out kickable));

            if (kick.Collider != null && kickable.IsKickable())
            {
                if (kickable.RedirectTo != null && kickable != kickable.RedirectTo)
                {
                    kickable = kickable.RedirectTo;
                }

                KickSFX.pitch = 0.9f + Random.value * 0.2f;
                KickSFX.Play();

                kickable.PhysicsObject.TryGetCustomObject(out Bouncy bouncy);
                kickable.SetKicked();

                var initialSpeed = new Vector2(-kick.Hit.normal.x * KickStrength, KickStrength * 0.36f);

                var kickX = kickable.PhysicsObject.GetSpeedAccessor(initialSpeed)
                    .X
                    .SetTarget(0)
                    .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
                    .Over(0.5f)
                    .BreakIf(() => bouncy != null && bouncy.HitByWall(), false)
                    .UsingTimer(GameTimer)
                    .Build();

                var kickY = kickable.PhysicsObject.GetSpeedAccessor(initialSpeed)
                    .Y
                    .SetTarget(0)
                    .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
                    .Over(0.5f)
                    .UsingTimer(GameTimer)
                    .Build();

                DefaultMachinery.AddBasicMachine(kickX.Combine(kickY));
                yield return TimeYields.WaitSeconds(GameTimer, KickCooldownInSeconds);
            }

            yield return TimeYields.WaitOneFrameX;

        }
    }
}
