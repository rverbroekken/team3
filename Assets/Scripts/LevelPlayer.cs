using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "LevelPlayer", menuName = "Levels/LevelPlayer", order = 1)]
public class LevelPlayer : LevelAbility
{
    public int healAmount = 10;
    public override void Resolve()
    {
  //      Player.instance.Heal(healAmount);
    }
}