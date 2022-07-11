using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UI;
using Licht.Unity.Objects;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class LevelExits : BaseUIObject
{
    public LevelExit[] Exits;
    public UICounter LevelExitCounter;

    private void OnEnable()
    {
        LevelExitCounter.Maximum = Exits.Length;
        LevelExitCounter.Initial = Exits.Count(exit => exit.Found);
        LevelExitCounter.Set(LevelExitCounter.Initial);
    }
}   
