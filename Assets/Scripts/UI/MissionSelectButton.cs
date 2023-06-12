using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionSelectButton : MonoBehaviour
{
    public string MissionName;
    [System.NonSerialized]
    public string SceneName;

    private void Start()
    {
        var text = GetComponentInChildren<TMPro.TMP_Text>();
        text.text = MissionName;
    }

    public void StartMission()
    {
        MissionController.main.LoadMission(SceneName);
    }
}
