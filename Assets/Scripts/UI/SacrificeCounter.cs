using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Mechanics;
using Assets.Scripts.UI;
using Licht.Impl.Events;
using Licht.Unity.Objects;
using UnityEngine;

public class SacrificeCounter : BaseUIObject
{
    public UICounter UICounter;
    public int NumberOfSacrifices;

    protected override void OnAwake()
    {
        base.OnAwake();
        UICounter.Maximum = NumberOfSacrifices;
    }

    private void OnEnable()
    {
        this.ObserveEvent(Sacrifice.SacrificeEvents.OnSacrifice, OnSacrifice);
    }

    private void OnSacrifice()
    {
        UICounter.Increase(1);
    }

    private void OnDisable()
    {
        this.StopObservingEvent(Sacrifice.SacrificeEvents.OnSacrifice, OnSacrifice);
    }
}
