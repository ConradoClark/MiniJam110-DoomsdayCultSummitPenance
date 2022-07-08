using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Events;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pickup : BaseGameObject
{
    public LichtPhysicsCollisionDetector GroundedDetector;
    public ScriptInput PickupInput;

    private InputAction _pickupAction;
    private LichtPhysics _physics;
    private IEventPublisher<PickupEvents, PickupEventArgs> _pickupEventPublisher;

    public Vector3 PickupOffset;

    public Pickupable PickedUpObject { get; private set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        var playerInput = PlayerInput.GetPlayerByIndex(0);
        _pickupAction = playerInput.actions[PickupInput.ActionName];
        _physics = this.GetLichtPhysics();
        _pickupEventPublisher = this.RegisterAsEventPublisher<PickupEvents, PickupEventArgs>();
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandlePickup());
    }

    private IEnumerable<IEnumerable<Action>> HandlePickup()
    {
        while (isActiveAndEnabled)
        {
            Pickupable pickupable = null;
            var trigger = GroundedDetector.Triggers.FirstOrDefault(t =>
                _physics.TryGetPhysicsObjectByCollider(t.Collider, out var targetObject)
                && targetObject.TryGetCustomObject(out pickupable));

            if (_pickupAction.WasPerformedThisFrame() && trigger.Collider != null && PickedUpObject == null)
            {
                Debug.Log("Picked up " + pickupable.name);
                PickedUpObject = pickupable;
                _pickupEventPublisher.PublishEvent(PickupEvents.OnPickup, new PickupEventArgs
                {
                    Source = this,
                    Target = pickupable
                });

                pickupable.ObjectToPick.SetParent(transform);

                yield return pickupable.ObjectToPick.GetAccessor()
                    .Position.ToPosition(transform.position + PickupOffset)
                    .Over(0.35f)
                    .Easing(EasingYields.EasingFunction.BackEaseInOut)
                    .UsingTimer(GameTimer)
                    .Build();
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
