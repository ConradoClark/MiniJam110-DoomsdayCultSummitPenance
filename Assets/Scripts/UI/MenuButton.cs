using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UI;
using Licht.Impl.Orchestration;
using Licht.Unity.Mixins;
using Licht.Unity.Objects;
using UnityEngine;

public class MenuButton : BaseUIObject
{
    public SpriteRenderer ButtonSpriteRenderer;
    public Sprite ButtonSprite;
    public Sprite SelectedButtonSprite;
    private ClickableObjectMixin _clickableObject;
    public ScriptInput MousePosInput;
    public ScriptInput MouseClickInput;
    public MenuButtonFunction MenuButtonFunction;

    protected override void OnAwake()
    {
        base.OnAwake();
        _clickableObject = new ClickableObjectMixinBuilder(this, MousePosInput, MouseClickInput).Build();
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(_clickableObject.HandleHover(() => ButtonSpriteRenderer.sprite = SelectedButtonSprite,
            () => ButtonSpriteRenderer.sprite = ButtonSprite));

        DefaultMachinery.AddBasicMachine(HandleClick());
    }

    private IEnumerable<IEnumerable<Action>> HandleClick()
    {
        while (isActiveAndEnabled)
        {
            if (_clickableObject.WasClickedThisFrame() && MenuButtonFunction!=null)
            {
                yield return MenuButtonFunction.Execute().AsCoroutine();
            }
            
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
