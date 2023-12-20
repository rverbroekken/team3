using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "LevelData", menuName = "Levels/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
   public enum LevelType
    {
        Attack,
        Skill
    }
    public LevelType type;
    public Sprite image;
    public string description;

    // XXX: Hidden in inspector because it will be drawn by custom Editor.
    [HideInInspector]
    public LevelAbility[] onPlayed;
}
