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
    public int levelTimeInSeconds = 180;
    public float minSpawnDelay = 0.25f;
    public float maxSpawnDelay = 1f;

    [Range(-12f, 12f)] public float minSpawnX = -5f;
    [Range(-12f, 12f)] public float maxSpawnX = 5f;

    public BombData[] bombs = new BombData[1];

    public FruitData[] fruit = new FruitData[20];
}
