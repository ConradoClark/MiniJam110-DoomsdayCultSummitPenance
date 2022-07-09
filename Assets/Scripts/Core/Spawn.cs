using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Mechanics;
using Licht.Unity.Objects;
using Licht.Unity.Physics;

public class Spawn : BaseGameObject
{
    public LichtPhysicsObject PhysicsObject;
    public SacrificeType SacrificeType;

    protected override void OnAwake()
    {
        base.OnAwake();
        PhysicsObject.AddCustomObject(this);
    }
}

