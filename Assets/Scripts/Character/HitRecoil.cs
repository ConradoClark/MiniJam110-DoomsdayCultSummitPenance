using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class HitRecoil : BaseGameObject
{ 
    public LichtPhysicsObject PhysicsObject;
    public Damageable Damageable;
    public Vector2 RecoilSpeed;
    public EasingYields.EasingFunction RecoilEasing;
    public float RecoilTimeInSeconds;

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
        if (damageType != typeof(DamageOnTouch) || source==null) return;

        DefaultMachinery.AddBasicMachine(Recoil(source));
    }

    private IEnumerable<IEnumerable<Action>> Recoil(Component source)
    {
        var direction = source.transform.position.x > transform.position.x ? -1f : 1;

        yield return PhysicsObject.GetSpeedAccessor(new Vector2(RecoilSpeed.x * direction, RecoilSpeed.y))
            .ToSpeed(Vector2.zero)
            .Over(RecoilTimeInSeconds)
            .Easing(RecoilEasing)
            .UsingTimer(GameTimer)
            .Build();
    }
}
