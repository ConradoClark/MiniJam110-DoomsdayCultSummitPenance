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
        public TMP_Text Exits;
        public TMP_Text Sacrifices;
        public TMP_Text Time;

        private double _startTime;
        private LevelExits _levelExits;
        private SacrificeCounter _sacrificeCounter;

        protected override void OnAwake()
        {
            base.OnAwake();
            _levelExits = SceneObject<LevelExits>.Instance();
            _confirmExit = PlayerInput.GetPlayerByIndex(0).actions[Confirm.ActionName];
            _startTime = UITimer.TotalElapsedTimeInMilliseconds;
            _sacrificeCounter = SceneObject<SacrificeCounter>.Instance();
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

        private string FormatTime(double ms)
        {
            var minutes = (float) (ms / 1000) / 60;
            var seconds = (float) (ms / 1000) % 60;

            return
                $"00:{Mathf.FloorToInt(minutes).ToString().PadLeft(2, '0')}:{Mathf.FloorToInt(seconds).ToString().PadLeft(2, '0')}";
        }

        public IEnumerable<IEnumerable<Action>> Show()
        {
            var endTime = UITimer.TotalElapsedTimeInMilliseconds - _startTime;
            Time.text = FormatTime(endTime);
            Exits.text = $"{_levelExits.ExitsFound}/{_levelExits.Exits.Length}";
            Sacrifices.text = $"{_sacrificeCounter.SacrificesMade}/{_sacrificeCounter.NumberOfSacrifices}";

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
