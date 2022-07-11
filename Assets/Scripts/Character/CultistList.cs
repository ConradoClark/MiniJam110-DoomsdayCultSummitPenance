using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Character
{
    [CreateAssetMenu(fileName = "CultistList", menuName = "DoomsdayCult/CultistList", order = 1)]
    public class CultistList : ScriptableObject
    {
        public List<CultistDefinition> Cultists;
    }
}
