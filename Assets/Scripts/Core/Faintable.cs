using System;
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
    public Kickable Kickable;

    public bool IsFainted { get; private set; }
    public bool IsFrozen { get; set; }
    private DurationPoolablePool _faintEffectPool;
    private DurationPoolable _currentFaintEffect;
    private bool _comingBackToLife;

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

        Faint();
    }

    public void Faint(bool showDizzy = true)
    {
        IsFainted = FaintCollider.enabled = true;
        Animator.SetBool("Faint", true);
        if (showDizzy && _faintEffectPool.TryGetFromPool(out var effect))
        {
            SetEffect(effect);
        }

        if (Pickupable != null)
        {
            Pickupable.AllowsPickup = true;
        }
        DefaultMachinery.AddBasicMachine(ComeBackToLife());
    }

    private void SetEffect(DurationPoolable effect)
    {
        effect.DurationInSeconds = FaintDurationInSeconds;
        effect.Component.transform.position = transform.position + (Vector3)FaintEffectOffset;
        effect.Component.transform.SetParent(transform.parent);
        _currentFaintEffect = effect;
    }

    private IEnumerable<IEnumerable<Action>> ComeBackToLife(bool instantly = false)
    {
        if (IsFrozen)
        {
            yield return TimeYields.WaitOneFrameX;
            yield break;
        }
        if (_comingBackToLife)
        {
            _comingBackToLife = false;
            yield return TimeYields.WaitOneFrameX;
        }

        _comingBackToLife = true;
        wait:

        if (!instantly) yield return TimeYields.WaitSeconds(GameTimer, FaintDurationInSeconds * 0.75f, 
            breakCondition: () => !_comingBackToLife || (Kickable != null && Kickable.WasKickedRecently) || (Pickupable != null && Pickupable.IsReleasing));

        if (_comingBackToLife && (Kickable == null || !Kickable.WasKickedRecently) &&
            (Pickupable == null || !Pickupable.IsReleasing))
        {
            Animator.SetTrigger("Shake");
        }

        if (!instantly) yield return TimeYields.WaitSeconds(GameTimer, FaintDurationInSeconds * 0.25f,
            breakCondition: () => !_comingBackToLife || (Kickable != null && Kickable.WasKickedRecently) || (Pickupable != null && Pickupable.IsReleasing));

        if ((Pickupable?.IsReleasing).GetValueOrDefault() || (Kickable?.WasKickedRecently).GetValueOrDefault())
        {
            Animator.SetTrigger("StopShake");
            if (_currentFaintEffect != null && !_currentFaintEffect.IsActive && _faintEffectPool.TryGetFromPool(out var effect))
            {
                SetEffect(effect);
            }

            while ((Pickupable?.IsReleasing).GetValueOrDefault() || (Kickable?.WasKickedRecently).GetValueOrDefault())
            {
                yield return TimeYields.WaitOneFrameX;
            }

            goto wait;
        }

        if (!_comingBackToLife) yield break;
        
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
