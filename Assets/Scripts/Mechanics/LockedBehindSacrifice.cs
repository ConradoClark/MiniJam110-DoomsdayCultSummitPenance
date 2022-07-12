using System;
using System.Linq;
using Assets.Scripts.Mechanics;
using Licht.Unity.Objects;
using UnityEngine;

[DefaultExecutionOrder(9000)]
public class LockedBehindSacrifice : BaseGameObject
{
    public Sacrifice[] Sacrifices;
    public bool IsComplete => Sacrifices.All(sac => sac.IsComplete);

    public event Action OnComplete;

    private void OnEnable()
    {
        foreach (var sac in Sacrifices)
        {
            sac.OnSacrifice += CheckSacrifice;
        }
    }

    private void OnDisable()
    {
        foreach (var sac in Sacrifices)
        {
            if (sac == null) continue;
            sac.OnSacrifice -= CheckSacrifice;
        }
    }

    public void CheckSacrifice()
    {
        if (IsComplete) OnComplete?.Invoke();
    }
}
