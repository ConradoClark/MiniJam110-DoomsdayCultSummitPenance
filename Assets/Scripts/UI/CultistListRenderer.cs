using Assets.Scripts.Character;
using Licht.Unity.Objects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.UI
{
    public class CultistListRenderer : BaseUIObject
    {
        public CultistList CultistList;
        public ScriptPrefab CultistGemPrefab;

        public CultistDefinition[] PossibleCultists;

        public float XDistance;
        public float YDistance;

        private CultistGemPool _gemPool;

        protected override void OnAwake()
        {
            base.OnAwake();
            _gemPool = SceneObject<CultistGemManager>.Instance().GetEffect(CultistGemPrefab);
        }

        public void AddRandomCultist()
        {
            CultistList.Cultists.Add(PossibleCultists[Random.Range(0, PossibleCultists.Length)]);
            _gemPool.ReleaseAll();
            OnEnable();
        }

        public bool CanAddCultists()
        {
            return CultistList.Cultists.Count < 6;
        }

        private void OnEnable()
        {
            if (_gemPool.TryGetManyFromPool(CultistList.Cultists.Count, out var objects))
            {
                for (var i = 0; i < objects.Length; i++)
                {
                    var obj = objects[i];
                    var def = CultistList.Cultists[i];
                    obj.SpriteRenderer.material = def.SpriteMaterial;
                    obj.CultistType = def;
                    obj.transform.position = new Vector3(transform.position.x + i % 3 * XDistance, transform.position.y - ((i / 3) * YDistance));

                    if (i == 0)
                    {
                        obj.Select();
                    }
                }
            }
        }
    }
}
