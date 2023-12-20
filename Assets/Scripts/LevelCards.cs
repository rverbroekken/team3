using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "LevelCards", menuName = "Levels/LevelCards", order = 1)]
public class LevelCards : LevelAbility
{
    public int numCards = 1;
    public override void Resolve()
    {
//        Deck.instance.DrawCards(numCards);
    }
}
