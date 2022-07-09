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

    private int _currentHitPoints;

    public enum DamageType
    {
        ThrownObject
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
        _currentHitPoints = HitPoints;
        DefaultMachinery.AddBasicMachine(DetectDamage());
    }

    private static readonly Dictionary<DamageType, Type[]> DamageTypeMatch = new()
    {
        { DamageType.ThrownObject, new[] { typeof(Kickable) } }
    };

    private IEnumerable<IEnumerable<Action>> DetectDamage()
    {
        while (isActiveAndEnabled)
        {
            var hitDetected = HitDetector.Triggers.Any(t =>
                t.TriggeredHit && _physics.TryGetPhysicsObjectByCollider(t.Collider, out var physicsObject) &&
                GetTypeMatch().Any(type => physicsObject.HasCustomObjectOfType(type)));

            if (hitDetected)
            {
                _currentHitPoints -= 1;

                if (_currentHitPoints == 0 && Killable!=null)
                {
                    DefaultMachinery.AddBasicMachine(Killable.Kill());
                    break;
                }

                yield return TimeYields.WaitSeconds(GameTimer, DamageCooldownInSeconds);
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<Type> GetTypeMatch()
    {
        return DamageTypeMatch.Where(match => HitByDamageTypes.Contains(match.Key))
            .SelectMany(match => match.Value);
    }

    public override bool PerformReset()
    {
        _currentHitPoints = HitPoints;
        return base.PerformReset();
    }
}
