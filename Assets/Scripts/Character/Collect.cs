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

public class Collect : BaseGameObject
{
    public CollectableInventory[] Inventories;
    public LichtPhysicsCollisionDetector CollisionDetector;
    private LichtPhysics _physics;

    protected override void OnAwake()
    {
        base.OnAwake();
        _physics = this.GetLichtPhysics();
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleCollect());
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
                yield return collectable.Collect().AsCoroutine();

                var inventory = Inventories.First(inv => inv.Identifier == collectable.CollectableType);
                inventory.Collect(collectable.Amount);

                yield return TimeYields.WaitMilliseconds(GameTimer, 100);
            }
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
