using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

public class Throw : BaseGameObject
{
    public float ThrowStrength;
    public Pickup Pickup;
    public ScriptInput ThrowInput;
    public LichtPlatformerMoveController MoveController;
    public AudioSource ThrowSFX;
    private InputAction _throwAction;

    protected override void OnAwake()
    {
        base.OnAwake();
        var playerInput = PlayerInput.GetPlayerByIndex(0);
        _throwAction = playerInput.actions[ThrowInput.ActionName];
    }
    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleThrow());
    }

    private IEnumerable<IEnumerable<Action>> HandleThrow()
    {
        while (isActiveAndEnabled)
        {
            if (_throwAction.WasPerformedThisFrame() && Pickup.PickedUpObject != null)
            {
                ThrowSFX.Play();
                var latestDirection = MoveController.LatestDirection;
                var latestXSpeed = MoveController.Target.LatestSpeed.x;
                Pickup.Release(new Vector2(ThrowStrength * latestDirection + latestXSpeed, ThrowStrength*0.5f));
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
