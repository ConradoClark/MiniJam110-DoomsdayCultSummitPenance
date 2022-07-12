using System;
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
    public AudioSource PickupSFX;

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

    public IEnumerable<IEnumerable<Action>> Release(Vector2 speed)
    {
        if (PickedUpObject == null) yield break;
        PickedUpObject.Release(speed);
        yield return TimeYields.WaitOneFrameX;
        PickedUpObject = null;
    }

    private IEnumerable<IEnumerable<Action>> HandlePickup()
    {
        while (isActiveAndEnabled)
        {
            Pickupable pickupable = null;
            var trigger = GroundedDetector.Triggers.FirstOrDefault(t =>
                _physics.TryGetPhysicsObjectByCollider(t.Collider, out var targetObject)
                && targetObject.TryGetCustomObject(out pickupable));

            if (_pickupAction.IsPressed() && trigger.Collider != null && PickedUpObject == null)
            {
                PickupSFX.Play();

                _pickupEventPublisher.PublishEvent(PickupEvents.OnPickup, new PickupEventArgs
                {
                    Source = this,
                    Target = pickupable
                });

                if (pickupable.RedirectTo != null && pickupable != pickupable.RedirectTo)
                {
                    pickupable = pickupable.RedirectTo;
                }   

                pickupable.ObjectToPick.SetParent(transform);

                yield return pickupable.ObjectToPick.GetAccessor()
                    .LocalPosition.ToPosition(PickupOffset)
                    .Over(0.35f)
                    .Easing(EasingYields.EasingFunction.BackEaseInOut)
                    .UsingTimer(GameTimer)
                    .Build();

                PickedUpObject = pickupable;

                while (pickupable.IsPickedUp)
                {
                    yield return TimeYields.WaitOneFrameX;
                }

                yield return TimeYields.WaitOneFrameX;

                PickedUpObject = null;
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
