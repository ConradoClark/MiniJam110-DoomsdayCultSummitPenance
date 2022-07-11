using System.Collections.Generic;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Mechanics
{
    [CreateAssetMenu(fileName = "SacrificeIdentifier", menuName = "DoomsdayCult/SacrificeIdentifier", order = 1)]
    public class SacrificeIdentifier : ScriptableObject
    {
        public List<ScriptIdentifier> SacrificeSpawns = new();
        public List<ScriptIdentifier> SacrificeCultists = new();
    }
}
