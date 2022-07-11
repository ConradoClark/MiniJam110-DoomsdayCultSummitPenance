using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Enemies;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterKill : Killable
{
    public string HubWorldSceneName;

    public LichtPhysicsObject PhysicsObject;
    public SpriteRenderer Renderer;

    public ScriptBasicMachinery LichtPhysicsMachinery;
    public ScriptBasicMachinery PostUpdate;

    public Damageable Damageable;
    public LichtPhysicsCollisionDetector CollisionDetector;

    private LichtPhysics _physics;
    protected override void OnAwake()
    {
        base.OnAwake();
        _physics = this.GetLichtPhysics();
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleInstantDeath());
    }

    private IEnumerable<IEnumerable<Action>> HandleInstantDeath()
    {
        while (isActiveAndEnabled)
        {
            var triggeredDeath = CollisionDetector.Triggers.Any(t => t.TriggeredHit &&
                                                                     _physics.TryGetPhysicsObjectByCollider(t.Collider,
                                                                         out var target) &&
                                                                     target.TryGetCustomObject<BottomlessPit>(out _));

            if (triggeredDeath)
            {
                Damageable.InstantKill();
                yield break;
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    public override IEnumerable<IEnumerable<Action>> Kill()
    {
        PhysicsObject.enabled = false;

        transform.localRotation = Quaternion.AngleAxis(180, Vector3.forward);

        yield return transform.GetAccessor()
            .Position
            .Y
            .Increase(0.5f)
            .Over(0.25f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(GameTimer)
            .Build();

        yield return transform.GetAccessor()
            .Position
            .Y
            .Decrease(5f)
            .Over(1f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .UsingTimer(GameTimer)
            .Build();

        Renderer.enabled = false;

        LichtPhysicsMachinery.Machinery.FinalizeWith(() =>
        {
        });

        PostUpdate.Machinery.FinalizeWith(() => { });

        DefaultMachinery.FinalizeWith(() =>
        {
            SceneManager.LoadScene(HubWorldSceneName, LoadSceneMode.Single);
        });
    }

    public override bool PerformReset()
    {
        PhysicsObject.enabled = true;
        transform.localRotation = Quaternion.identity;
        Renderer.enabled = true;
        return base.PerformReset();
    }
}
