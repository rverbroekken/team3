using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using sgg;

public class MissionsWidget : MonoBehaviour
{
    public MissionWidget prefab;
    private List<MissionWidget> missionWidgets = new List<MissionWidget>();

    void Awake()
    {
    }

    public void NewGame(LevelData levelData)
    {
        foreach (var fruitData in levelData.fruit)
        {
            if (fruitData?.isGoalItem == true)
            {
                var goal = fruitData.goalAmount;
                if (goal <= 0)
                {
                    goal = fruitData.amount;
                }
                AddMission(fruitData.fruit.type, goal * 3, fruitData.fruit.texture);
            }
        }
        MoveMissionWidgets();
    }

    private void AddMission(string type, int amount, Texture2D texture)
    {
        var item = Instantiate(prefab, transform);
        item.transform.SetLocalX(-300f);
        item.Init(type, amount, texture);
        missionWidgets.Add(item);
    }
    public void MoveMissionWidgets()
    {
        for (var i = 0; i < missionWidgets.Count; i++)
        {
            var item = missionWidgets[i];
            item.DOKill();
            item.transform.DOLocalMoveX(i * 220f, 0.4f);
        }
        
    }

    public bool ItemToTray(string type)
    {
        foreach (var item in missionWidgets)
        {
            if (item.ItemToTray(type))
            {
                // mission done
                missionWidgets.Remove(item);
                Destroy(item.gameObject);

                MoveMissionWidgets();

                return missionWidgets.Count == 0;
            }
        }
        return false;
    }

    public bool ItemFromTray(string type)
    {
        foreach (var item in missionWidgets)
        {
            item.ItemFromTray(type);
        }
        return false;
    }

    public void Clear()
    {
        foreach (var item in missionWidgets)
        {
            Destroy(item.gameObject);
        }
        missionWidgets.Clear();
    }

}
