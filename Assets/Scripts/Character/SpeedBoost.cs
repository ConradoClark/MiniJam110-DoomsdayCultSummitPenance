using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Character
{
    public class SpeedBoost : BaseGameObject
    {
        public float Speed;
        public LichtPhysicsObject PhysicsObject;
        public SpriteRenderer SpriteRenderer;
        public LichtPlatformerMoveController MoveController;
        public ScriptInput Action;
        public AudioSource AudioSource;
        public ScriptPrefab BoostEffect;
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
        }

        private void OnEnable()
        {
            DefaultMachinery.AddBasicMachine(HandleSpeedBoost());
        }

        private IEnumerable<IEnumerable<Action>> Boost()
        {
            while (_blink)
            {
                if (BoostEffect.Pool.TryGetFromPool(out var obj))
                {
                    obj.Component.transform.position = SpriteRenderer.transform.position;
                }

                yield return TimeYields.WaitMilliseconds(GameTimer, 75);
            }
        }

        private IEnumerable<IEnumerable<Action>> Blink()
        {
            SpriteRenderer.GetPropertyBlock(_propBlock);

            var original = _propBlock.GetColor("_Colorize");

            while (_blink)
            {
                var color = Color.HSVToRGB(Random.value, 1, 0.8f);
                color = new Color(color.r, color.g, color.b, 0.25f);
                _propBlock.SetColor("_Colorize", color);
                SpriteRenderer.SetPropertyBlock(_propBlock);

                yield return TimeYields.WaitMilliseconds(GameTimer, 150);
            }
            _propBlock.SetColor("_Colorize", original);
            SpriteRenderer.SetPropertyBlock(_propBlock);
        }

        private IEnumerable<IEnumerable<Action>> HandleSpeedBoost()
        {
            while (isActiveAndEnabled)
            {
                if (_action.WasPerformedThisFrame() && Pickup.PickedUpObject == null)
                {
                    AudioSource.Play();
                    var latestDirection = MoveController.LatestDirection;

                    _blink = true;

                    DefaultMachinery.AddBasicMachine(Blink());
                    DefaultMachinery.AddBasicMachine(Boost());
                    yield return PhysicsObject.GetSpeedAccessor(new Vector2(latestDirection * Speed, 0))
                        .X
                        .SetTarget(0f)
                        .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
                        .UsingTimer(GameTimer)
                        .Over(1f)
                        .Build();

                    _blink = false;
                    yield return TimeYields.WaitOneFrameX;
                }

                yield return TimeYields.WaitOneFrameX;
            }

        }
    }
}
