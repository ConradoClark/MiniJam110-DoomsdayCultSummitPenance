using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Core
{
    [DefaultExecutionOrder(-1000)]
    public class SpawnCollection : BaseGameObject
    {
        public HashSet<Spawn> Spawns { get; private set; }

        protected override void OnAwake()
        {
            base.OnAwake();
            Spawns = new HashSet<Spawn>();
        }

        public void Add(Spawn spawn)
        {
            if (Spawns.Contains(spawn)) return;

            Spawns.Add(spawn);
        }

        private void OnDisable()
        {
            Spawns.Clear();
        }
    }
}
