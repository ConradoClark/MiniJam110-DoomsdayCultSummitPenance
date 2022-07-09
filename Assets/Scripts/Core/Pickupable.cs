using System;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class Pickupable : BaseGameObject
{
    public Transform ObjectToPick;
    public LichtPhysicsObject PhysicsObject;
    public Collider2D PickupCollider;
    public ScriptIdentifier Gravity;
    public bool IsPickedUp { get; private set; }

    public bool AllowsPickup { get;  set; }
    public bool Releasing { get; private set; }

    private Transform _originalParent;
    private LichtPhysics _physics;

    private void OnEnable()
    {
        this.ObserveEvent<PickupEvents, PickupEventArgs>(PickupEvents.OnPickup, OnPickup);
    }

    private void OnDisable()
    {
        this.StopObservingEvent<PickupEvents, PickupEventArgs>(PickupEvents.OnPickup, OnPickup);
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _physics = this.GetLichtPhysics();
        PhysicsObject.AddCustomObject(this);
        _originalParent = ObjectToPick.parent;
    }

    private void OnPickup(PickupEventArgs obj)
    {
        if (obj.Target != this) return;
        PickupCollider.enabled = false;
        PhysicsObject.enabled = false;
        _physics.BlockCustomPhysicsForceForObject(this, PhysicsObject, Gravity.Name);
        IsPickedUp = true;
    }

    public void Release(Vector2 speed)
    {
        IsPickedUp = false;
        PhysicsObject.enabled = true;
        PickupCollider.enabled = AllowsPickup;
        ObjectToPick.parent = _originalParent;
        _physics.UnblockCustomPhysicsForceForObject(this, PhysicsObject, Gravity.Name);
        DefaultMachinery.AddBasicMachine(HandleRelease(speed));
    }

    private IEnumerable<IEnumerable<Action>> HandleRelease(Vector2 speed)
    {
        Releasing = true;
        PhysicsObject.TryGetCustomObject(out Bouncy bouncy);
        var xThrow = PhysicsObject
            .GetSpeedAccessor(new Vector2(speed.x, 0))
            .X
            .SetTarget(0)
            .Over(1f)
            .BreakIf(() => bouncy != null && bouncy.HitByWall(), false)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(GameTimer)
            .Build();

        var yThrow = PhysicsObject
            .GetSpeedAccessor(new Vector2(0, speed.y))
            .Y
            .SetTarget(0)
            .Over(0.35f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(GameTimer)
            .Build();

        yield return xThrow.Combine(yThrow);
        Releasing = false;
    }
}

