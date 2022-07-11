using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UI;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PlayerHitPoints : BaseGameObject
{
    public UICounter HealthCounter;
    public Damageable Damageable;

    public int HitPoints = 3;
    protected override void OnAwake()
    {
        base.OnAwake();
        HealthCounter.Maximum = HealthCounter.Initial = Damageable.HitPoints = HitPoints;
    }

    private void OnEnable()
    {
        Damageable.OnDamage += Damageable_OnDamage;
    }

    private void OnDisable()
    {
        Damageable.OnDamage -= Damageable_OnDamage;
    }

    private void Damageable_OnDamage(Type damageType, LichtPhysicsObject source)
    {
        HealthCounter.Set(Damageable.CurrentHitPoints);
    }

}
