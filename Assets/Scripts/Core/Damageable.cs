using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core;
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
        Touch
    }

    public DamageType[] HitByDamageTypes;
    public LichtPhysicsCollisionDetector HitDetector;
    public float DamageCooldownInSeconds;
    private LichtPhysics _physics;

    protected override void OnAwake()
    {
        base.OnAwake();
        _physics = this.GetLichtPhysics();
    }

    private void OnEnable()
    {
        CurrentHitPoints = HitPoints;
        DefaultMachinery.AddBasicMachine(DetectDamage());
    }

    private static readonly Dictionary<DamageType, Type[]> DamageTypeMatch = new()
    {
        { DamageType.ThrownObject, new[] { typeof(Kickable) } },
        { DamageType.Touch, new[] { typeof(DamageOnTouch) } }
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
                (damageType = GetTypeMatch().FirstOrDefault(type => source.HasCustomObjectOfType(type))) != null);

            if (hitDetected)
            {
                CurrentHitPoints -= 1;
                OnDamage?.Invoke(damageType, source);

                if (CurrentHitPoints == 0 && Killable!=null)
                {
                    DefaultMachinery.AddBasicMachine(Killable.Kill());
                    break;
                }

                yield return TimeYields.WaitSeconds(GameTimer, DamageCooldownInSeconds);
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

    private IEnumerable<Type> GetTypeMatch()
    {
        return DamageTypeMatch.Where(match => HitByDamageTypes.Contains(match.Key))
            .SelectMany(match => match.Value);
    }

    public override bool PerformReset()
    {
        CurrentHitPoints = HitPoints;
        return base.PerformReset();
    }
}
