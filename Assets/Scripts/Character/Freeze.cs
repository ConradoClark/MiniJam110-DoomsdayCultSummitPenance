using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Character
{
    public class Freeze : BaseGameObject
    {
        public LichtPhysicsObject FreezeAura;
        public FreezeAuraEffect FreezeAuraEffect;
        public SpriteRenderer SpriteRenderer;
        public LichtPlatformerMoveController MoveController;
        public LichtPlatformerJumpController JumpController;
        public ScriptInput Action;
        public AudioSource AudioSource;
        public ScriptPrefab Snowflake;
        public Pickup Pickup;

        private bool _blink;
        private MaterialPropertyBlock _propBlock;
        private InputAction _action;

        protected override void OnAwake()
        {
            base.OnAwake();
            var playerInput = PlayerInput.GetPlayerByIndex(0);
            _action = playerInput.actions[Action.ActionName];
            _propBlock = new MaterialPropertyBlock();

            FreezeAuraEffect.enabled = FreezeAura.enabled = false;
        }

        private void OnEnable()
        {
            DefaultMachinery.AddBasicMachine(HandleFreezing());
        }

        private IEnumerable<IEnumerable<Action>> SpawnSnowflakes()
        {
            while (_blink)
            {
                if (Snowflake.Pool.TryGetFromPool(out var obj))
                {
                    obj.Component.transform.position =
                        transform.position + new Vector3(Random.insideUnitCircle.x* 0.3f, Random.insideUnitCircle.x * 0.15f);
                    obj.Component.transform.rotation =
                        Quaternion.AngleAxis(Mathf.Rad2Deg * Random.value, Vector3.forward);
                }

                yield return TimeYields.WaitMilliseconds(GameTimer, 150);
            }
        }

        private IEnumerable<IEnumerable<Action>> Blink()
        {
            SpriteRenderer.GetPropertyBlock(_propBlock);

            var original = _propBlock.GetColor("_Colorize");

            while (_blink)
            {
                var color = new Color(0.2f, 0.2f, Random.value, 0.55f);
                _propBlock.SetColor("_Colorize", color);
                SpriteRenderer.SetPropertyBlock(_propBlock);

                yield return TimeYields.WaitMilliseconds(GameTimer, 100);

                color = new Color(0.3f, 0.45f, 0.95f, 0.85f);
                _propBlock.SetColor("_Colorize", color);
                SpriteRenderer.SetPropertyBlock(_propBlock);

                yield return TimeYields.WaitMilliseconds(GameTimer, 100);
            }
            _propBlock.SetColor("_Colorize", original);
            SpriteRenderer.SetPropertyBlock(_propBlock);
        }

        private IEnumerable<IEnumerable<Action>> HandleFreezing()
        {
            while (isActiveAndEnabled)
            {
                if (_action.WasPerformedThisFrame() && Pickup.PickedUpObject == null)
                {
                    AudioSource.Play();

                    _blink = true;
                    MoveController.BlockMovement(this);
                    JumpController.BlockMovement(this);

                    FreezeAuraEffect.enabled = FreezeAura.enabled = true;
                    DefaultMachinery.AddBasicMachine(Blink());
                    DefaultMachinery.AddBasicMachine(SpawnSnowflakes());
                    yield return TimeYields.WaitSeconds(GameTimer, 0.85f);
                    FreezeAuraEffect.enabled = FreezeAura.enabled = false;
                    _blink = false;
                    yield return TimeYields.WaitOneFrameX;

                    MoveController.UnblockMovement(this);
                    JumpController.UnblockMovement(this);

                }

                yield return TimeYields.WaitOneFrameX;
            }

        }
    }
}
