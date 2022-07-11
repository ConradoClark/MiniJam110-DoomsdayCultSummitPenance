﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Mechanics
{
    [DefaultExecutionOrder(10000)]
    public class Sacrifice : BaseGameObject
    {
        public SacrificeIdentifier Identifier;
        public SacrificeType Type;
        public int Amount;

        public List<Spawn> Spawns;
        public bool IsComplete { get; private set; }

        protected override void OnAwake()
        {
            base.OnAwake();
            Spawns = new List<Spawn>();
            // load spawns, place them at right spot

            var spawnCollection = SceneObject<SpawnCollection>.Instance().Spawns;

            var usedSpawns = spawnCollection.Where(spawn => Identifier.SacrificeSpawns.Contains(spawn.SpawnIdentifier)).ToArray();
            DefaultMachinery.AddBasicMachine(InitialSacrifice(usedSpawns));
        }

        private IEnumerable<IEnumerable<Action>> InitialSacrifice(Spawn[] spawns)
        {
            yield return TimeYields.WaitOneFrameX;

            foreach (var spawn in spawns)
            {
                spawn.transform.position = transform.position + (Vector3)Random.insideUnitCircle * 0.1f;
                MakeSacrifice(spawn);
            }
            CheckSacrifice();
        }

        public void MakeSacrifice(Spawn spawn)
        {
            if (IsComplete || spawn.SacrificeType != Type) return;

            Spawns.Add(spawn);

            if (spawn.SpawnIdentifier != null && !Identifier.SacrificeSpawns.Contains(spawn.SpawnIdentifier))
            {
                Identifier.SacrificeSpawns.Add(spawn.SpawnIdentifier);
            }
            
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
