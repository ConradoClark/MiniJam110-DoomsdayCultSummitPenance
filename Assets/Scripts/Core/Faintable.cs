using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Core;
using Licht.Impl.Events;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class Faintable : BaseGameObject
{
    public LichtPhysicsObject PhysicsObject;
    public float FaintDurationInSeconds;
    public Animator Animator;
    public ScriptPrefab FaintEffect;
    public Vector2 FaintEffectOffset;
    public bool IsFainted { get; private set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        PhysicsObject.AddCustomObject(this);
    }

    private void OnEnable()
    {
        this.ObserveEvent<OnHitEvents, OnHitEventArgs>(OnHitEvents.OnHit, OnHit);
    }

    private void OnDisable()
    {
        this.StopObservingEvent<OnHitEvents, OnHitEventArgs>(OnHitEvents.OnHit, OnHit);
    }

    private void OnHit(OnHitEventArgs obj)
    {
        if (obj.Target != PhysicsObject) return;

        IsFainted = true;
        Animator.SetTrigger("Faint");
        if (FaintEffect.Pool.TryGetFromPool(out var effect))
        {
            effect.Component.transform.position = transform.position + (Vector3) FaintEffectOffset;
        }
    }
}
