using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Physics;
using UnityEngine;

public class Kickable : MonoBehaviour
{
    public LichtPhysicsObject PhysicsObject;
    public Faintable Faintable;
    public bool AlwaysKickable;

    private void Awake()
    {
        PhysicsObject.AddCustomObject(this);
    }

    public bool IsKickable()
    {
        return AlwaysKickable || Faintable != null && Faintable.IsFainted;
    }
}
