using System;
using System.Collections.Generic;
using Assets.Scripts.Character;
using Assets.Scripts.UI;
using Licht.Impl.Orchestration;
using Licht.Unity.Mixins;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class CultistGem : EffectPoolable
{
    public CultistDefinition Current;
    public CultistDefinition CultistType;
    public ScriptInput MousePosInput;
    public ScriptInput MouseClickInput;
    public Vector2 CursorOffset;
    public AudioSource SelectSFX;
    public SpriteRenderer SpriteRenderer;

    private ClickableObjectMixin _clickable;
    private CultistSelectorCursor _cursor;
    private CultistGemText _text;

    protected override void OnAwake()
    {
        base.OnAwake();
        _cursor = SceneObject<CultistSelectorCursor>.Instance();
        _clickable = new ClickableObjectMixinBuilder(this, MousePosInput, MouseClickInput).Build();
        _text = SceneObject<CultistGemText>.Instance();

        if (ActiveOnInitialization && enabled)
        {
            OnActivation();
        }
    }

    public override void OnActivation()
    {
        DefaultMachinery.AddBasicMachine(HandleGem());
    }

    private IEnumerable<IEnumerable<Action>> HandleGem()
    {
        while (isActiveAndEnabled)
        {
            if (_clickable.WasClickedThisFrame() )
            {
                if (_cursor.transform.position != transform.position + (Vector3)CursorOffset) SelectSFX.Play();

                _cursor.transform.position = transform.position + (Vector3)CursorOffset;

                Current.SpriteMaterial = CultistType.SpriteMaterial;
                Current.AbilityIdentifier = CultistType.AbilityIdentifier;
                Current.Ability = CultistType.Ability;
                Current.Name = CultistType.Name;
                Current.Color = CultistType.Color;
                _text.Name.text = CultistType.Name;
                _text.Ability.color = CultistType.Color;
                _text.Ability.text = CultistType.Ability;
            }
            yield return TimeYields.WaitOneFrameX;
        }
    }

    public void Select()
    {
        _cursor.transform.position = transform.position + (Vector3)CursorOffset;

        Current.SpriteMaterial = CultistType.SpriteMaterial;
        Current.AbilityIdentifier = CultistType.AbilityIdentifier;
        Current.Ability = CultistType.Ability;
        Current.Name = CultistType.Name;
        Current.Color = CultistType.Color;
        _text.Name.text = CultistType.Name;
        _text.Ability.color = CultistType.Color;
        _text.Ability.text = CultistType.Ability;
    }

    public override bool IsEffectOver { get; protected set; }
}
