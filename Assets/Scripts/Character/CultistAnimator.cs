using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class CultistAnimator : BaseGameObject
{
    public ScriptIdentifier Grounded;
    public LichtPhysicsObject PhysicsObject;
    public SpriteRenderer SpriteRenderer;
    public Animator Animator;
    public LichtPlatformerMoveController MoveController;
    public LichtPlatformerJumpController JumpController;

    private void OnEnable()
    {
        this.ObserveEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents, LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnTurn, OnTurn);

        this.ObserveEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents, LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnStartMoving, OnStartMoving);

        this.ObserveEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents, LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnStopMoving, OnStopMoving);

        this.ObserveEvent<LichtPlatformerJumpController.LichtPlatformerJumpEvents, LichtPlatformerJumpController.LichtPlatformerJumpEventArgs>(
            LichtPlatformerJumpController.LichtPlatformerJumpEvents.OnJumpStart, OnJumpStart);
    }

    private void OnDisable()
    {
        this.StopObservingEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents, LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnTurn, OnTurn);

        this.StopObservingEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents, LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnStartMoving, OnStartMoving);

        this.StopObservingEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents, LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnStopMoving, OnStopMoving);

        this.StopObservingEvent<LichtPlatformerJumpController.LichtPlatformerJumpEvents, LichtPlatformerJumpController.LichtPlatformerJumpEventArgs>(
            LichtPlatformerJumpController.LichtPlatformerJumpEvents.OnJumpStart, OnJumpStart);
    }

    private void OnTurn(LichtPlatformerMoveController.LichtPlatformerMoveEventArgs obj)
    {
        if (obj.Source != MoveController) return;
        SpriteRenderer.flipX = obj.Direction < 0;
    }

    private void OnStopMoving(LichtPlatformerMoveController.LichtPlatformerMoveEventArgs obj)
    {
        if (obj.Source != MoveController) return;
        Animator.SetBool("Walking", false);
        OnTurn(obj);
    }

    private void OnStartMoving(LichtPlatformerMoveController.LichtPlatformerMoveEventArgs obj)
    {
        if (obj.Source != MoveController) return;
        Animator.SetBool("Walking", true);
        OnTurn(obj);
    }
    private void OnJumpStart(LichtPlatformerJumpController.LichtPlatformerJumpEventArgs obj)
    {
        if (obj.Source != JumpController) return;

        Animator.SetBool("Jumping", true);
        DefaultMachinery.AddBasicMachine(WaitForGrounded());
    }

    private IEnumerable<IEnumerable<Action>> WaitForGrounded()
    {
        yield return TimeYields.WaitMilliseconds(GameTimer, 100);
        while (!PhysicsObject.GetPhysicsTrigger(Grounded))
        {
            yield return TimeYields.WaitOneFrameX;
        }
        Animator.SetBool("Jumping", false);
    }

}
