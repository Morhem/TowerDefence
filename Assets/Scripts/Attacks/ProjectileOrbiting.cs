using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileOrbiting : BaseTowerAttack
{
    [System.NonSerialized]
    float angularSpeed;
    [SerializeField]
    bool DestroyOnMonsterHit;
    float TTL;


    void Start()
    {
        GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (MissionController.main.Pause)
            return;
        //transform.RotateAround(MissionController.main.transform.position, Vector3.up, angularSpeed * Time.deltaTime + (360f / orbitQuantity) * orbitIndex);
        transform.RotateAround(MissionController.main.transform.position, Vector3.up, angularSpeed * Time.deltaTime);
    }

    public override void SetStats(CharacterStatBlock stats)
    {
        angularSpeed = stats.CurrentProjectileSpeed;
        TTL = stats.CurrentTTL;
        base.SetStats(stats);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        {

            var ms = other.GetComponent<Monster>();
            var hit = HitMonster(ms);
            if (hit)
            {
                if (DestroyOnMonsterHit)
                {
                    Destruction();
                }
            }

        }

    }

}
