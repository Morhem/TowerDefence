using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPack : MonoBehaviour
{
    void Start()
    {
        transform.DetachChildren();
        GameObject.Destroy(gameObject);
    }
}
