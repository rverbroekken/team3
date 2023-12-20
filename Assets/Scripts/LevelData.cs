using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Levels/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    /*
        public enum LevelType
        {
            Attack,
            Skill
        }
        public LevelType type;
    */

    public string description;

    public float minSpawnDelay = 0.25f;
    public float maxSpawnDelay = 1f;

    public BombData[] bombs = new BombData[1];

    public FruitData[] fruit = new FruitData[20];
}
