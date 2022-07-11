using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Mixins;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class CultistHireButton : BaseUIObject
    {
        public InventoryCounter SorceryCounter;
        public ScriptInput MousePos;
        public ScriptInput MouseClick;

        public Sprite NormalSprite;
        public Sprite SelectedSprite;

        public AudioSource HireSound;
        public AudioSource DenySound;
        public SpriteRenderer Button;
        public Color DisabledColor;

        private ClickableObjectMixin _clickable;
        private CultistListRenderer _cultistListRenderer;

        protected override void OnAwake()
        {
            base.OnAwake();
            _clickable = new ClickableObjectMixinBuilder(this, MousePos, MouseClick).Build();
            _cultistListRenderer = SceneObject<CultistListRenderer>.Instance();
        }

        private void OnEnable()
        {
            DefaultMachinery.AddBasicMachine(HandleClick());
            DefaultMachinery.AddBasicMachine(_clickable.HandleHover(() => UpdateHover(true),
                () => UpdateHover(false)));
            UpdateColor();
        }

        private IEnumerable<IEnumerable<Action>> HandleClick()
        {
            while (isActiveAndEnabled)
            {
                if (_clickable.WasClickedThisFrame())
                {
                    if (SorceryCounter.Inventory.Amount >= 20 && _cultistListRenderer.CanAddCultists())
                    {
                        _cultistListRenderer.AddRandomCultist();
                        SorceryCounter.Inventory.Spend(20);
                        HireSound.Play();
                    }
                    else
                    {
                        DenySound.Play();
                        yield return TimeYields.WaitMilliseconds(UITimer, 200);
                    }

                    UpdateColor();
                }
                yield return TimeYields.WaitOneFrameX;
            }
        }

        private void UpdateHover(bool isHovering)
        {
            if (SorceryCounter.Inventory.Amount < 20)
            {
                Button.sprite = NormalSprite;
                return;
            }

            Button.sprite = isHovering ? SelectedSprite : NormalSprite;
        }

        private void UpdateColor()
        {
            Button.color = SorceryCounter.Inventory.Amount < 20 || !_cultistListRenderer.CanAddCultists() ? DisabledColor : Color.white;
        }
    }
}
