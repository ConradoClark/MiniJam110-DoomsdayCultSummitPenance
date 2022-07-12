using Licht.Unity.Objects;
using Licht.Unity.Physics;

namespace Assets.Scripts.Enemies
{
    public class BottomlessPit : BaseGameObject
    {
        public LichtPhysicsObject PhysicsObject;

        protected override void OnAwake()
        {
            base.OnAwake();
            PhysicsObject.AddCustomObject(this);
        }
    }
}
