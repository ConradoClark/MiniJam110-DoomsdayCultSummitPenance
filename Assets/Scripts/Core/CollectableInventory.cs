﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectableInventory", menuName = "DoomsdayCult/CollectableInventory", order = 1)]
public class CollectableInventory : ScriptableObject
{
    public ScriptIdentifier Identifier;
    public int Amount;

    public event Action<int> OnCollect;

    public void Collect(int amount)
    {
        Amount += amount;
        OnCollect?.Invoke(amount);
    }
}