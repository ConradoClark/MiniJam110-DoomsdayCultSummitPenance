using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class BounceOnGround : BaseGameObject
    {
        public LichtPlatformerJumpController.CustomJumpParams MiniBounceParams;
        public LichtPhysicsObject PhysicsObject;
        public LichtPlatformerMoveController MoveController;
        public LichtPlatformerJumpController JumpController;
        public ScriptIdentifier Grounded;

        public AudioSource Special;
        public AudioSource Jump;

        public SpriteRenderer SpriteRenderer;
        public ScriptInput Action;
        private InputAction _action;

        private bool _blink;
        private MaterialPropertyBlock _propBlock;

        protected override void OnAwake()
        {
            base.OnAwake();
            var playerInput = PlayerInput.GetPlayerByIndex(0);
            _action = playerInput.actions[Action.ActionName];
            _propBlock = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            DefaultMachinery.AddBasicMachine(HandleBouncingOnGround());
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

        private IEnumerable<IEnumerable<Action>> HandleBouncingOnGround()
        {
            while (isActiveAndEnabled)
            {
                if (PhysicsObject.GetPhysicsTrigger(Grounded))
                {
                    if (!_action.IsPressed())
                    {
                        yield return JumpController.ExecuteJump(customParams: MiniBounceParams).AsCoroutine();
                    }
                    else
                    {
                        JumpController.BlockMovement(this);
                        MoveController.BlockMovement(this);
                        var elapsedTime = (float) GameTimer.TotalElapsedTimeInMilliseconds;
                        _blink = true;
                        Special.Play();

                        DefaultMachinery.AddBasicMachine(Blink());

                        var shrink = PhysicsObject.transform.GetAccessor()
                            .LocalScale
                            .Y
                            .Decrease(0.1f)
                            .Over(2f)
                            .UsingTimer(GameTimer)
                            .WithStep(0.01f)
                            .BreakIf(() => !_blink)
                            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
                            .Build();

                        DefaultMachinery.AddBasicMachine(shrink);

                        while (_action.IsPressed())
                        {
                            yield return TimeYields.WaitOneFrameX;
                        }

                        _blink = false;

                        yield return TimeYields.WaitOneFrameX;
                        PhysicsObject.transform.localScale = Vector3.one;

                        MoveController.UnblockMovement(this);

                        var pitch = Jump.pitch;

                        Jump.pitch = 0.75f;
                        Jump.Play();
                        Jump.pitch = pitch;

                        JumpController.UnblockMovement(this);

                        elapsedTime = Mathf.Clamp(((float) GameTimer.TotalElapsedTimeInMilliseconds - elapsedTime) * 0.001f,0,2f);
                        yield return JumpController.ExecuteJump(customParams: new LichtPlatformerJumpController.CustomJumpParams
                        {
                            AccelerationTime = 0,
                            DecelerationTime = elapsedTime * 0.35f,
                            JumpSpeed = elapsedTime * 0.4f,
                            MovementStartEasing= MiniBounceParams.MovementStartEasing,
                            MovementEndEasing = MiniBounceParams.MovementEndEasing,
                            MinJumpDelay = 2f,
                        }).AsCoroutine();
                    }
                    
                }

                yield return TimeYields.WaitOneFrameX;
            }
        }
    }
}
