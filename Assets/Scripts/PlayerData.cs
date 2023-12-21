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
        currentLevelIdx = levelIdx+1;

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

    private static string DataPath()
    {
        string path = "Game.player";

        //#if !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, path);
//#else
//        path = Path.Combine(Application.dataPath, path);
//#endif
        return path;
    }

    public static PlayerData Load()
    {
        string path = DataPath(); 
        if (File.Exists(path))
        {
            string fromjson = File.ReadAllText(path);
            if (fromjson != "")
            {
                try 
                {
                    var player = JsonUtility.FromJson<PlayerData>(fromjson);
                    return player;
                }
                catch (Exception)
                {
                }
            }
        }
        return  new PlayerData();
    }

    private void Save()
    {
        string toJson = JsonUtility.ToJson(this, true);
        File.WriteAllText(DataPath(), toJson);
    }
}

