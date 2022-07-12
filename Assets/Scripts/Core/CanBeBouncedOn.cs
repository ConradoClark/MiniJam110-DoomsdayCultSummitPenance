using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;

namespace Assets.Scripts.Core
{
    public class CanBeBouncedOn : BaseGameObject
    {
        public LichtPhysicsObject PhysicsObject;
        public LichtPhysicsCollisionDetector CollisionDetector;
        public Damageable Damageable;

        private void OnEnable()
        {
            PhysicsObject.AddCustomObject(this);
            if (Damageable != null)
            {
                DefaultMachinery.AddBasicMachine(HandleBounceCooldown());
            }
        }

        private void OnDisable()
        {
            PhysicsObject.RemoveCustomObject<CanBeBouncedOn>();
        }

        private IEnumerable<IEnumerable<Action>> HandleBounceCooldown()
        {
            while (isActiveAndEnabled)
            {
                if (!Damageable.CanBeDamaged)
                {
                    yield return TimeYields.WaitMilliseconds(GameTimer, 100);
                    PhysicsObject.RemoveCustomObject<CanBeBouncedOn>();

                    while (!Damageable.CanBeDamaged)
                    {
                        yield return TimeYields.WaitOneFrameX;

                        if (!isActiveAndEnabled) break;
                    }

                    PhysicsObject.AddCustomObject(this);
                }

                yield return TimeYields.WaitOneFrameX;
            }
        }
    }
}
