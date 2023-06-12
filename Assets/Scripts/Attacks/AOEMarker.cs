using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEMarker : MonoBehaviour
{
    [SerializeField]
    float duration;
    void Update()
    {
        duration -= Time.deltaTime;
        if (duration < 0f)
            GameObject.Destroy(gameObject);
    }
}
