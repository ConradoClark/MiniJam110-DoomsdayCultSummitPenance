using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Character;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class Player : BaseGameObject
{
    public CultistList CultistList;
    public CultistDefinition CurrentCultist;
    public SpriteRenderer SpriteRenderer;
    public SpriteRenderer GemSpriteRenderer;

    protected override void OnAwake()
    {
        base.OnAwake();
        SpriteRenderer.material = CurrentCultist.SpriteMaterial;
        GemSpriteRenderer.material = CurrentCultist.SpriteMaterial;
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
