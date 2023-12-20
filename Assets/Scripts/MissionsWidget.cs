using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class MissionsWidget : MonoBehaviour
{
    public MissionWidget prefab;
    private List<MissionWidget> missionWidgets = new List<MissionWidget>();

    void Awake()
    {
    }

    public void AddMission(string type, int amount, Texture2D texture)
    {
        var item = Instantiate(prefab, transform);
        item.transform.SetLocalX(missionWidgets.Count * 220f);
        item.Init(type, amount, texture);
        missionWidgets.Add(item);
    }

    public bool MatchMade(string type)
    {
        foreach (var item in missionWidgets)
        {
            if (item.MatchMade(type))
            {
                // mission done
                missionWidgets.Remove(item);
                return missionWidgets.Count == 0;
            }
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
