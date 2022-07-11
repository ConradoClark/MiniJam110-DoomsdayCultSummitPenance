using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class SacrificialFire : BaseGameObject
{
    public LevelExit Exit;
    public LichtPhysicsObject PhysicsObject;

    protected override void OnAwake()
    {
        base.OnAwake();
        PhysicsObject.AddCustomObject(this);
    }
}
