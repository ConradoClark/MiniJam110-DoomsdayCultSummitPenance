using System;
using System.Collections.Generic;
using Assets.Scripts.Core;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;

public class BounceOnEnemies : BaseGameObject
{
    public LichtPlatformerJumpController.CustomJumpParams BounceParams;
    public ScriptIdentifier Grounded;
    public LichtPlatformerJumpController JumpController;
    public LichtPhysicsCollisionDetector BouncingDetector;
    public LichtPhysicsObject PhysicsObject;
    public AudioSource BounceSFX;

    public Damageable Damageable;

    private IEventPublisher<HitEvents, OnHitEventArgs> _hitPublisher;
    private LichtPhysics _physics;

    protected override void OnAwake()
    {
        base.OnAwake();
        _physics = this.GetLichtPhysics();
        PhysicsObject.AddCustomObject(this);
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
            foreach (var bouncing in BouncingDetector.Triggers)
            {
                // Triggered bouncing / Not jumping / Not on ground
                if (bouncing.TriggeredHit && !JumpController.IsJumping && !PhysicsObject.GetPhysicsTrigger(Grounded) &&
                    _physics.TryGetPhysicsObjectByCollider(bouncing.Collider, out var targetObject)
                    && targetObject.TryGetCustomObject<CanBeBouncedOn>(out var bounced) &&
                    bounced.CollisionDetector.Collider == bouncing.Collider)
                {
                    if (targetObject.TryGetCustomObject<Faintable>(out var faintable) && faintable.IsFainted)
                    {
                        yield return TimeYields.WaitOneFrameX; //can be optimized
                        break;
                    }

                    _hitPublisher.PublishEvent(HitEvents.OnHit, new OnHitEventArgs
                    {
                        Source = PhysicsObject,
                        Target = targetObject,
                    });

                    if (!JumpController.IsJumping)
                    {
                        BounceSFX.Play();
                        yield return JumpController.ExecuteJump(customParams: BounceParams).AsCoroutine();
                    }
                    break;
                }
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
