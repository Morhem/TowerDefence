using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveButton : MonoBehaviour
{
    public void Revive()
    {
        MissionController.main.Revive();
    }
}
