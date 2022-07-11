using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;
using Random = UnityEngine.Random;

public class Collect : BaseGameObject
{
    public CollectableInventory[] Inventories;
    public LichtPhysicsCollisionDetector CollisionDetector;
    public AudioSource CollectSFX;
    
    private LichtPhysics _physics;
    private double _elapsedTime;

    protected override void OnAwake()
    {
        base.OnAwake();
        _physics = this.GetLichtPhysics();
    }

    private void OnEnable()
    {
        _elapsedTime = GameTimer.TotalElapsedTimeInMilliseconds;
        CollectSFX.pitch = 0.85f;
        DefaultMachinery.AddBasicMachine(HandleCollect());
        DefaultMachinery.AddBasicMachine(ResetPitch());
    }


    private IEnumerable<IEnumerable<Action>> ResetPitch()
    {
        while (isActiveAndEnabled)
        {
            if (GameTimer.TotalElapsedTimeInMilliseconds - _elapsedTime > 3000)
            {
                CollectSFX.pitch = 0.85f;
                _elapsedTime = GameTimer.TotalElapsedTimeInMilliseconds;
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleCollect()
    {
        while (isActiveAndEnabled)
        {
            Collectable collectable = null;
            if (CollisionDetector.Triggers.Any(t =>
                    _physics.TryGetPhysicsObjectByCollider(t.Collider, out var physicsObject)
                    && physicsObject.TryGetCustomObject(out collectable)
                    && Inventories.Any(inv=>inv.Identifier == collectable.CollectableType)))
            {
                _elapsedTime = GameTimer.TotalElapsedTimeInMilliseconds;
                CollectSFX.pitch += 0.05f;
                CollectSFX.Play();
                if (CollectSFX.pitch > 1.4f) CollectSFX.pitch = 0.85f;

                yield return collectable.Collect().AsCoroutine();

                var inventory = Inventories.First(inv => inv.Identifier == collectable.CollectableType);
                inventory.Collect(collectable.Amount);

                yield return TimeYields.WaitMilliseconds(GameTimer, 100);
            }
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
