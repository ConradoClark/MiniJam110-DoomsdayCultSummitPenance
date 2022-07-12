using System;
using System.Collections.Generic;
using Assets.Scripts.Character;
using Licht.Impl.Orchestration;
using Licht.Unity.CharacterControllers;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class Player : BaseGameObject
{
    public ScriptIdentifier Grounded;
    public LichtPhysicsObject PhysicsObject;
    public CultistList CultistList;
    public CultistDefinition CurrentCultist;
    public SpriteRenderer SpriteRenderer;
    public SpriteRenderer GemSpriteRenderer;

    public BounceOnGround BounceOnGround;
    public BounceOnEnemies BounceOnEnemies;
    public ScriptIdentifier BouncyAbility;
    public ScriptIdentifier FrostyAbility;
    public ScriptIdentifier SpeedyAbility;

    public LichtPlatformerMoveController MoveController;

    public SpeedBoost SpeedBoost;
    public Freeze Freeze;

    protected override void OnAwake()
    {
        base.OnAwake();
        SpriteRenderer.material = CurrentCultist.SpriteMaterial;
        GemSpriteRenderer.material = CurrentCultist.SpriteMaterial;
        SetAbiliies();
    }

    private void SetAbiliies()
    {
        if (CurrentCultist.AbilityIdentifier == BouncyAbility)
        {
            BounceOnGround.enabled = true;
            BounceOnEnemies.BounceParams.JumpSpeed *= 1.1f;
        }

        else if (CurrentCultist.AbilityIdentifier == FrostyAbility)
        {
            DefaultMachinery.AddBasicMachine(Slippery());
            Freeze.enabled = true;
        }

        else if (CurrentCultist.AbilityIdentifier == SpeedyAbility)
        {
            SpeedBoost.enabled = true;
            MoveController.SpeedMultiplier = 1.25f;
        }
    }

    private IEnumerable<IEnumerable<Action>> Slippery()
    {
        var original = MoveController.DecelerationTime;
        while (isActiveAndEnabled)
        {
            MoveController.DecelerationTime = PhysicsObject.GetPhysicsTrigger(Grounded) ? 0.75f : original;

            yield return TimeYields.WaitOneFrameX;
        }
    }

    public void RemoveCultistFromList()
    {
        for(var i =0;i<CultistList.Cultists.Count;i++)
        {
            var cultist = CultistList.Cultists[i];
            if (cultist.SpriteMaterial == CurrentCultist.SpriteMaterial)
            {
                CultistList.Cultists.RemoveAt(i);
                break;
            }
        }
    }
}
