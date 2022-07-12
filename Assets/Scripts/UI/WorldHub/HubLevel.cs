using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UI.WorldHub;
using Licht.Impl.Orchestration;
using Licht.Unity.Mixins;
using Licht.Unity.Objects;
using UnityEngine;

public class HubLevel : BaseUIObject
{
    public string SceneName;
    public LevelExit[] Requirements;

    public ScriptInput MousePosInput;
    public ScriptInput MouseClickInput;
    public AudioSource SelectSFX;
    public string LevelName;

    private ClickableObjectMixin _clickable;
    private HubLevelSelector _hubLevelSelector;
    private HubLevelText _hubLevelText;

    protected override void OnAwake()
    {
        base.OnAwake();
        _clickable = new ClickableObjectMixinBuilder(this, MousePosInput, MouseClickInput).Build();
        _hubLevelSelector = SceneObject<HubLevelSelector>.Instance();
        _hubLevelText = SceneObject<HubLevelText>.Instance();
    }

    private void OnEnable()
    {
        if (Requirements.Any(r => !r.Found))
        {
            gameObject.SetActive(false);
        }
        else
        {
            if (Requirements.Any(r => !r.MarkedInWorldHub))
            {
                foreach (var req in Requirements)
                {
                    req.MarkedInWorldHub = true;
                }
                SelectLevel();
            }
            DefaultMachinery.AddBasicMachine(HandleClick());
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleClick()
    {
        while (isActiveAndEnabled)
        {
            if (_clickable.WasClickedThisFrame() && _hubLevelSelector.HubLevel != this)
            {
                SelectSFX.Play();
                SelectLevel();
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private void SelectLevel()
    {
        _hubLevelText.TextComponent.text = LevelName;
        _hubLevelSelector.HubLevel = this;
        _hubLevelSelector.transform.position = transform.position;
    }
}
