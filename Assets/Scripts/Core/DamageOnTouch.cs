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

    protected override void OnAwake()
    {
        base.OnAwake();
        PhysicsObject.AddCustomObject(this);
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleActivation());
    }

    private IEnumerable<IEnumerable<Action>> HandleActivation()
    {
        while (isActiveAndEnabled)
        {
            if (Faintable.IsFainted)
            {
                PhysicsObject.RemoveCustomObject<DamageOnTouch>();
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