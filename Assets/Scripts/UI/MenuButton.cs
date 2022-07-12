using System;
using System.Collections.Generic;
using Assets.Scripts.UI;
using Licht.Impl.Orchestration;
using Licht.Unity.Mixins;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuButton : BaseUIObject
{
    public SpriteRenderer ButtonSpriteRenderer;
    public Sprite ButtonSprite;
    public Sprite SelectedButtonSprite;
    private ClickableObjectMixin _clickableObject;
    public ScriptInput MousePosInput;
    public ScriptInput MouseClickInput;
    public MenuButtonFunction MenuButtonFunction;
    public ScriptInput ConfirmInput;
    private InputAction _confirm;
    protected override void OnAwake()
    {
        base.OnAwake();
        _confirm = PlayerInput.GetPlayerByIndex(0).actions[ConfirmInput.ActionName];
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
            if ((_clickableObject.WasClickedThisFrame() || _confirm.WasPerformedThisFrame()) && MenuButtonFunction!=null)
            {
                yield return MenuButtonFunction.Execute().AsCoroutine();
            }
            
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
