using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;

public class WalkUntilLedgeOrWall : BaseGameObject
{
    public LichtPhysicsObject PhysicsObject;
    public ScriptIdentifier RightLedge;
    public ScriptIdentifier LeftLedge;
    public LichtPhysicsCollisionDetector WallDetector;
    public Faintable Faintable;

    public float Speed;
    public float InitialDirection;

    private float _direction;

    protected override void OnAwake()
    {
        base.OnAwake();
        _direction = InitialDirection;
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(Walk());
        DefaultMachinery.AddBasicMachine(CheckCollisions());
    }

    private IEnumerable<IEnumerable<Action>> Walk()
    {
        while (isActiveAndEnabled)
        {
            if (!Faintable.IsFainted) PhysicsObject.ApplySpeed(new Vector2(Speed * _direction,0));
            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> CheckCollisions()
    {
        while (isActiveAndEnabled)
        {
            if (!PhysicsObject.GetPhysicsTrigger(RightLedge))
            {
                _direction = -1;
            }

            if (!PhysicsObject.GetPhysicsTrigger(LeftLedge))
            {
                _direction = 1;
            }

            if (WallDetector.Triggers.Any(t => t.Direction.x < -0.9f))
            {
                _direction = -1;
            }

            if (WallDetector.Triggers.Any(t => t.Direction.x > 0.9f))
            {
                _direction = 1;
            }

            yield return TimeYields.WaitMilliseconds(GameTimer, 50);
        }
    }
}
