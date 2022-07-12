using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.UI.LevelCounters;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class SacrificeOnFire : BaseGameObject
    {
        public LichtPhysicsObject PhysicsObject;
        public LichtPhysicsCollisionDetector FireHitBox;
        public Animator Animator;

        private LichtPhysics _physics;
        private LevelResults _levelResults;
        private Player _player;
        private LevelExits _levelExits;

        public IEnumerable<IEnumerable<Action>> Sacrifice()
        {
            Animator.SetTrigger("Sacrifice");
            yield return TimeYields.WaitMilliseconds(GameTimer, 100);
            PhysicsObject.enabled = false;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _physics = this.GetLichtPhysics();
            _levelResults = SceneObject<LevelResults>.Instance();
            _player = SceneObject<Player>.Instance();
            _levelExits = SceneObject<LevelExits>.Instance();

        }

        private void OnEnable()
        {
            DefaultMachinery.AddBasicMachine(HandleSacrifice());
        }

        private IEnumerable<IEnumerable<Action>> HandleSacrifice()
        {
            while (isActiveAndEnabled)
            {
                SacrificialFire fire = null;

                var sacrificed = FireHitBox.Triggers.Any(t =>
                    t.TriggeredHit && _physics.TryGetPhysicsObjectByCollider(t.Collider, out var target) &&
                    target.TryGetCustomObject(out fire));

                if (sacrificed)
                {
                    yield return Sacrifice().AsCoroutine();
                    fire.Exit.Found = true;
                    _levelExits.UpdateCounter();
                    
                    _player.RemoveCultistFromList();

                    yield return _levelResults.Show().AsCoroutine();

                    break;
                }

                yield return TimeYields.WaitOneFrameX;
            }
        }
    }
}
