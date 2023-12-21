using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class LevelState
{
    public int levelIdx;
    public int numStars;
}

[Serializable]
public class PlayerData
{
    public int currentLevelIdx = 0;
    public int piggybankLevelIdx = 0;
    public int piggybankLevelState = 0;
    public List<LevelState> levelStates = new List<LevelState>();

    public const int dummyScoreForNextLevel = 50;

    public void LevelWon(int levelIdx, int numStars, int numDummyMatches)
    {
        var levelState = new LevelState();
        levelState.levelIdx = levelIdx;
        levelState.numStars = numStars;
        levelStates.Add(levelState);
        currentLevelIdx = levelIdx;

        piggybankLevelState += numDummyMatches;
        piggybankLevelIdx += (piggybankLevelState / dummyScoreForNextLevel);
        piggybankLevelState = piggybankLevelState % dummyScoreForNextLevel;

        Save();
    }

    public LevelState GetLastLevelState()
    {
        return levelStates[levelStates.Count-1];
    }
    

    public LevelState GetLevelState(int levelIdx)
    {
        return levelStates.Find(l => l.levelIdx == levelIdx);
    }

    public static PlayerData Load()
    {
        string path = Application.persistentDataPath + "/Game.player"; 
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close(); 
            return data;
        }
        return  new PlayerData();
    }

    private void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter(); 
        string path = Application.persistentDataPath + "/Game.player";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, this);
        stream.Close();
    }
}

