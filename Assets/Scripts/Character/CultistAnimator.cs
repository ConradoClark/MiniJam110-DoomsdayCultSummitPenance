using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Unity.CharacterControllers;
using UnityEngine;

public class CultistAnimator : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public LichtPlatformerMoveController MoveController;

    private void OnEnable()
    {
        this.ObserveEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents, LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnTurn, OnTurn);

        this.ObserveEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents, LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnStartMoving, OnTurn);
    }
    private void OnDisable()
    {
        this.StopObservingEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents, LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnTurn, OnTurn);

        this.ObserveEvent<LichtPlatformerMoveController.LichtPlatformerMoveEvents, LichtPlatformerMoveController.LichtPlatformerMoveEventArgs>(
            LichtPlatformerMoveController.LichtPlatformerMoveEvents.OnStartMoving, OnTurn);
    }

    private void OnTurn(LichtPlatformerMoveController.LichtPlatformerMoveEventArgs obj)
    {
        if (obj.Source != MoveController) return;

        SpriteRenderer.flipX = obj.Direction < 0;
    }


}
