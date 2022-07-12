using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;

namespace Assets.Scripts.Character
{
    public class FreezeAuraEffect : BaseGameObject
    {
        public LichtPhysicsCollisionDetector FreezeDetector;
        public LichtPhysicsObject FreezeAura;
        public ScriptPrefab IceCube;

        private LichtPhysics _physics;
        private EnemyAttachedPoolablePool _freezePool;
        protected override void OnAwake()
        {
            base.OnAwake();
            _physics = this.GetLichtPhysics();
            _freezePool = SceneObject<EnemyAttachedPoolableManager>.Instance().GetEffect(IceCube);
        }

        private void OnEnable()
        {
            FreezeAura.AddCustomObject(this);
            DefaultMachinery.AddBasicMachine(HandleFreezeAura());
        }

        private void OnDisable()
        {
            FreezeAura.RemoveCustomObject<FreezeAuraEffect>();
        }

        private IEnumerable<IEnumerable<Action>> HandleFreezeAura()
        {
            while (isActiveAndEnabled)
            {
                Faintable faintable = null;
                Damageable damageable = null;
                Pickupable pickupable = null;
                LichtPhysicsObject target = null;

                var triggered = FreezeDetector.Triggers.Any(f =>
                    f.TriggeredHit && _physics.TryGetPhysicsObjectByCollider(f.Collider, out target) &&
                    target.TryGetCustomObject(out faintable) && target.TryGetCustomObject(out damageable)
                    && target.TryGetCustomObject(out pickupable));

                if (triggered && !faintable.IsFrozen)
                {
                    faintable.Faint(false);
                    if (_freezePool.TryGetFromPool(out var obj))
                    {
                        obj.transform.position = faintable.transform.position;
                        obj.transform.SetParent(faintable.transform);
                        obj.Damageable = damageable;
                        obj.Pickupable.RedirectTo = pickupable;
                        target.AdditionalColliders.Add(obj.EffectCollider);

                        if (target.TryGetCustomObject(out Kickable kickable))
                        {
                            obj.Kickable.RedirectTo = kickable;
                        }
                    }
                    faintable.IsFrozen = true;
                }

                yield return TimeYields.WaitOneFrameX;
            }
        }
    }
}
