using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGuided : BaseTowerAttack
{
    public GameObject explosionPrefab;
    public GameObject Target;

    public string TargetTag;

    public float Speed;

    public bool TurnIntoAnotherProjectile;
    public bool AnotherProjectilePrefab;


    public override void SetStats(CharacterStatBlock stats)
    {
        Target = stats.Target;
        base.SetStats(stats);
    }

    private void LateUpdate()
    {
        if (MissionController.main.Pause)
            return;
        if (Target == null || !Target.activeSelf)
            return;
        var step = Speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, step);
    }
    void Update()
    {
        if (MissionController.main.Pause)
            return;
        if (Target == null || !Target.activeSelf)
            Destruction();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == TargetTag)
        {
            var ms = other.GetComponent<Monster>();
            HitMonster(ms);
        }
    }
    public override bool HitMonster(Monster monster)
    {
        var hit = base.HitMonster(monster);
        if (hit)
        {
            Destruction();
        }
        return hit;
    }
    
}
