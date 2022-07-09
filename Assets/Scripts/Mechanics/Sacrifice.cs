using System;
using System.Collections.Generic;
using Licht.Unity.Objects;

namespace Assets.Scripts.Mechanics
{
    public class Sacrifice : BaseGameObject
    {
        public ScriptIdentifier[] SpawnIdentifiers;
        public SacrificeType Type;
        public int Amount;

        public List<Spawn> Spawns;
        public bool IsComplete { get; private set; }

        protected override void OnAwake()
        {
            base.OnAwake();
            Spawns = new List<Spawn>();
            // load spawns, place them at right spot
            CheckSacrifice();
        }

        public void MakeSacrifice(Spawn spawn)
        {
            if (IsComplete || spawn.SacrificeType != Type) return;

            Spawns.Add(spawn);
            spawn.MarkAsSacrifice();
            CheckSacrifice();
            OnSacrifice?.Invoke();
        }

        private void CheckSacrifice()
        {
            IsComplete = Spawns.Count == Amount;
        }

        public event Action OnSacrifice;

    }
}
