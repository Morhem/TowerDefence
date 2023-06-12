using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="WaveScriptableObject", menuName ="ScriptableObjects/MonsterWave")]
public class WaveScriptableObject : ScriptableObject
{
    [SerializeField]
    public int Repeats;
    [System.NonSerialized]
    public int RepeatsDone = 0;
    [SerializeField]
    public GameObject[] Monsters;
    [SerializeField]
    public float Delay;

    public void Initialize()
    {
        RepeatsDone = 0;
    }
}
