using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;

public class SacrificialPit : BaseGameObject
{
    public LockedBehindSacrifice Trigger;
    public LichtPhysicsCollisionDetector PitDetector;
    public AudioSource SacrificeSFX;

    private LichtPhysics _physics;

    protected override void OnAwake()
    {
        base.OnAwake();
        _physics = this.GetLichtPhysics();
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandlePit());
    }

    private IEnumerable<IEnumerable<Action>> HandlePit()
    {
        while (isActiveAndEnabled)
        {
            Spawn spawn = null;
            var detected = PitDetector.Triggers.Any(t=>t.TriggeredHit &&
                                                        _physics.TryGetPhysicsObjectByCollider(t.Collider, out var target)
                                                        && target.TryGetCustomObject(out spawn));

            if (detected)
            {
                var sacrifice =
                    Trigger.Sacrifices.FirstOrDefault(sac => !sac.IsComplete && sac.Type == spawn.SacrificeType);

                if (sacrifice != null && !sacrifice.Spawns.Contains(spawn))
                {
                    sacrifice.MakeSacrifice(spawn);
                    if (SacrificeSFX!=null) SacrificeSFX.Play();
                }
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
