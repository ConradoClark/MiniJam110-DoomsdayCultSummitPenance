using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class DamageOnTouch : BaseGameObject
{
    public LichtPhysicsObject PhysicsObject;
    public Faintable Faintable;
    public Spawn Spawn;
    public Damageable Damageable;

    protected override void OnAwake()
    {
        base.OnAwake();
        PhysicsObject.AddCustomObject(this);
    }

    private void OnEnable()
    {
        if (Faintable != null)
        {
            DefaultMachinery.AddBasicMachine(HandleActivation());
        }

        if (Damageable != null)
        {
            DefaultMachinery.AddBasicMachine(HandleDamageable());
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleDamageable()
    {
        while (isActiveAndEnabled)
        {
            if (!Damageable.CanBeDamaged)
            {
                PhysicsObject.RemoveCustomObject<DamageOnTouch>();

                while (!Damageable.CanBeDamaged)
                {
                    if (!isActiveAndEnabled) yield break;
                    yield return TimeYields.WaitOneFrameX;
                }

                PhysicsObject.AddCustomObject(this);
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleActivation()
    {
        while (isActiveAndEnabled)
        {
            if (Faintable.IsFainted || Spawn.IsSacrificed)
            {
                PhysicsObject.RemoveCustomObject<DamageOnTouch>();

                if (Spawn.IsSacrificed)
                {
                    yield return TimeYields.WaitOneFrameX;
                    break;
                }

                while (Faintable.IsFainted)
                {
                    yield return TimeYields.WaitOneFrameX;
                }
            }
            else
            {
                if (!PhysicsObject.TryGetCustomObject<DamageOnTouch>(out _))
                {
                    PhysicsObject.AddCustomObject(this);
                }
                while (!Faintable.IsFainted)
                {
                    yield return TimeYields.WaitOneFrameX;
                }
            }
        }
    }
}