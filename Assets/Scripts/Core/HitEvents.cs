using Licht.Unity.Physics;

namespace Assets.Scripts.Core
{
    public enum HitEvents
    {
        OnHit
    }

    public class OnHitEventArgs
    {
        public LichtPhysicsObject Source;
        public LichtPhysicsObject Target;
    }
}
