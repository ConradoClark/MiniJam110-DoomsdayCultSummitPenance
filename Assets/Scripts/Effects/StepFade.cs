using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Effects
{

    public class StepFade : BaseGameObject
    {
        public SpriteRenderer SpriteRenderer;
        private void OnEnable()
        {
            SpriteRenderer.material.SetColor("_Color", Color.white);

            DefaultMachinery.AddBasicMachine(SpriteRenderer.GetAccessor()
                .Material("_Color")
                .AsColor()
                .A
                .SetTarget(0)
                .WithStep(0.2f)
                .Over(0.5f)
                .UsingTimer(GameTimer)
                .Build());
        }
    }

    
}
