using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Character
{
    [CreateAssetMenu(fileName = "CultistList", menuName = "DoomsdayCult/CultistList", order = 1)]
    public class CultistList : ScriptableObject
    {
        public List<CultistDefinition> Cultists;
    }
}
