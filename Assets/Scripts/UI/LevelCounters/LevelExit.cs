using UnityEngine;

[CreateAssetMenu(fileName = "LevelExit", menuName = "DoomsdayCult/LevelExit", order = 1)]
public class LevelExit: ScriptableObject
{
    public string Name;
    public bool MarkedInWorldHub;
    public bool Found;
}