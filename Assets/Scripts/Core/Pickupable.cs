using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class Pickupable : BaseGameObject
{
    public Transform ObjectToPick;
    public LichtPhysicsObject PhysicsObject;
    public Collider2D PickupCollider;

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
        PhysicsObject.AddCustomObject(this);
    }

    private void OnPickup(PickupEventArgs obj)
    {
        if (obj.Target != this) return;
        PickupCollider.enabled = false;
        PhysicsObject.enabled = false;
    }
}

