using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    public int Damage;
    public float Speed;


    void Update()
    {
        if (MissionController.main.Pause)
            return;
        var step = Speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, MissionController.main.transform.position, step);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tower")
        {
            HitTower();
        }
    }
    public void HitTower()
    {
        MissionController.main.GetHit(Damage);
        GameObject.Destroy(gameObject);
    }
}
