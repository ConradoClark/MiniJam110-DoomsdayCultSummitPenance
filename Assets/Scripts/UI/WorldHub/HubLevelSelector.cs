using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HubLevelSelector : BaseUIObject
{
    public HubLevel HubLevel;

    public ScriptInput Confirm;

    private InputAction _confirm;

    private void OnEnable()
    {
        var playerInput = PlayerInput.GetPlayerByIndex(0);
        _confirm = playerInput.actions[Confirm.ActionName];

        DefaultMachinery.AddBasicMachine(HandleConfirm());
    }

    private IEnumerable<IEnumerable<Action>> HandleConfirm()
    {
        while (isActiveAndEnabled)
        {
            if (_confirm.WasPerformedThisFrame())
            {
                if (HubLevel == null)
                {
                    yield return TimeYields.WaitOneFrameX;
                    continue;
                }

                DefaultMachinery.FinalizeWith(() =>
                {
                    SceneManager.LoadScene(HubLevel.SceneName);
                });
            }

            yield return TimeYields.WaitOneFrameX;
        }
        

    }
}
