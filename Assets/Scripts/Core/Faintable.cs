using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Core;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Pooling;
using UnityEngine;

public class Faintable : Resettable
{
    public LichtPhysicsObject PhysicsObject;
    public float FaintDurationInSeconds;
    public Animator Animator;
    public ScriptPrefab FaintEffect;
    public Vector2 FaintEffectOffset;
    public Collider2D FaintCollider;
    public Pickupable Pickupable;

    public bool IsFainted { get; private set; }
    private DurationPoolablePool _faintEffectPool;
    private DurationPoolable _currentFaintEffect;

    protected override void OnAwake()
    {
        base.OnAwake();
        PhysicsObject.AddCustomObject(this);
        _faintEffectPool = SceneObject<DurationPoolableManager>.Instance().GetEffect(FaintEffect);
    }

    private void OnEnable()
    {
        this.ObserveEvent<HitEvents, OnHitEventArgs>(HitEvents.OnHit, OnHit);
    }

    private void OnDisable()
    {
        this.StopObservingEvent<HitEvents, OnHitEventArgs>(HitEvents.OnHit, OnHit);
    }

    private void OnHit(OnHitEventArgs obj)
    {
        if (obj.Target != PhysicsObject) return;

        IsFainted = FaintCollider.enabled = true;
        Animator.SetBool("Faint", true);
        if (_faintEffectPool.TryGetFromPool(out var effect))
        {
            effect.DurationInSeconds = FaintDurationInSeconds;
            effect.Component.transform.position = transform.position + (Vector3)FaintEffectOffset;
            effect.Component.transform.SetParent(transform.parent);
            _currentFaintEffect = effect;
        }

        if (Pickupable != null)
        {
            Pickupable.AllowsPickup = true;
        }
        DefaultMachinery.AddBasicMachine(ComeBackToLife());
    }

    private IEnumerable<IEnumerable<Action>> ComeBackToLife(bool instantly = false)
    {
        if (!instantly) yield return TimeYields.WaitSeconds(GameTimer, FaintDurationInSeconds);

        IsFainted = FaintCollider.enabled = false;
        if (_currentFaintEffect != null && _currentFaintEffect.IsActive)
        {
            _faintEffectPool.Release(_currentFaintEffect);
        }

        Animator.SetBool("Faint", false);

        if (Pickupable != null && Pickupable.IsPickedUp)
        {
            Pickupable.AllowsPickup = false;
            Pickupable.Release(Vector2.zero);
        }
    }

    public override bool PerformReset()
    {
        DefaultMachinery.AddBasicMachine(ComeBackToLife(true));
        return base.PerformReset();
    }
}
