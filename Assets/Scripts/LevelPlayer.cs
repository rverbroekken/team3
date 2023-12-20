using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// HealPlayer.cs
public class LevelPlayer : LevelAbility
{
    public int healAmount = 10;
    public override void Resolve()
    {
  //      Player.instance.Heal(healAmount);
    }
}