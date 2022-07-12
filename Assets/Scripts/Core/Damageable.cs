using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Character;
using Assets.Scripts.Core;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;

public class Damageable : Resettable
{
    public int HitPoints;
    public Killable Killable;

    public int CurrentHitPoints { get; private set; }

    public enum DamageType
    {
        ThrownObject,
        Touch,
        Bounce,
        Freeze
    }

    public DamageType[] HitByDamageTypes;
    public LichtPhysicsCollisionDetector HitDetector;
    public float DamageCooldownInSeconds;
    private LichtPhysics _physics;

    public bool CanBeDamaged { get; private set; }


    protected override void OnAwake()
    {
        base.OnAwake();
        _physics = this.GetLichtPhysics();
    }

    private void OnEnable()
    {
        CurrentHitPoints = HitPoints;
        CanBeDamaged = CurrentHitPoints > 0;
        DefaultMachinery.AddBasicMachine(DetectDamage());
        DefaultMachinery.AddBasicMachine(SetCustomObject());
        this.ObserveEvent<HitEvents, OnHitEventArgs>(HitEvents.OnHit, OnHit);
    }

    private IEnumerable<IEnumerable<Action>> SetCustomObject()
    {
        yield return TimeYields.WaitOneFrameX;
        if (HitDetector != null && HitDetector.PhysicsObject != null) HitDetector.PhysicsObject.AddCustomObject(this);
    }

    private void OnHit(OnHitEventArgs obj)
    {
        if (!CanBeDamaged || obj.Target != HitDetector.PhysicsObject) return;

        Type damageType = null;
        LichtPhysicsObject source = null;

        var hitDetected = HitDetector.Triggers.Any(t =>
            t.TriggeredHit && _physics.TryGetPhysicsObjectByCollider(t.Collider, out source) &&
            (damageType = GetTypeMatch(true).FirstOrDefault(type => source.HasCustomObjectOfType(type))) != null);

        if (hitDetected)
        {
            CurrentHitPoints -= 1;
            OnDamage?.Invoke(damageType, source);

            if (CurrentHitPoints == 0 && Killable != null)
            {
                DefaultMachinery.AddBasicMachine(Killable.Kill());
            }
        }
    }

    private void OnDisable()
    {
        this.StopObservingEvent<HitEvents, OnHitEventArgs>(HitEvents.OnHit, OnHit);
        if (HitDetector != null && HitDetector.PhysicsObject != null) HitDetector.PhysicsObject.RemoveCustomObject<Damageable>();
    }

    private static readonly Dictionary<DamageType, (Type[] types, bool requiresHit)> DamageTypeMatch = new()
    {
        { DamageType.ThrownObject, (new[] { typeof(Kickable) }, false) },
        { DamageType.Touch, (new[] { typeof(DamageOnTouch) }, false) },
        { DamageType.Freeze, (new[] { typeof(FreezeAuraEffect) }, false) },
        { DamageType.Bounce, (new[] { typeof(BounceOnEnemies) }, true) }
    };

    public event Action<Type, LichtPhysicsObject> OnDamage;

    private IEnumerable<IEnumerable<Action>> DetectDamage()
    {
        while (isActiveAndEnabled)
        {
            Type damageType=null;
            LichtPhysicsObject source = null;
            var hitDetected = HitDetector.Triggers.Any(t =>
                t.TriggeredHit && _physics.TryGetPhysicsObjectByCollider(t.Collider, out source) &&
                (damageType = GetTypeMatch(false).FirstOrDefault(type => source.HasCustomObjectOfType(type))) != null);

            if (hitDetected)
            {
                CurrentHitPoints -= 1;
                OnDamage?.Invoke(damageType, source);

                CanBeDamaged = false;
                if (CurrentHitPoints == 0 && Killable!=null)
                {
                    DefaultMachinery.AddBasicMachine(Killable.Kill());
                    break;
                }

                yield return TimeYields.WaitSeconds(GameTimer, DamageCooldownInSeconds);
                CanBeDamaged = CurrentHitPoints > 0;
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    public void InstantKill()
    {
        CurrentHitPoints = 0;
        OnDamage?.Invoke(typeof(DamageOnTouch), null);
        DefaultMachinery.AddBasicMachine(Killable.Kill());
    }

    private IEnumerable<Type> GetTypeMatch(bool requiresHit)
    {
        return DamageTypeMatch.Where(match => HitByDamageTypes.Contains(match.Key))
            .SelectMany(match => match.Value.requiresHit == requiresHit ? match.Value.types : Array.Empty<Type>());
    }

    public override bool PerformReset()
    {
        CurrentHitPoints = HitPoints;
        CanBeDamaged = CurrentHitPoints > 0;
        return base.PerformReset();
    }
}
