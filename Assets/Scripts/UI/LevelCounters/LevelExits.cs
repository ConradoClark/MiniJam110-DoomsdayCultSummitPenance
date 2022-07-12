using System.Linq;
using Assets.Scripts.UI;
using Licht.Unity.Objects;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class LevelExits : BaseUIObject
{
    public LevelExit[] Exits;
    public UICounter LevelExitCounter;
    public int ExitsFound;

    private void OnEnable()
    {
        LevelExitCounter.Maximum = Exits.Length;
        ExitsFound = Exits.Count(exit => exit.Found);
        LevelExitCounter.Initial = ExitsFound;
        LevelExitCounter.Set(ExitsFound);
    }

    public void UpdateCounter()
    {
        ExitsFound = Exits.Count(exit => exit.Found);
        LevelExitCounter.Set(ExitsFound);
    }
}   
