using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Character
{
    [CreateAssetMenu(fileName = "CultistDefinition", menuName = "DoomsdayCult/CultistDefinition", order = 1)]
    public class CultistDefinition : ScriptableObject
    {
        public Material SpriteMaterial;
        public ScriptIdentifier AbilityIdentifier;
        public string Name;
        public string Ability;
        public Color Color;
    }
}
