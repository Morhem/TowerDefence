using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMissions : MonoBehaviour
{
    void Start()
    {
        var mssionButton = Resources.Load<GameObject>($"UIPrefabs/MissionButton");
        var missions = Resources.LoadAll<MissionScriptableObject>($"Prefabs/Missions/");
        foreach (var mission in missions)
        {
            var button = GameObject.Instantiate(mssionButton, transform).GetComponent<MissionSelectButton>();
            button.MissionName = mission.MissionName;
            button.SceneName = mission.SceneName;
        }
        //var missions = Resources.LoadAll<GameObject>($"UIPrefabs/Missions/");
        //foreach (var mission in missions)
        //{
        //    GameObject.Instantiate(mission, transform);
        //}
    }
}
