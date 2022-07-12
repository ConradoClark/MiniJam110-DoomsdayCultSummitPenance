using Licht.Unity.Objects;
using Licht.Unity.Physics;

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
