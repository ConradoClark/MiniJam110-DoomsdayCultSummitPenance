using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Pooling;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class EnemyAttachedPoolable : EffectPoolable
    {
        public Kickable Kickable;
        public Damageable Damageable;
        public Pickupable Pickupable;
        public Collider2D EffectCollider;
        public override void OnActivation()
        {
            DefaultMachinery.AddBasicMachine(HandleKill());
        }

        private IEnumerable<IEnumerable<Action>> HandleKill()
        {
            yield return TimeYields.WaitOneFrameX;
            while (isActiveAndEnabled)
            {
                if (Damageable.CurrentHitPoints <= 0)
                {
                    IsEffectOver = true;
                    break;
                }

                yield return TimeYields.WaitOneFrameX;
            }
        }

        public override bool IsEffectOver { get; protected set; }
    }
}
