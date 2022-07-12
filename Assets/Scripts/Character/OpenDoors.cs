using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Mechanics;
using Licht.Impl.Orchestration;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Physics.CollisionDetection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Character
{
    public class OpenDoors : BaseGameObject
    {
        public LichtPhysicsCollisionDetector HitBox;
        public ScriptInput OpenInput;

        public LichtPlatformerMoveController MoveController;
        public LichtPlatformerJumpController JumpController;

        public SpriteRenderer SpriteRenderer;

        private InputAction _openAction;
        private LichtPhysics _physics;
        private Player _player;
        private CameraFollow _cameraFollow;
        protected override void OnAwake()
        {
            base.OnAwake();
            _physics = this.GetLichtPhysics();
            var playerInput = PlayerInput.GetPlayerByIndex(0);
            _openAction = playerInput.actions[OpenInput.ActionName];
            _player = SceneObject<Player>.Instance();
            _cameraFollow = SceneObject<CameraFollow>.Instance();
        }

        private void OnEnable()
        {
            DefaultMachinery.AddBasicMachine(HandleDoors());
        }

        private IEnumerable<IEnumerable<Action>> HandleDoors()
        {
            while (isActiveAndEnabled)
            {
                Door door = null;
                var triggered = HitBox.Triggers.Any(t =>
                    t.TriggeredHit && _physics.TryGetPhysicsObjectByCollider(t.Collider, out var target) &&
                    target.TryGetCustomObject(out door));

                if (triggered && _openAction.WasPerformedThisFrame())
                {
                    MoveController.BlockMovement(this);
                    JumpController.BlockMovement(this);
                    door.Open();

                    yield return TimeYields.WaitMilliseconds(GameTimer, 50);
                    SpriteRenderer.enabled = false;

                    yield return TimeYields.WaitMilliseconds(GameTimer, 150);

                    _player.transform.position = door.TargetDoor.transform.position;

                    _cameraFollow.MinX = door.TargetDoor.CameraMinX;
                    _cameraFollow.MaxX = door.TargetDoor.CameraMaxX;
                    _cameraFollow.MinY = door.TargetDoor.CameraMinY;
                    _cameraFollow.MaxY = door.TargetDoor.CameraMaxY;
                    door.TargetDoor.Open();

                    yield return TimeYields.WaitMilliseconds(GameTimer, 150);
                    SpriteRenderer.enabled = true;

                    MoveController.UnblockMovement(this);
                    JumpController.UnblockMovement(this);
                }

                yield return TimeYields.WaitOneFrameX;
            }
        }
    }
}
