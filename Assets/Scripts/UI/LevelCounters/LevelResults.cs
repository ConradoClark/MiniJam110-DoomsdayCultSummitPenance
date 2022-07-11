using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI.LevelCounters
{
    public class LevelResults : BaseUIObject
    {
        public ScriptBasicMachinery LichtPhysicsMachinery;
        public ScriptBasicMachinery PostUpdate;
        public SpriteRenderer BG;
        public AudioSource BGMusic;
        public AudioSource Fanfare;

        public AudioSource StatSFX;

        public ScriptInput Confirm;
        private InputAction _confirmExit;
        public Transform[] Objects;
        public TMP_Text SacText;

        protected override void OnAwake()
        {
            base.OnAwake();
            _confirmExit = PlayerInput.GetPlayerByIndex(0).actions[Confirm.ActionName];
        }

        private IEnumerable<IEnumerable<Action>> AnimateBG()
        {
            BG.material.SetColor("_Color", new Color(0,0,0,0));
            yield return BG.GetAccessor().Material("_Color")
                .AsColor()
                .A
                .SetTarget(0.75f)
                .Over(2f)
                .WithStep(0.15f)
                .Easing(EasingYields.EasingFunction.Linear)
                .UsingTimer(UITimer)
                .Build();
        }

        public IEnumerable<IEnumerable<Action>> Show()
        {
            BG.enabled = true;

            DefaultMachinery.AddBasicMachine(AnimateBG());

            BGMusic.Stop();
            Fanfare.Play();

            yield return TimeYields.WaitMilliseconds(UITimer, 2300);
            SacText.enabled = true;

            foreach (var obj in Objects)
            {
                yield return TimeYields.WaitMilliseconds(UITimer, 500);
                obj.gameObject.SetActive(true);
                StatSFX.Play();
            }

            while (!_confirmExit.WasPerformedThisFrame())
            {
                yield return TimeYields.WaitOneFrameX;
            }

            LichtPhysicsMachinery.Machinery.FinalizeWith(() =>
            {
            });

            PostUpdate.Machinery.FinalizeWith(() => { });

            DefaultMachinery.FinalizeWith(() =>
            {
                SceneManager.LoadScene("Scenes/WorldHub", LoadSceneMode.Single);
            });
        }
    }
}
