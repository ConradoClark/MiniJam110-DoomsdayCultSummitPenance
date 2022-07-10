using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class BlinkOnHit : BaseGameObject
{
    public SpriteRenderer SpriteRenderer;
    public Damageable Damageable;

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
        if (Damageable.HitPoints <= 0) return;
        DefaultMachinery.AddBasicMachine(Blink());
    }

    private IEnumerable<IEnumerable<Action>> Blink()
    {
        for (var i = 0; i < Mathf.FloorToInt(Damageable.DamageCooldownInSeconds) * 10; i++)
        {
            SpriteRenderer.enabled = !SpriteRenderer.enabled;
            yield return TimeYields.WaitMilliseconds(GameTimer, 100);
        }

        SpriteRenderer.enabled = true;
    }

}
