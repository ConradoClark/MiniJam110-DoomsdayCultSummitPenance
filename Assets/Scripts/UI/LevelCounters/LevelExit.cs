using System;
using Licht.Unity.Objects;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelExit", menuName = "DoomsdayCult/LevelExit", order = 1)]
public class LevelExit: ScriptableObject
{
    public string Name;
    public bool Found;
}