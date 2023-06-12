using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionScriptableObject", menuName = "ScriptableObjects/Mission")]
public class MissionScriptableObject : ScriptableObject
{
    public string MissionName;
    public string SceneName;
}
