using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;

public class UnaffectedByForce : BaseGameObject
{
    public LichtPhysicsObject PhysicsObject;
    public ScriptIdentifier Gravity;

    protected override void OnAwake()
    {
        base.OnAwake();
        DefaultMachinery.AddBasicMachine(BlockForce());
    }

    private IEnumerable<IEnumerable<Action>> BlockForce()
    {
        yield return TimeYields.WaitOneFrameX;
        this.GetLichtPhysics().BlockCustomPhysicsForceForObject(this, PhysicsObject, Gravity.Name);
    }
}
