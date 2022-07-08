using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;

public class BounceOnEnemies : BaseGameObject
{
    public LichtPlatformerJumpController.CustomJumpParams BounceParams;
    public ScriptIdentifier Grounded;
    public LichtPlatformerJumpController JumpController;
    public LichtPhysicsCollisionDetector BouncingDetector;
    public LichtPhysicsObject PhysicsObject;

    private IEventPublisher<HitEvents, OnHitEventArgs> _hitPublisher;
    private LichtPhysics _physics;

    protected override void OnAwake()
    {
        base.OnAwake();
        _physics = this.GetLichtPhysics();
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleBouncing());
        _hitPublisher = this.RegisterAsEventPublisher<HitEvents, OnHitEventArgs>();
    }

    private IEnumerable<IEnumerable<Action>> HandleBouncing()
    {
        while (isActiveAndEnabled)
        {
            var bouncing = BouncingDetector.Triggers.FirstOrDefault();
            // Triggered bouncing / Not jumping / Not on ground
            if (bouncing.TriggeredHit && !JumpController.IsJumping && !PhysicsObject.GetPhysicsTrigger(Grounded) &&
                _physics.TryGetPhysicsObjectByCollider(bouncing.Collider, out var targetObject))
            {
                if (targetObject.TryGetCustomObject<Faintable>(out var faintable) && faintable.IsFainted)
                {
                    yield return TimeYields.WaitOneFrameX; //can be optimized
                    continue;
                }

                _hitPublisher.PublishEvent(HitEvents.OnHit, new OnHitEventArgs
                {
                    Source = PhysicsObject,
                    Target = targetObject,
                });

                yield return JumpController.ExecuteJump(customParams: BounceParams).AsCoroutine();
                continue;
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
