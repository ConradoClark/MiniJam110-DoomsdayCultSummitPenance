using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;

public class Collectable : BaseGameObject
{
    public ScriptIdentifier CollectableType;
    public LichtPhysicsObject PhysicsObject;
    public ScriptPrefab ItemShine;
    public int Amount = 1;

    protected override void OnAwake()
    {
        base.OnAwake();
        PhysicsObject.AddCustomObject(this);
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(Wander());
    }

    private IEnumerable<IEnumerable<Action>> Shine()
    {
        if (ItemShine.Pool.TryGetFromPool(out var effect))
        {
            effect.Component.transform.position = transform.position;
        }

        yield break;
    }

    private IEnumerable<IEnumerable<Action>> Wander()
    {
        while (isActiveAndEnabled)
        {
            yield return transform.GetAccessor()
                .Position
                .Y
                .Increase(0.15f)
                .Over(1f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(GameTimer)
                .Build();

            yield return transform.GetAccessor()
                .Position
                .Y
                .Decrease(0.15f)
                .Over(1f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                .UsingTimer(GameTimer)
                .Build();
        }
    }

    public IEnumerable<IEnumerable<Action>> Collect()
    {
        yield return Shine().AsCoroutine();
        gameObject.SetActive(false);
    }
}
