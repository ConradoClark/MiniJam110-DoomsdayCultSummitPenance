using Licht.Unity.Pooling;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UICounterItem : EffectPoolable
    {
        public SpriteRenderer SpriteRenderer;
        public Sprite FullSprite;
        public Sprite EmptySprite;

        public void SetEmpty()
        {
            if (EmptySprite == null) SpriteRenderer.enabled = false;
            else SpriteRenderer.sprite = EmptySprite;
        }

        public void SetFull()
        {
            SpriteRenderer.enabled = true;
            SpriteRenderer.sprite = FullSprite;
        }

        public override void OnActivation()
        {
            SpriteRenderer.sprite = FullSprite;
            SpriteRenderer.enabled = true;
        }

        public override bool IsEffectOver { get; protected set; }
    }
}
